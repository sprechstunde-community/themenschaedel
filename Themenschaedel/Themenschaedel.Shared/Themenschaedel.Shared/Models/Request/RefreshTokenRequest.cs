using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Themenschaedel.Shared.Models.Request
{
    public class RefreshTokenRequest
    {
        [JsonPropertyName("refresh_token")]
        public string RefreshToken { get; set; }

        [JsonPropertyName("user_id")]
        public int UserId { get; set; }
    }
}
