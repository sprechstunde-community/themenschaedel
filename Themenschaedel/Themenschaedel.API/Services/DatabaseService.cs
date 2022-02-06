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

        public async Task ClearAllTokenAsync(int userId)
        {
            _logger.LogInformation($"Clearing all tokens for user: {userId}");
            using (var connection = _context.CreateConnection())
            {
                var parameters = new { id_users = userId };
                string processQuery = "DELETE FROM token WHERE id_users=@id_users";
                await connection.ExecuteAsync(processQuery, parameters);
            }
        }

        public async Task CreateRefreshTokenAsync(TokenExtended token)
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
            var query = $"SELECT * FROM episodes ORDER BY published_at DESC;";
            using (var connection = _context.CreateConnection())
            {
                var episodes = connection.Query<Episode>(query).ToList();
                return episodes;
            }
        }

        public async Task<List<EpisodeExtendedExtra>> GetAllEpisodesAsync()
        {
            _logger.LogInformation($"Returning all episodes from database.");
            var query = $"SELECT * FROM udf_GetEpisodes();";
            using (var connection = _context.CreateConnection())
            {
                var episodes = await connection.QueryAsync<EpisodeExtendedExtra>(query);
                List<EpisodeExtendedExtra> episodesList = episodes.ToList();
                List<Topic> topics = await GetAllTopicsSimpleAsync();
                List<Subtopic> subtopics = await GetAllSubtopicsAsync();
                for (int i = 0; i < episodesList.Count; i++)
                {
                    if (episodesList[i].Verified)
                    {
                        List<Topic> foundTopics = topics.FindAll(x => x.EpisodeId == episodesList[i].Id);
                        List<TopicExtended> topicsToAdd = new List<TopicExtended>();
                        for (int j = 0; j < foundTopics.Count; j++)
                        {
                            List<Subtopic> subtopicsToAdd = subtopics.FindAll(x => x.TopicId == foundTopics[j].Id);
                            topicsToAdd.Add(new TopicExtended(foundTopics[j], subtopicsToAdd));
                        }

                        episodesList[i].Topic = topicsToAdd;
                    }
                    episodesList[i].Person = await GetPeopleFeaturedInEpisodeByEpisodeIdAsync(episodesList[i].Id);
                }
                return episodes.ToList();
            }
        }

        public async Task<List<EpisodeExtended>> GetEpisodesAsync(int page, int perPage)
        {
            _logger.LogInformation($"Returning all episodes from database, page: {page} per page: {perPage}.");
            var parameters = new { Page = page, PerPage = perPage };
            var query = $"SELECT * FROM udf_episodes_GetRowsByPageNumberAndSize(@Page,@PerPage);";
            using (var connection = _context.CreateConnection())
            {
                var episodes = await connection.QueryAsync<EpisodeExtended>(query, parameters);
                return episodes.ToList();
            }
        }

        public async Task<TokenCache> GetRefreshTokenAsync(string refreshToken)
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

        public async Task<bool> IsRegistrationEmailUniqueAsync(string email)
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

        public async Task<bool> IsRegistrationUsernameUniqueAsync(string username)
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

        public async Task RegiserUserAsync(UserRegistrationExtended user)
        {
            _logger.LogInformation($"Registering new User with UUID: {user.UUID}.");
            using (var connection = _context.CreateConnection())
            {
                var parameters = new { uuid = user.UUID, username = user.Username, email = user.Email, email_verification_id = user.EmailValidationId, password = user.Password, salt = user.Salt, created_at = DateTime.Now, id_roles = user.RoleId };
                string processQuery = "INSERT INTO users (uuid,username,email,email_verification_id,password,salt,created_at,id_roles) VALUES (@uuid,@username,@email,@email_verification_id,@password,@salt,@created_at,@id_roles)";
                await connection.ExecuteAsync(processQuery, parameters);
            }
        }

        public async Task VerifyEmailAddressAsync(string verificationId)
        {
            _logger.LogInformation($"Verifying email with verification id: {verificationId}.");
            using (var connection = _context.CreateConnection())
            {
                var parameters = new { email_verification_id = verificationId, new_id = "", email_verified_at = DateTime.Now };
                string processQuery = "UPDATE users SET email_verification_id=@new_id,email_verified_at=@email_verified_at WHERE email_verification_id=@email_verification_id;";
                await connection.ExecuteAsync(processQuery, parameters);
            }
        }

        public async Task<User> GetUserByUsernameAsync(string username)
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

        public async Task<User> GetUserByUserIdAsync(int userId)
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

        public async Task ClearSingleTokenAsync(string refreshToken)
        {
            _logger.LogInformation($"Clearing sinlge refresh token with value: {refreshToken}.");
            using (var connection = _context.CreateConnection())
            {
                var parameters = new { refresh_token = refreshToken };
                string processQuery = "DELETE FROM token WHERE value=@refresh_token";
                await connection.ExecuteAsync(processQuery, parameters);
            }
        }

        public async Task<EpisodeExtendedExtra> GetEpisodeAsync(int episodeId, bool editorRequest = false)
        {
            _logger.LogInformation($"Returning episode with id: {episodeId}.");
            var parameters = new { epId = episodeId };
            var query = $"SELECT * FROM udf_GetEpisodes() WHERE id=@epId LIMIT 1";
            using (var connection = _context.CreateConnection())
            {
                EpisodeExtendedExtra episode = await connection.QuerySingleAsync<EpisodeExtendedExtra>(query, parameters);
                if (episode.Verified || editorRequest)
                {
                    episode.Topic = await GetTopicsAsync(episode.Id);
                }
                episode.Person = await GetPeopleFeaturedInEpisodeByEpisodeIdAsync(episode.Id);
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

        public async Task<List<Subtopic>> GetSubtopicsAsync(Int64 topicId)
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

        public async Task<List<Subtopic>> GetAllSubtopicsAsync()
        {
            _logger.LogInformation($"Returning all subtopics from the database.");
            var query = $"SELECT * FROM subtopics;";
            using (var connection = _context.CreateConnection())
            {
                var subtopics = await connection.QueryAsync<Subtopic>(query);
                return subtopics.ToList();
            }
        }

        public async Task<List<Topic>> GetAllTopicsSimpleAsync()
        {
            _logger.LogInformation($"Returning all topic from database.");
            var query = $"SELECT * FROM topic ORDER BY id_episodes;";
            using (var connection = _context.CreateConnection())
            {
                var topics = await connection.QueryAsync<Topic>(query);
                List<Topic> topicsList = topics.ToList();
                return topicsList;
            }
        }

        public async Task<int> GetEpisodeCountAsync()
        {
            _logger.LogInformation($"Returning current episode count in databse.");
            var query = $"SELECT COUNT(*) FROM episodes;";
            using (var connection = _context.CreateConnection())
            {
                int episodeCount = await connection.QuerySingleAsync<int>(query);
                return episodeCount;
            }
        }

        public async Task VerifyEpisodeAsync(int episodeId)
        {
            _logger.LogInformation($"Verifying episode with id: {episodeId}.");
            using (var connection = _context.CreateConnection())
            {
                var parameters = new { epId = episodeId, updatedAt = DateTime.Now };
                string processQuery = "UPDATE episodes SET verified=true,updated_at=@updatedAt WHERE id=@epId;";
                await connection.ExecuteAsync(processQuery, parameters);
            }
        }

        public async Task UnverifyEpisodeAsync(int episodeId)
        {
            _logger.LogInformation($"Removing verification for episode with id: {episodeId}.");
            using (var connection = _context.CreateConnection())
            {
                var parameters = new { epId = episodeId, updatedAt = DateTime.Now };
                string processQuery = "UPDATE episodes SET verified=false,updated_at=@updatedAt WHERE id=@epId;";
                await connection.ExecuteAsync(processQuery, parameters);
            }
        }

        public async Task<List<EpisodeExtendedExtra>> GetEpisodeAwaitingVerificationAsync(int page, int perPage)
        {
            _logger.LogInformation($"Returning all unverified episodes from database.");
            var parameters = new { Page = page, PerPage = perPage };
            var query = $"SELECT * FROM udf_episodes_unverified_GetRowsByPageNumberAndSize(@Page, @PerPage);";
            using (var connection = _context.CreateConnection())
            {
                var episodes = await connection.QueryAsync<EpisodeExtendedExtra>(query, parameters);
                List<EpisodeExtendedExtra> episodesList = episodes.ToList();
                List<Topic> topics = await GetAllTopicsSimpleAsync();
                List<Subtopic> subtopics = await GetAllSubtopicsAsync();
                for (int i = 0; i < episodesList.Count; i++)
                {
                    List<Topic> foundTopics = topics.FindAll(x => x.EpisodeId == episodesList[i].Id);
                    List<TopicExtended> topicsToAdd = new List<TopicExtended>();
                    for (int j = 0; j < foundTopics.Count; j++)
                    {
                        List<Subtopic> subtopicsToAdd = subtopics.FindAll(x => x.TopicId == foundTopics[j].Id);
                        topicsToAdd.Add(new TopicExtended(foundTopics[j], subtopicsToAdd));
                    }
                    episodesList[i].Topic = topicsToAdd;

                    episodesList[i].Person = await GetPeopleFeaturedInEpisodeByEpisodeIdAsync(episodesList[i].Id);
                }
                return episodes.ToList();
            }
        }

        public async Task<int> GetUnverifiedEpisodeCountAsync()
        {
            _logger.LogInformation($"Returning current unverified episode count in databse.");
            var query = $"SELECT COUNT(*) FROM udf_GetUnverifiedEpisodes();";
            using (var connection = _context.CreateConnection())
            {
                int episodeCount = await connection.QuerySingleAsync<int>(query);
                return episodeCount;
            }
        }

        public async Task<List<Person>> GetPeopleFeaturedInEpisodeByEpisodeIdAsync(int episodeId)
        {
            _logger.LogInformation($"Returning all people featured in episode, by episode id. Episode ID: {episodeId}.");
            var parameters = new { epId = episodeId };
            var query = $"SELECT c.* FROM episodes a " +
                        $"LEFT JOIN episode_person b ON b.id_episodes = a.id " +
                        $"LEFT JOIN person c ON c.id = b.id_person " +
                        $"WHERE a.id = @epId;";
            using (var connection = _context.CreateConnection())
            {
                var people = await connection.QueryAsync<Person>(query, parameters);
                List<Person> peopleList = people.ToList();
                if (peopleList.Count == 0) return null;
                if (peopleList[0].Id == 0) return null;
                return people.ToList();
            }
        }

        public async Task<bool> CheckIfEpisodeIsClaimedByEpisodeIdAsync(int episodeId)
        {
            _logger.LogInformation($"Returning claim status for episode id: {episodeId}.");
            var parameters = new { epId = episodeId };
            var query = $"SELECT COUNT(b.*) FROM episodes a " +
                        $"LEFT JOIN claims b ON b.id_episodes = a.id " +
                        $"WHERE a.id = @epId;";
            using (var connection = _context.CreateConnection())
            {
                int episodeClaimCount = await connection.QuerySingleAsync<int>(query, parameters);
                return episodeClaimCount == 1;
            }
        }

        public async Task<UserMinimal> GetUserFromClaimByEpisodeIdAsync(int episodeId)
        {
            _logger.LogInformation($"Returning all people featured in episode, by episode id. Episode ID: {episodeId}.");
            var parameters = new { epId = episodeId };
            var query = $"SELECT c.* FROM episodes a " +
                        $"LEFT JOIN episode_person b ON b.id_episodes = a.id " +
                        $"LEFT JOIN users c ON c.id = b.id_user " +
                        $"WHERE a.id = @epId;";
            using (var connection = _context.CreateConnection())
            {
                var singleUser = await connection.QuerySingleAsync<UserMinimal>(query, parameters);
                return singleUser;
            }
        }

        public async Task<List<TopicExtended>> GetAllTopicsAsync()
        {
            _logger.LogInformation($"Returning all topic from database.");
            var query = $"SELECT * FROM topic ORDER BY id_episodes;";
            List<Subtopic> subtopics = await GetAllSubtopicsAsync();
            using (var connection = _context.CreateConnection())
            {
                var topics = await connection.QueryAsync<TopicExtended>(query);
                List<TopicExtended> topicsList = topics.ToList();
                for (int i = 0; i < topicsList.Count; i++)
                {
                    topicsList[i].Subtopic = subtopics.FindAll(x => x.TopicId == topicsList[i].Id);
                }
                return topicsList;
            }
        }

        public async Task ClaimEpisodeAsync(int episodeId, int userId, DateTime validUntil, DateTime ClaimedAt)
        {
            _logger.LogInformation($"Creating claim on episode with id: {episodeId}. Claim was registered by user with id: {userId}.");
            using (var connection = _context.CreateConnection())
            {
                var parameters = new { epId = episodeId, uId = userId, valid_until = validUntil, claimed_at = ClaimedAt, created_at = DateTime.Now };
                string processQuery = "INSERT INTO claims (claimed_at,valid_until,created_at,id_user,id_episodes) VALUES (@claimed_at,@valid_until,@created_at,@uId,@epId);";
                await connection.ExecuteAsync(processQuery, parameters);
            }
        }

        public async Task<List<Claim>> GetAllExpiredClaimsAsync()
        {
            _logger.LogInformation($"Returning all expired claims.");
            var query = $"SELECT * FROM claims WHERE valid_until < NOW();";
            using (var connection = _context.CreateConnection())
            {
                var claims = await connection.QueryAsync<Claim>(query);
                return claims.ToList();
            }
        }

        public async Task ClearAllExpiredClaimsAsync()
        {
            _logger.LogInformation($"Deleting all expired Claims.");
            using (var connection = _context.CreateConnection())
            {
                string processQuery = "DELETE FROM claims WHERE valid_until < NOW();";
                await connection.ExecuteAsync(processQuery);
            }
        }

        public async Task<bool> CheckIfUserHasClaimOnEpisodeAsync(int episodeId, int userId)
        {
            _logger.LogInformation($"Checking if user with id: {userId} has a valid claim on episode with id: {episodeId}.");
            var parameters = new { epId = episodeId, uId = userId };
            var query = $"SELECT valid_until FROM claims WHERE id_episodes=@epId AND id_user=@uId;";
            using (var connection = _context.CreateConnection())
            {
                var validUntil = await connection.QuerySingleAsync<DateTime>(query, parameters);

                if (validUntil == null)
                {
                    _logger.LogInformation($"User with id: {userId} does not have a valid claim on episode with id: {episodeId}.");
                    return false;
                }

                bool claimStillValid = validUntil < DateTime.Now;
                if (claimStillValid) _logger.LogInformation($"User with id: {userId} has a valid claim on episode with id: {episodeId}.");
                else _logger.LogInformation($"User with id: {userId} has an expired and thereby not a valid claim on episode with id: {episodeId}.");
                return claimStillValid;
            }
        }

        public async Task<EpisodeExtended> GetMinimalEpisodeAsync(int episodeId)
        {
            _logger.LogInformation($"Returning all expired claims.");
            var parameters = new { epId = episodeId };
            var query = $"SELECT * FROM udf_GetEpisodes() WHERE id=@epId;";
            using (var connection = _context.CreateConnection())
            {
                var episode = await connection.QuerySingleAsync<EpisodeExtended>(query, parameters);
                return episode;
            }
        }

        public List<Claim> GetAllExpiredClaims()
        {
            _logger.LogInformation($"Returning all expired claims.");
            var query = $"SELECT * FROM claims WHERE valid_until < NOW();";
            using (var connection = _context.CreateConnection())
            {
                var claims = connection.Query<Claim>(query);
                return claims.ToList();
            }
        }

        public void ClearAllExpiredClaims()
        {
            _logger.LogInformation($"Deleting all expired Claims.");
            using (var connection = _context.CreateConnection())
            {
                string processQuery = "DELETE FROM claims WHERE valid_until < NOW();";
                connection.Execute(processQuery);
            }
        }

        public async Task<Episode> GetClaimedEpisodeByUserIdAsync(int userId)
        {
            _logger.LogInformation($"Returning claimed episode by user with id: {userId}.");
            var parameters = new { uId = userId };
            var query = $"SELECT a.* FROM episodes a WHERE EXISTS (SELECT FROM claims WHERE  claims.id_episodes = a.id AND claims.id_user = @uId);";
            using (var connection = _context.CreateConnection())
            {
                var episode = await connection.QuerySingleAsync<Episode>(query, parameters);
                return episode;
            }
        }

        public async Task InsertTopicAsync(ProcessedTopicPostRequest topic, int episodeId, int userId)
        {
            _logger.LogDebug($"Topic contributed by user with id: {userId} in episode with id: {episodeId} to insert: {ObjectLogger.Dump(topic)}");
            _logger.LogInformation($"Inserting list of topics.");
            var parameters = new
            {
                name = topic.Name,
                timestamp_start = topic.TimestampStart,
                timestamp_end = topic.TimestampEnd,
                duration = topic.Duration,
                community_contributed = topic.CommunityContributed,
                ad = topic.Ad,
                created_at = DateTime.Now,
                epId = episodeId,
                uId = userId
            };
            using (var connection = _context.CreateConnection())
            {
                string processQuery = "INSERT INTO topic (name,timestamp_start,timestamp_end,duration,community_contributed,ad,created_at,id_episodes,id_user) " +
                                      "VALUES (@name,@timestamp_start,@timestamp_end,@duration,@community_contributed,@ad,@created_at,@epId,@uId); SELECT * FROM LASTVAL();";
                int topicId = await connection.ExecuteScalarAsync<int>(processQuery, parameters);
                for (int i = 0; i < topic.Subtopics.Count; i++)
                {
                    await InsertSubtopicAsync(topic.Subtopics[i], topicId, userId);
                }
            }
        }

        public async Task InsertSubtopicAsync(SubtopicPostRequest subtopic, int topicId, int userId)
        {
            _logger.LogDebug($"Subtopic to insert: {subtopic.Name} by user with id: {userId} in topic with id: {topicId}");
            _logger.LogInformation($"Inserting list of topics.");
            var parameters = new
            {
                name = subtopic.Name,
                id_topic = topicId,
                id_user = userId
            };
            using (var connection = _context.CreateConnection())
            {
                string processQuery = "INSERT INTO subtopics (name,id_topic,id_user) " +
                                      "VALUES (@name,@id_topic,@id_user);";
                await connection.ExecuteAsync(processQuery, parameters);
            }
        }

        public async Task DeleteTopicAndSubtopicAsync(int episodeId)
        {
            _logger.LogDebug($"Deleting all topics from episode with id: {episodeId}");
            List<TopicExtended> topics = await GetTopicsAsync(episodeId);
            for (int i = 0; i < topics.Count; i++)
            {
                await DeleteSubtopicsByTopicIdAsync(topics[i].Id);
            }
            var parameters = new { epId = episodeId };
            using (var connection = _context.CreateConnection())
            {
                string processQuery = "DELETE FROM topic WHERE id_episodes=@epId;";
                await connection.ExecuteAsync(processQuery, parameters);
            }
        }

        private async Task DeleteSubtopicsByTopicIdAsync(Int64 topicId)
        {
            _logger.LogDebug($"Deleting all subtopic with topic id: {topicId}");
            var parameters = new { id_topic = topicId };
            using (var connection = _context.CreateConnection())
            {
                string processQuery = "DELETE FROM subtopics WHERE id_topic=@id_topic;";
                await connection.ExecuteAsync(processQuery, parameters);
            }
        }

        public async Task ResetIdentityForTopicAndSubtopicsAsync()
        {
            _logger.LogDebug($"Reseting id identity for Topic and Subtopics table");
            int lastTopicId = await GetLastIdInTopic();
            int lastSubtpicsId = await GetLastIdInSubtopics();
            //var parameters = new { last_topic_id = lastTopicId, last_subtopics_id = lastSubtpicsId };
            using (var connection = _context.CreateConnection())
            {
                string processQuery = $"ALTER SEQUENCE topic_id_seq RESTART WITH {lastTopicId + 1}; ALTER SEQUENCE subtopics_id_seq RESTART WITH {lastSubtpicsId + 3};";
                await connection.ExecuteAsync(processQuery);
            }
        }

        private async Task<int> GetLastIdInTopic()
        {
            _logger.LogDebug($"Getting last Id in table topic.");
            using (var connection = _context.CreateConnection())
            {
                string processQuery = "SELECT id FROM topic ORDER BY id DESC LIMIT 1;";
                return await connection.QuerySingleAsync<int>(processQuery);
            }
        }

        private async Task<int> GetLastIdInSubtopics()
        {
            _logger.LogDebug($"Getting last Id in table topic.");
            using (var connection = _context.CreateConnection())
            {
                string processQuery = "SELECT id FROM subtopics ORDER BY id DESC LIMIT 1;";
                return await connection.QuerySingleAsync<int>(processQuery);
            }
        }

        public async Task<Claim> GetClaimByUserIdAsync(int userId)
        {
            _logger.LogInformation($"Returning claim by user with id: {userId}.");
            var parameters = new { uId = userId };
            var query = $"SELECT * FROM claims WHERE id_user = @uId;";
            using (var connection = _context.CreateConnection())
            {
                var claim = await connection.QuerySingleAsync<Claim>(query, parameters);
                return claim;
            }
        }

        public async Task UpdateClaimsValidUntilAsync(int claimId, DateTime newValidUntilTime)
        {
            _logger.LogInformation($"Adding additional time to claim with id: {claimId}, new time is: {newValidUntilTime.ToString()}.");
            var parameters = new { cId = claimId, valid_until = newValidUntilTime};
            var query = $"UPDATE claims SET valid_until=@valid_until WHERE id=@cId";
            using (var connection = _context.CreateConnection())
            {
                await connection.ExecuteAsync(query, parameters);
            }
        }

        public async Task DeleteClaimByEpisodeIdAsync(int episodeId)
        {
            _logger.LogDebug($"Deleting claim with episode id: {episodeId}");
            var parameters = new { epId = episodeId };
            using (var connection = _context.CreateConnection())
            {
                string processQuery = "DELETE FROM claims WHERE id_episodes=@epId;";
                await connection.ExecuteAsync(processQuery, parameters);
            }
        }
    }
}
