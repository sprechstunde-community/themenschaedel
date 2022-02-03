﻿using Themenschaedel.Shared.Models;
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
        public Task<EpisodeExtended> GetEpisodeAsync(int episodeId);
        public Task<List<Episode>> GetEpisodesAsync(int page, int perPage);
        public Task<List<EpisodeExtended>> GetAllEpisodesAsync();
        public Task<List<EpisodeExtended>> GetEpisodeAwaitingVerificationAsync(int page, int perPage);
        public List<Episode> GetAllEpisodes();
        public void AddEpisodes(List<Episode> episodes);
        public Task<int> GetEpisodeCount();
        public Task<int> GetUnverifiedEpisodeCount();
        public Task VerifyEpisode(int episodeId);
        public Task UnverifyEpisode(int episodeId);

        // Topics
        public Task<List<TopicExtended>> GetTopicsAsync(int episodeId);
        public Task<List<TopicExtended>> GetAllTopics();

        // Subtopics
        public Task<List<Subtopic>> GetSubtopicsAsync(int topicId);

        // Person
        public Task<List<Person>> GetPeopleFeaturedInEpisodeByEpisodeId(int episodeId);

        // Claims
        public Task<bool> CheckIfEpisodeIsClaimedByEpisodeId(int episodeId);
        public Task<UserMinimal> GetUserFromClaimByEpisodeId(int episodeId);
    }
}
