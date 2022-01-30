using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Themenschaedel.Shared.Models.Response
{
    public class LoginResponse
    {
        [JsonPropertyName("refresh_token")]
        public string RefreshToken { get; set; }

        [JsonPropertyName("access_token")]
        public string AccessToken { get; set; }

        [JsonPropertyName("token_type")]
        public string TokenType { get; set; }

        [JsonPropertyName("valid_until")]
        public DateTime ValidUntil { get; set; }
    }
}
