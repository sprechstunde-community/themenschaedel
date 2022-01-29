using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Sentry;
using Themenschaedel.Shared.Models.Request;
using User = Themenschaedel.Shared.Models.User;

namespace Themenschaedel.API.Services
{
    public class AuthenticationService : IAuthenticationService
    {
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
            throw new NotImplementedException();
        }

        public async Task<User> Login(string username, string email, string password)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> Logout(string token)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> Register(UserRegistration user)
        {
            try
            {
                UserRegistrationExtended userToCreate = new UserRegistrationExtended();
                userToCreate.Email = user.Email;
                userToCreate.Username = user.Username;

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

        public Task<bool> VerifyEmail(string verificationId)
        {
            throw new NotImplementedException();
        }


        private static Random random = new Random();

        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
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
    }
}
