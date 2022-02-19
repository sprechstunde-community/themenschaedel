using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Themenschaedel.Shared.Models.Response
{
    public class LoginResponse
    {
        public LoginResponse(){}

        public LoginResponse(LoginResponse reponse)
        {
            this.UserId = reponse.UserId;
            RefreshToken = reponse.RefreshToken;
            AccessToken = reponse.AccessToken;
            TokenType = reponse.TokenType;
            ValidUntil = reponse.ValidUntil;
        }

        [JsonPropertyName("user_id")]
        public int UserId { get; set; }
        [JsonPropertyName("refresh_token")]
        public string RefreshToken { get; set; }

        [JsonPropertyName("access_token")]
        public string AccessToken { get; set; }

        [JsonPropertyName("token_type")]
        public string TokenType { get; set; }

        [JsonPropertyName("valid_until")]
        public DateTime ValidUntil { get; set; }
    }

    public class LoginResponseExtended : LoginResponse
    {
        public LoginResponseExtended() { }
        public LoginResponseExtended(LoginResponse reponse) : base(reponse) { }

        public DateTime SessionExpirationDate { get; set; }
    }
}
