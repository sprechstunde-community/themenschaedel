﻿using Themenschaedel.Shared.Models;
using Themenschaedel.Shared.Models.Request;
using Themenschaedel.Shared.Models.Response;

namespace Themenschaedel.API.Services
{
    public interface IDatabaseService
    {
        // AuthenticationService
        public Task RegiserUserAsync(UserRegistrationExtended user);
        public Task<bool> IsRegistrationUsernameUniqueAsync(string username); // Checks if there are any other users with the same name
        public Task<bool> IsRegistrationEmailUniqueAsync(string email); // Checks if there are any other users with the same email
        public Task VerifyEmailAddressAsync(string verificationId);
        public Task<User> GetUserByUsernameAsync(string username);
        public Task<User> GetUserByUserIdAsync(int userId);
        public Task CreateRefreshTokenAsync(TokenExtended token);
        public Task<TokenCache> GetRefreshTokenAsync(string refreshToken);
        public Task ClearAllTokenAsync(int userId);
        public Task ClearSingleTokenAsync(string refreshToken);

        // Epsiodes
        public Task<EpisodeExtendedExtra> GetEpisodeAsync(int episodeId, bool editorRequest = false);
        public Task<EpisodeExtended> GetMinimalEpisodeAsync(int episodeId);
        public Task<List<EpisodeExtended>> GetEpisodesAsync(int page, int perPage);
        public Task<List<EpisodeExtendedExtra>> GetAllEpisodesAsync();
        public Task<List<EpisodeExtendedExtra>> GetEpisodeAwaitingVerificationAsync(int page, int perPage);
        public List<Episode> GetAllEpisodes();
        public void AddEpisodes(List<Episode> episodes);
        public Task<int> GetEpisodeCountAsync();
        public Task<int> GetUnverifiedEpisodeCountAsync();
        public Task VerifyEpisodeAsync(int episodeId);
        public Task UnverifyEpisodeAsync(int episodeId);

        // Topics
        public Task<List<TopicExtended>> GetTopicsAsync(int episodeId);
        public Task<List<Topic>> GetAllTopicsSimpleAsync();
        public Task<List<TopicExtended>> GetAllTopicsAsync();
        public Task InsertTopicAsync(ProcessedTopicPostRequest topic, int episodeId, int userId);
        public Task DeleteTopicAndSubtopicAsync(int episodeId);

        // Subtopics
        public Task<List<Subtopic>> GetSubtopicsAsync(Int64 topicId);

        // Person
        public Task<List<Person>> GetPeopleFeaturedInEpisodeByEpisodeIdAsync(int episodeId);

        // Claims
        public Task<bool> CheckIfEpisodeIsClaimedByEpisodeIdAsync(int episodeId);
        public Task<UserMinimal> GetUserFromClaimByEpisodeIdAsync(int episodeId);
        public Task ClaimEpisodeAsync(int episodeId, int userId, DateTime validUntil, DateTime ClaimedAt);
        public List<Claim> GetAllExpiredClaims();
        public void ClearAllExpiredClaims();
        public Task<List<Claim>> GetAllExpiredClaimsAsync();
        public Task ClearAllExpiredClaimsAsync();
        public Task<bool> CheckIfUserHasClaimOnEpisodeAsync(int episodeId, int userId);
        public Task<Episode> GetClaimedEpisodeByUserIdAsync(int userId);
        public Task<Claim> GetClaimByUserIdAsync(int userId);
        public Task UpdateClaimsValidUntil(int claimId, DateTime newValidUntilTime);

        public Task ResetIdentityForTopicAndSubtopicsAsync();
    }
}
