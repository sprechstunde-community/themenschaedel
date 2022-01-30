﻿using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Sentry;
using Themenschaedel.Shared.Models;
using Themenschaedel.Shared.Models.Request;
using Themenschaedel.Shared.Models.Response;
using User = Themenschaedel.Shared.Models.User;

namespace Themenschaedel.API.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        public static List<TokenCache> TokenCache = new List<TokenCache>();

        private readonly ILogger<AuthenticationService> _logger;
        private readonly IDatabaseService _databaseService;
        private readonly IMailService _mailService;
        private readonly IConfiguration _configuration;

        public AuthenticationService(ILogger<AuthenticationService> logger, IDatabaseService databaseService, IMailService mailService, IConfiguration configuration)
        {
            _logger = logger;
            _databaseService = databaseService;
            _mailService = mailService;
            _configuration = configuration;
        }

        public async Task<User> GetUser(string token)
        {
            TokenCache cachedToken = await CheckForValidTokenByToken(token);
            if (cachedToken == null)
            {
                throw new TokenDoesNotExistException();
            }
            return cachedToken.User;
        }

        public async Task<TokenExtended> Login(string username, string password)
        {
            User user = null;
            TokenCache cachedToken = await CheckForValidTokenByUsername(username);
            bool validTokenCached = cachedToken == null;
            if (validTokenCached)
            {
                user = await _databaseService.GetUserByUsername(username);
            }
            else
            {
                user = cachedToken.User;
            }

            if (user.EmailVerifiedAt == null) throw new UserEmailNotVerifiedException();

            string tempPassword = $"{_configuration["Auth:FrontSalt"]}{password}{_configuration["Auth:BackSalt"]}";
            byte[] salt = user.Salt;
            string hashedPassword = HashPassword(tempPassword, salt);

            TokenExtended token = null;

            if (hashedPassword == user.Password)
            {
                if (!validTokenCached) return new TokenExtended(cachedToken);

                token = await CreateToken(user.Id);
                await _databaseService.CreateRefreshToken(token);

                TokenCache.Add(new TokenCache(token, user));
            }
            else
            {
                throw new PasswordOrEmailIncorrectException();
            }

            return token;
        }


        private async Task<TokenCache> CheckForValidTokenByUsername(string username)
        {
            int tokenCacheIndex = TokenCache.FindIndex(x => x.User.Username == username);
            if (tokenCacheIndex != -1)
            {
                TokenCache cachedToken = TokenCache[tokenCacheIndex];
                if (cachedToken.ValidUntil > DateTime.Now)
                {
                    return cachedToken;
                }
                else
                {
                    TokenCache.RemoveAt(tokenCacheIndex);
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        private async Task<TokenCache> CheckForValidTokenByToken(string token)
        {
            int tokenCacheIndex = TokenCache.FindIndex(x => x.Value == token);
            if (tokenCacheIndex != -1)
            {
                TokenCache cachedToken = TokenCache[tokenCacheIndex];
                if (cachedToken.ValidUntil > DateTime.Now)
                {
                    return cachedToken;
                }
                else
                {
                    TokenCache.RemoveAt(tokenCacheIndex);
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        private async Task<TokenExtended> CreateToken(int userId)
        {
            TokenExtended token = new TokenExtended();
            token.UserId = userId;
            token.Value = RandomStringWithSpecialChars(128);
            token.RefreshToken = RandomStringWithSpecialChars(256);
            token.ValidUntil = DateTime.Now.AddDays(1);
            token.CreatedAt = DateTime.Now;
            return token;
        }

        public async Task<bool> Logout(string token)
        {
            TokenCache cachedToken = await CheckForValidTokenByToken(token);
            if (cachedToken == null)
            {
                throw new TokenDoesNotExistException();
            }
            return TokenCache.Remove(cachedToken);
        }

        public async Task<bool> Register(UserRegistration user)
        {
            try
            {
                UserRegistrationExtended userToCreate = new UserRegistrationExtended();
                userToCreate.Email = user.Email;
                userToCreate.Username = user.Username;
                userToCreate.UUID = Guid.NewGuid().ToString("N");

                string tempPassword = $"{_configuration["Auth:FrontSalt"]}{user.Password}{_configuration["Auth:BackSalt"]}";
                byte[] salt = GetSalt();
                string hashedPassword = HashPassword(tempPassword, salt);
                userToCreate.Password = hashedPassword;
                userToCreate.Salt = salt;

                userToCreate.EmailValidationId = RandomString(64);

                await _databaseService.RegiserUser(userToCreate);
                await _mailService.SendMail(userToCreate.Email, userToCreate.EmailValidationId);
                return true;
            }
            catch (Exception e)
            {
                _logger.LogError($"Error while trying to register new user. Error:\n{e.Message}");
                SentrySdk.CaptureException(e);
                return false;
            }
        }

        public async Task<bool> VerifyEmail(string verificationId)
        {
            try
            {
                await _databaseService.VerifyEmailAddress(verificationId);
                return true;
            }
            catch (Exception e)
            {
                _logger.LogError($"Error during email verification. Error:\n{e.Message}");
                SentrySdk.CaptureException(e);
            }

            return false;
        }


        private static Random random = new Random();

        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public static string RandomStringWithSpecialChars(int length)
        {
            const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789*&^%$#@!()_-+=";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        private static byte[] GetSalt()
        {
            byte[] salt = new byte[128 / 8];
            using (var rngCsp = new RNGCryptoServiceProvider())
            {
                rngCsp.GetNonZeroBytes(salt);
            }

            return salt;
        }

        private static string HashPassword(string password, byte[] salt)
        {
            return Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 100000,
                numBytesRequested: 256 / 8));
        }

        public async Task LogoutAll(string token)
        {
            TokenCache cachedToken = TokenCache.Find(x => x.Value == token);
            List<TokenCache> cachedTokens = TokenCache.FindAll(x => x.UserId == cachedToken.UserId);
            if (cachedTokens.Count == 0)
            {
                throw new TokenDoesNotExistException();
            }

            for (int i = 0; i < cachedTokens.Count; i++)
            {
                TokenCache.Remove(cachedTokens[i]);
            }

            // maybe add, if refresh token exists
            await _databaseService.ClearAllToken(cachedTokens[0].UserId);
        }

        public async Task<Token> RefreshToken(RefreshTokenRequest refreshTokenRequest)
        {
            User user = await _databaseService.GetUserByUserId(refreshTokenRequest.UserId);
            TokenCache token = await _databaseService.GetRefreshToken(refreshTokenRequest.RefreshToken);
            TokenExtended extendedToken = await CreateToken(user.Id);
            extendedToken.RefreshToken = refreshTokenRequest.RefreshToken;
            TokenCache cacheToken = new TokenCache(extendedToken, user);
            TokenCache.Add(cacheToken);
            Token newToken = new Token(cacheToken);
            return newToken;
        }
    }
}