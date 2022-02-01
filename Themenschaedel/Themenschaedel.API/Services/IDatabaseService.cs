using Themenschaedel.Shared.Models;
using Themenschaedel.Shared.Models.Request;

namespace Themenschaedel.API.Services
{
    public interface IDatabaseService
    {
        // AuthenticationService
        public Task RegiserUser(UserRegistrationExtended user);
        public Task<bool> IsRegistrationUsernameUnique(string username); // Checks if there are any other users with the same name
        public Task<bool> IsRegistrationEmailUnique(string email); // Checks if there are any other users with the same email
        public Task VerifyEmailAddress(string verificationId);
        public Task<User> GetUserByUsername(string username);
        public Task<User> GetUserByUserId(int userId);
        public Task CreateRefreshToken(TokenExtended token);
        public Task<TokenCache> GetRefreshToken(string refreshToken);
        public Task ClearAllToken(int userId);
        public Task ClearSingleToken(string refreshToken);

        // Epsiodes
        public Task<List<EpisodeExtended>> GetEpisodesAsync(int page, int perPage);
        public Task<List<Episode>> GetAllEpisodesAsync();
        public List<Episode> GetAllEpisodes();
        public void AddEpisodes(List<Episode> episodes);
    }
}
