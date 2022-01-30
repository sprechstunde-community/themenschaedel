using System;
using System.Collections.Generic;
using System.Text;

namespace Themenschaedel.Shared.Models.Response
{
    public class UserResponse
    {
        public UserResponse(User user)
        {
            this.Id = user.Id;
            this.UUID = user.UUID;
            this.Username = user.Username;
            this.Email = user.Email;
            this.CreatedAt = user.CreatedAt;
        }

        public int Id { get; set; }
        public string UUID { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
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
}
