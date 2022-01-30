﻿using Dapper;
using Themenschaedel.Shared.Models;
using Themenschaedel.Shared.Models.Request;
using Themenschaedel.Shared.Models.Response;

namespace Themenschaedel.API.Services
{
    public class DatabaseService : IDatabaseService
    {
        private readonly DapperService _context;
        public DatabaseService(DapperService context)
        {
            _context = context;
        }

        public void AddEpisodes(List<Episode> episodes)
        {
            using (var connection = _context.CreateConnection())
            {
                foreach (Episode item in episodes)
                {
                    var parameters = new { uuid = item.UUID, title = item.Title, episode_number = item.EpisodeNumber, subtitle = item.Subtitle, description = item.Description, media_file = item.MediaFile, spotify_file = item.SpotifyFile, duration = item.Duration, type = item.Type, image = item.Image, explicitItem = item.Explicit, published_at = item.PublishedAt, created_at = item.CreatedAt, updated_at = item.UpdatedAt };
                    string processQuery = "INSERT INTO episodes (uuid,title,episode_number,subtitle,description,media_file,spotify_file,duration,type,image,explicit,published_at,created_at,updated_at) VALUES (@uuid,@title,@episode_number,@subtitle,@description,@media_file,@spotify_file,@duration,@type,@image,@explicitItem,@published_at,@created_at,@updated_at)";
                    connection.Execute(processQuery, parameters);
                }
            }
        }

        public async Task ClearAllToken(int userId)
        {
            using (var connection = _context.CreateConnection())
            {
                var parameters = new { id_users = userId };
                string processQuery = "DELETE FROM token WHERE id_users=@id_users";
                await connection.ExecuteAsync(processQuery, parameters);
            }
        }

        public async Task CreateRefreshToken(TokenExtended token)
        {
            using (var connection = _context.CreateConnection())
            {
                var parameters = new { value = token.RefreshToken, created_at = token.CreatedAt, id_users = token.UserId };
                string processQuery = "INSERT INTO token (value,created_at,id_users) VALUES (@value,@created_at,@id_users)";
                await connection.ExecuteAsync(processQuery, parameters);
            }
        }

        public List<Episode> GetAllEpisodes()
        {
            var query = $"SELECT * FROM episodes ORDER BY published_at";
            using (var connection = _context.CreateConnection())
            {
                var episodes = connection.Query<Episode>(query).ToList();
                return episodes;
            }
        }

        public async Task<List<Episode>> GetAllEpisodesAsync()
        {
            var query = $"SELECT * FROM episodes ORDER BY published_at";
            using (var connection = _context.CreateConnection())
            {
                var episodes = await connection.QueryAsync<Episode>(query);
                return episodes.ToList();
            }
        }

        public async Task<List<Episode>> GetEpisodesAsync(int page, int per_page)
        {
            var parameters = new { Page = page, PerPage = per_page };
            var query = $"SELECT * FROM episodes ORDER BY published_at DESC " +
                        $"OFFSET @Page ROWS " +
                        $"FETCH NEXT @PerPage ROWS ONLY";
            using (var connection = _context.CreateConnection())
            {
                var episodes = await connection.QueryAsync<Episode>(query, parameters);
                return episodes.ToList();
            }
        }

        public async Task<TokenCache> GetRefreshToken(string refreshToken)
        {
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
            using (var connection = _context.CreateConnection())
            {
                var parameters = new { uuid = user.UUID, username = user.Username, email = user.Email, email_verification_id = user.EmailValidationId, password = user.Password, salt = user.Salt, created_at = DateTime.Now };
                string processQuery = "INSERT INTO users (uuid,username,email,email_verification_id,password,salt,created_at) VALUES (@uuid,@username,@email,@email_verification_id,@password,@salt,@created_at)";
                await connection.ExecuteAsync(processQuery, parameters);
            }
        }

        public async Task VerifyEmailAddress(string verificationId)
        {
            using (var connection = _context.CreateConnection())
            {
                var parameters = new { email_verification_id = verificationId, new_id = "", email_verified_at = DateTime.Now };
                string processQuery = "UPDATE users SET email_verification_id=@new_id,email_verified_at=@email_verified_at WHERE email_verification_id=@email_verification_id;";
                await connection.ExecuteAsync(processQuery, parameters);
            }
        }

        public async Task<User> GetUserByUsername(string username)
        {
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
    }
}