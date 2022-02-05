using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.Net.Http.Headers;
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

        public User GetUser(string token)
        {
            TokenCache cachedToken = CheckForValidTokenByToken(token);
            if (cachedToken == null)
            {
                throw new TokenDoesNotExistException();
            }
            return cachedToken.User;
        }

        public async Task<TokenExtended> LoginAsync(string username, string password)
        {
            User user = null;
            TokenCache cachedToken = CheckForValidTokenByUsername(username);
            bool validTokenCached = cachedToken == null;
            if (validTokenCached)
            {
                user = await _databaseService.GetUserByUsernameAsync(username);
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
                token = CreateToken(user.Id);
                await _databaseService.CreateRefreshTokenAsync(token);

                TokenCache.Add(new TokenCache(token, user));
            }
            else
            {
                throw new PasswordOrEmailIncorrectException();
            }

            return token;
        }


        private TokenCache? CheckForValidTokenByUsername(string username)
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

        private TokenCache? CheckForValidTokenByToken(string token)
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

        private TokenExtended CreateToken(int userId)
        {
            TokenExtended token = new TokenExtended();
            token.UserId = userId;
            token.Value = RandomStringWithSpecialChars(128);
            token.RefreshToken = RandomStringWithSpecialChars(256);
            token.ValidUntil = DateTime.Now.AddDays(1);
            token.CreatedAt = DateTime.Now;
            return token;
        }

        public async Task<bool> LogoutAsync(string token)
        {
            TokenCache cachedToken = CheckForValidTokenByToken(token);
            if (cachedToken == null)
            {
                throw new TokenDoesNotExistException();
            }

            List<TokenCache> toClearCacheTokens = TokenCache.FindAll(x => x.RefreshToken == cachedToken.RefreshToken);
            for (int i = 0; i < toClearCacheTokens.Count; i++)
            {
                TokenCache.Remove(toClearCacheTokens[i]);
            }

            await _databaseService.ClearSingleTokenAsync(cachedToken.RefreshToken);
            return true;
        }

        public async Task<bool> RegisterAsync(UserRegistration user)
        {
            try
            {
                UserRegistrationExtended userToCreate = new UserRegistrationExtended();
                userToCreate.Email = user.Email;
                userToCreate.Username = user.Username;
                userToCreate.RoleId = 1;
                userToCreate.UUID = Guid.NewGuid().ToString("N");

                string tempPassword = $"{_configuration["Auth:FrontSalt"]}{user.Password}{_configuration["Auth:BackSalt"]}";
                byte[] salt = GetSalt();
                string hashedPassword = HashPassword(tempPassword, salt);
                userToCreate.Password = hashedPassword;
                userToCreate.Salt = salt;

                userToCreate.EmailValidationId = RandomString(64);

                await _databaseService.RegiserUserAsync(userToCreate);
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

        public async Task<bool> VerifyEmailAsync(string verificationId)
        {
            try
            {
                await _databaseService.VerifyEmailAddressAsync(verificationId);
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

        public async Task LogoutAllAsync(string token)
        {
            int cachedTokenIndex = TokenCache.FindIndex(x => x.Value == token);
            if (cachedTokenIndex == -1)
            {
                throw new TokenDoesNotExistException();
            }

            List<TokenCache> cachedTokens = TokenCache.FindAll(x => x.UserId == TokenCache[cachedTokenIndex].UserId);
            for (int i = 0; i < cachedTokens.Count; i++)
            {
                TokenCache.Remove(cachedTokens[i]);
            }

            // maybe add, if refresh token exists
            await _databaseService.ClearAllTokenAsync(cachedTokens[0].UserId);
        }

        public async Task<Token> RefreshTokenAsync(RefreshTokenRequest refreshTokenRequest)
        {
            User user = await _databaseService.GetUserByUserIdAsync(refreshTokenRequest.UserId);

            // This logic checks if a refresh token exists. If not, it will throw the exception 'RefreshTokenDoesNotExist'.
            try
            {
                TokenCache token = await _databaseService.GetRefreshTokenAsync(refreshTokenRequest.RefreshToken);
            }
            catch (RefreshTokenDoesNotExist e)
            {
                throw new RefreshTokenDoesNotExist();
            }
            
            TokenExtended extendedToken = CreateToken(user.Id);
            extendedToken.RefreshToken = refreshTokenRequest.RefreshToken;
            TokenCache cacheToken = new TokenCache(extendedToken, user);
            TokenCache.Add(cacheToken);
            Token newToken = new Token(cacheToken);
            return newToken;
        }

        public bool IsTokenValid(string token)
        {
            return CheckForValidTokenByToken(token) != null;
        }

        public bool IsTokenValid(HttpRequest request)
        {
            var authorization = request.Headers[HeaderNames.Authorization];
            string token = null;

            if (AuthenticationHeaderValue.TryParse(authorization, out var headerValue))
            {
                var scheme = headerValue.Scheme;
                token = headerValue.Parameter;
            }

            return CheckForValidTokenByToken(token) != null;
        }

        public string GetValidToken(HttpRequest request)
        {
            var authorization = request.Headers[HeaderNames.Authorization];
            string token = null;

            if (AuthenticationHeaderValue.TryParse(authorization, out var headerValue))
            {
                var scheme = headerValue.Scheme;
                token = headerValue.Parameter;
            }

            TokenCache cachedToken = CheckForValidTokenByToken(token);

            bool tokenValid = cachedToken != null;

            if (tokenValid)
            {
                return cachedToken.Value;
            }
            else
            {
                throw new TokenDoesNotExistException();
            }
        }

        /// <summary>
        /// Return user object from a valid token. This method only checks the local Token Cache.
        /// </summary>
        /// <param name="request"></param>
        /// <returns>User object</returns>
        /// <exception cref="TokenDoesNotExistException"></exception>
        public async Task<User> GetUserFromValidToken(HttpRequest request)
        {
            var authorization = request.Headers[HeaderNames.Authorization];
            string token = null;

            if (AuthenticationHeaderValue.TryParse(authorization, out var headerValue))
            {
                var scheme = headerValue.Scheme;
                token = headerValue.Parameter;
            }

            TokenCache cachedToken = CheckForValidTokenByToken(token);

            bool tokenValid = cachedToken != null;

            if (tokenValid)
            {
                return cachedToken.User;
            }
            else
            {
                throw new TokenDoesNotExistException();
            }
        }

        /// <summary>
        /// Wrapper to check if the user has elivated Permissions (Moderator or Admin role).
        /// </summary>
        /// <param name="request"></param>
        /// <returns>True/False</returns>
        /// <exception cref="TokenDoesNotExistException"></exception>
        public async Task<bool> CheckIfUserHasElivatedPermission(HttpRequest request)
        {
            User user = await GetUserFromValidToken(request);
            return RoleMisc.UserHasElevatedPermission(user.RolesId);
        }
    }
}
