using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Themenschaedel.Shared.Models.Request
{
    public class UserRegistration
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class UserRegistrationExtended : UserRegistration
    {
        public string UUID { get; set; }
        public byte[] Salt { get; set; }

        [JsonPropertyName("email_validation_id")]
        public string EmailValidationId { get; set; }
    }
}
