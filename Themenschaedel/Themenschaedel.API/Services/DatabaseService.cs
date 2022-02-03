using Dapper;
using Themenschaedel.Shared;
using Themenschaedel.Shared.Models;
using Themenschaedel.Shared.Models.Request;
using Themenschaedel.Shared.Models.Response;

namespace Themenschaedel.API.Services
{
    public class DatabaseService : IDatabaseService
    {
        private readonly DapperService _context;
        private readonly ILogger<DatabaseService> _logger;
        public DatabaseService(DapperService context, ILogger<DatabaseService> databaseServiceLogger)
        {
            _context = context;
            _logger = databaseServiceLogger;
        }

        public void AddEpisodes(List<Episode> episodes)
        {
            using (var connection = _context.CreateConnection())
            {
                foreach (Episode item in episodes)
                {
                    _logger.LogDebug($"Adding new episode: {ObjectLogger.Dump(item)}");
                    _logger.LogInformation($"Adding new episode with EpisodeNumber: {item.EpisodeNumber}");
                    var parameters = new { uuid = item.UUID, title = item.Title, episode_number = item.EpisodeNumber, subtitle = item.Subtitle, description = item.Description, media_file = item.MediaFile, spotify_file = item.SpotifyFile, duration = item.Duration, type = item.Type, image = item.Image, explicitItem = item.Explicit, published_at = item.PublishedAt, created_at = item.CreatedAt, updated_at = item.UpdatedAt, verified = item.Verified };
                    string processQuery = "INSERT INTO episodes (uuid,title,episode_number,subtitle,description,media_file,spotify_file,duration,type,image,explicit,published_at,created_at,updated_at,verified) VALUES (@uuid,@title,@episode_number,@subtitle,@description,@media_file,@spotify_file,@duration,@type,@image,@explicitItem,@published_at,@created_at,@updated_at,@verified)";
                    connection.Execute(processQuery, parameters);
                }
            }
        }

        public async Task ClearAllToken(int userId)
        {
            _logger.LogInformation($"Clearing all tokens for user: {userId}");
            using (var connection = _context.CreateConnection())
            {
                var parameters = new { id_users = userId };
                string processQuery = "DELETE FROM token WHERE id_users=@id_users";
                await connection.ExecuteAsync(processQuery, parameters);
            }
        }

        public async Task CreateRefreshToken(TokenExtended token)
        {
            _logger.LogDebug($"Creating token for TokenExtended Object: {ObjectLogger.Dump(token)}");
            _logger.LogInformation($"Creating token for refresh token: {token.RefreshToken}");
            using (var connection = _context.CreateConnection())
            {
                var parameters = new { value = token.RefreshToken, created_at = token.CreatedAt, id_users = token.UserId };
                string processQuery = "INSERT INTO token (value,created_at,id_users) VALUES (@value,@created_at,@id_users)";
                await connection.ExecuteAsync(processQuery, parameters);
            }
        }

        public List<Episode> GetAllEpisodes()
        {
            _logger.LogInformation($"Returning all episodes from database.");
            var query = $"SELECT * FROM episodes ORDER BY published_at";
            using (var connection = _context.CreateConnection())
            {
                var episodes = connection.Query<Episode>(query).ToList();
                return episodes;
            }
        }

        public async Task<List<EpisodeExtended>> GetAllEpisodesAsync()
        {
            _logger.LogInformation($"Returning all episodes from database.");
            var query = $"SELECT * FROM episodes ORDER BY published_at";
            using (var connection = _context.CreateConnection())
            {
                var episodes = await connection.QueryAsync<EpisodeExtended>(query);
                List<EpisodeExtended> episodesList = episodes.ToList();
                for (int i = 0; i < episodesList.Count; i++)
                {
                    episodesList[i].Topic = await GetTopicsAsync(episodesList[i].Id);
                }
                return episodes.ToList();
            }
        }

        public async Task<List<Episode>> GetEpisodesAsync(int page, int perPage)
        {
            _logger.LogInformation($"Returning all episodes from database, page: {page} per page: {perPage}.");
            var parameters = new { Page = page, PerPage = perPage };
            var query = $"SELECT * FROM udf_episodes_GetRowsByPageNumberAndSize(@Page, @PerPage);";
            using (var connection = _context.CreateConnection())
            {
                var episodes = await connection.QueryAsync<Episode>(query, parameters);
                return episodes.ToList();
            }
        }

        public async Task<TokenCache> GetRefreshToken(string refreshToken)
        {
            _logger.LogInformation($"Getting refresh token from database: {refreshToken}.");
            var parameters = new { tokenVal = refreshToken };
            var query = $"SELECT * FROM token WHERE value=@tokenVal";
            using (var connection = _context.CreateConnection())
            {
                var token = await connection.QueryAsync<TokenCache>(query, parameters);
                List<TokenCache> tokenList = token.ToList();
                if (tokenList.Count == 0)
                {
                    throw new RefreshTokenDoesNotExist();
                }
                else
                {
                    return token.ToList()[0];
                }
            }
        }

        public async Task<bool> IsRegistrationEmailUnique(string email)
        {
            _logger.LogInformation($"Verifying user with email: {email}.");
            var parameters = new { email = email };
            var query = $"SELECT * FROM users WHERE email=@email";
            using (var connection = _context.CreateConnection())
            {
                var users = await connection.QueryAsync<User>(query, parameters);
                return users.ToList().Count == 0;
            }
        }

        public async Task<bool> IsRegistrationUsernameUnique(string username)
        {
            _logger.LogInformation($"Checking if username is unique. Username: {username}.");
            var parameters = new { username = username };
            var query = $"SELECT * FROM users WHERE username=@username";
            using (var connection = _context.CreateConnection())
            {
                var users = await connection.QueryAsync<User>(query, parameters);
                return users.ToList().Count == 0;
            }
        }

        public async Task RegiserUser(UserRegistrationExtended user)
        {
            _logger.LogInformation($"Registering new User with UUID: {user.UUID}.");
            using (var connection = _context.CreateConnection())
            {
                var parameters = new { uuid = user.UUID, username = user.Username, email = user.Email, email_verification_id = user.EmailValidationId, password = user.Password, salt = user.Salt, created_at = DateTime.Now, id_roles = user.RoleId };
                string processQuery = "INSERT INTO users (uuid,username,email,email_verification_id,password,salt,created_at,id_roles) VALUES (@uuid,@username,@email,@email_verification_id,@password,@salt,@created_at,@id_roles)";
                await connection.ExecuteAsync(processQuery, parameters);
            }
        }

        public async Task VerifyEmailAddress(string verificationId)
        {
            _logger.LogInformation($"Verifying email with verification id: {verificationId}.");
            using (var connection = _context.CreateConnection())
            {
                var parameters = new { email_verification_id = verificationId, new_id = "", email_verified_at = DateTime.Now };
                string processQuery = "UPDATE users SET email_verification_id=@new_id,email_verified_at=@email_verified_at WHERE email_verification_id=@email_verification_id;";
                await connection.ExecuteAsync(processQuery, parameters);
            }
        }

        public async Task<User> GetUserByUsername(string username)
        {
            _logger.LogInformation($"Returning user with username: {username}.");
            var parameters = new { username = username };
            var query = $"SELECT * FROM users WHERE username=@username";
            using (var connection = _context.CreateConnection())
            {
                var users = await connection.QueryAsync<User>(query, parameters);
                List<User> user = users.ToList();
                if (user.Count != 0)
                {
                    return users.ToList()[0];
                }
                else
                {
                    throw new UserDoesNotExistException();
                }
            }
        }

        public async Task<User> GetUserByUserId(int userId)
        {
            _logger.LogInformation($"Returning user with id: {userId}.");
            var parameters = new { userIdVal = userId };
            var query = $"SELECT * FROM users WHERE id=@userIdVal";
            using (var connection = _context.CreateConnection())
            {
                var users = await connection.QueryAsync<User>(query, parameters);
                List<User> user = users.ToList();
                if (user.Count != 0)
                {
                    return users.ToList()[0];
                }
                else
                {
                    throw new UserDoesNotExistException();
                }
            }
        }

        public async Task ClearSingleToken(string refreshToken)
        {
            _logger.LogInformation($"Clearing sinlge refresh token with value: {refreshToken}.");
            using (var connection = _context.CreateConnection())
            {
                var parameters = new { refresh_token = refreshToken };
                string processQuery = "DELETE FROM token WHERE value=@refresh_token";
                await connection.ExecuteAsync(processQuery, parameters);
            }
        }

        public async Task<EpisodeExtended> GetEpisodeAsync(int episodeId)
        {
            _logger.LogInformation($"Returning episode with id: {episodeId}.");
            var parameters = new { epId = episodeId };
            var query = $"SELECT * FROM episodes WHERE id=@epId LIMIT 1";
            using (var connection = _context.CreateConnection())
            {
                EpisodeExtended episode = await connection.QuerySingleAsync<EpisodeExtended>(query, parameters);
                if (episode.Verified)
                {
                    episode.Topic = await GetTopicsAsync(episode.Id);
                }
                return episode;
            }
        }

        public async Task<List<TopicExtended>> GetTopicsAsync(int episodeId)
        {
            _logger.LogInformation($"Returning all topics from database with Episode ID: {episodeId}.");
            var parameters = new { epId = episodeId };
            var query = $"SELECT * FROM topic WHERE id_episodes=@epId";
            using (var connection = _context.CreateConnection())
            {
                var topics = await connection.QueryAsync<TopicExtended>(query, parameters);
                List<TopicExtended> topicsList = topics.ToList();
                for (int i = 0; i < topicsList.Count; i++)
                {
                    topicsList[i].Subtopic = await GetSubtopicsAsync(topicsList[i].Id);
                }
                return topics.ToList();
            }
        }

        public async Task<List<Subtopic>> GetSubtopicsAsync(int topicId)
        {
            _logger.LogInformation($"Returning all subtopics from database with Topic ID: {topicId}.");
            var parameters = new { topId = topicId };
            var query = $"SELECT * FROM subtopics WHERE id_topic=@topId";
            using (var connection = _context.CreateConnection())
            {
                var subtopics = await connection.QueryAsync<Subtopic>(query, parameters);
                return subtopics.ToList();
            }
        }

        public async Task<List<TopicExtended>> GetAllTopics()
        {
            _logger.LogInformation($"Returning all topic from database.");
            var query = $"SELECT * FROM topic ORDER BY id_episodes";
            using (var connection = _context.CreateConnection())
            {
                var topics = await connection.QueryAsync<TopicExtended>(query);
                List<TopicExtended> topicsList = topics.ToList();
                if (topicsList.Count == 0) throw new EmptyDatabaseListReturnException();
                for (int i = 0; i < topicsList.Count; i++)
                {
                    topicsList[i].Subtopic = await GetSubtopicsAsync(topicsList[i].Id);
                }
                return topicsList;
            }
        }

        public async Task<int> GetEpisodeCount()
        {
            _logger.LogInformation($"Returning current episode count in databse.");
            var query = $"SELECT COUNT(*) FROM episodes;";
            using (var connection = _context.CreateConnection())
            {
                int episodeCount = await connection.QuerySingleAsync<int>(query);
                return episodeCount;
            }
        }

        public async Task VerifyEpisode(int episodeId)
        {
            _logger.LogInformation($"Verifying episode with id: {episodeId}.");
            using (var connection = _context.CreateConnection())
            {
                var parameters = new { epId = episodeId, updatedAt = DateTime.Now };
                string processQuery = "UPDATE episodes SET verified=true,updated_at=@updatedAt WHERE id=@epId;";
                await connection.ExecuteAsync(processQuery, parameters);
            }
        }

        public async Task UnverifyEpisode(int episodeId)
        {
            _logger.LogInformation($"Removing verification for episode with id: {episodeId}.");
            using (var connection = _context.CreateConnection())
            {
                var parameters = new { epId = episodeId, updatedAt = DateTime.Now };
                string processQuery = "UPDATE episodes SET verified=false,updated_at=@updatedAt WHERE id=@epId;";
                await connection.ExecuteAsync(processQuery, parameters);
            }
        }

        public async Task<List<EpisodeExtended>> GetEpisodeAwaitingVerificationAsync(int page, int perPage)
        {
            _logger.LogInformation($"Returning all unverified episodes from database.");
            var parameters = new { Page = page, PerPage = perPage };
            var query = $"SELECT * FROM udf_episodes_unverified_GetRowsByPageNumberAndSize(@Page, @PerPage);";
            using (var connection = _context.CreateConnection())
            {
                var episodes = await connection.QueryAsync<EpisodeExtended>(query, parameters);
                List<EpisodeExtended> episodesList = episodes.ToList();
                for (int i = 0; i < episodesList.Count; i++)
                {
                    episodesList[i].Topic = await GetTopicsAsync(episodesList[i].Id);
                }
                return episodes.ToList();
            }
        }

        public async Task<int> GetUnverifiedEpisodeCount()
        {
            _logger.LogInformation($"Returning current unverified episode count in databse.");
            var query = $"SELECT COUNT(*) FROM udf_episodes_unverified_GetRowsByPageNumberAndSize(1, 2147483647);";
            using (var connection = _context.CreateConnection())
            {
                int episodeCount = await connection.QuerySingleAsync<int>(query);
                return episodeCount;
            }
        }
    }

    [Serializable]
    public class EmptyDatabaseListReturnException : Exception
    {
        public EmptyDatabaseListReturnException()
        { }

        public EmptyDatabaseListReturnException(string message)
            : base(message)
        { }

        public EmptyDatabaseListReturnException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}
