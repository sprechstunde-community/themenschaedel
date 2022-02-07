using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Themenschaedel.Shared.Models.Response
{
    public class UserResponse
    {
        public UserResponse(){}

        public UserResponse(User user)
        {
            this.Id = user.Id;
            this.UUID = user.UUID;
            this.Username = user.Username;
            this.Email = user.Email;
            this.CreatedAt = user.CreatedAt;
        }

        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("uuid")]
        public string UUID { get; set; }

        [JsonPropertyName("username")]
        public string Username { get; set; }

        [JsonPropertyName("email")]
        public string Email { get; set; }

        [JsonPropertyName("created_at")]
        public DateTime CreatedAt { get; set; }
    }

    [Serializable]
    public class TokenDoesNotExistException : Exception
    {
        public TokenDoesNotExistException()
        { }

        public TokenDoesNotExistException(string message)
            : base(message)
        { }

        public TokenDoesNotExistException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }

    [Serializable]
    public class RefreshTokenDoesNotExist : Exception
    {
        public RefreshTokenDoesNotExist()
        { }

        public RefreshTokenDoesNotExist(string message)
            : base(message)
        { }

        public RefreshTokenDoesNotExist(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}
