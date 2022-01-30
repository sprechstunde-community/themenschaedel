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
            this.UserId = token.UserId;
        }

        public Token(TokenCache token)
        {
            this.Id = token.Id;
            this.Value = token.Value;
            this.ValidUntil = token.ValidUntil;
            this.CreatedAt = token.CreatedAt;
            this.UserId = token.UserId;
        }

        public Token(TokenExtended token)
        {
            this.Id = token.Id;
            this.Value = token.Value;
            this.ValidUntil = token.ValidUntil;
            this.CreatedAt = token.CreatedAt;
            this.UserId = token.UserId;
        }

        public int Id { get; set; }
        public string Value { get; set; }
        public DateTime ValidUntil { get; set; }
        public DateTime CreatedAt { get; set; }
        public int UserId { get; set; }
    }

    public class TokenCache : TokenExtended
    {
        public TokenCache() { }
        public TokenCache(TokenExtended token) : base(token){ }

        public TokenCache(TokenExtended token, User user) : base(token)
        {
            this.User = user;
        }

        public User User { get; set; }
    }

    public class TokenExtended : Token
    {
        public TokenExtended() { }

        public TokenExtended(TokenExtended token) : base(token)
        {
            this.RefreshToken = token.RefreshToken;
        }
        public string RefreshToken { get; set; }
    }
}
