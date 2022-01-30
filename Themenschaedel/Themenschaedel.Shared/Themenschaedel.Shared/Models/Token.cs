using System;
using System.Collections.Generic;
using System.Text;

namespace Themenschaedel.Shared.Models
{
    public class Token
    {
        public Token() { }

        public Token(Token token)
        {
            this.Id = token.Id;
            this.Value = token.Value;
            this.ValidUntil = token.ValidUntil;
            this.CreatedAt = token.CreatedAt;
        }

        public Token(TokenCache token)
        {
            this.Id = token.Id;
            this.Value = token.Value;
            this.ValidUntil = token.ValidUntil;
            this.CreatedAt = token.CreatedAt;
        }

        public int Id { get; set; }
        public string Value { get; set; }
        public DateTime ValidUntil { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class TokenCache : Token
    {
        public TokenCache(Token token, User user) : base(token)
        {
            this.User = user;
        }

        public User User { get; set; }
    }
}
