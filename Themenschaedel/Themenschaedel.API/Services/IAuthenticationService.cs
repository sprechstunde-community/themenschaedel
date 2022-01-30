using Themenschaedel.Shared.Models;
using Themenschaedel.Shared.Models.Request;

namespace Themenschaedel.API.Services
{
    public interface IAuthenticationService
    {
        public Task<bool> Register(UserRegistration user);
        public Task<TokenExtended> Login(string username, string password);
        public Task<bool> Logout(string token);
        public Task LogoutAll(string token);
        public Task<User> GetUser(string token);
        public Task<bool> VerifyEmail(string verificationId);
        public Task<Token> RefreshToken(RefreshTokenRequest refreshTokenRequest);
    }
}
