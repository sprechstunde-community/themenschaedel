using System;
using System.Collections.Generic;
using System.Text;

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
        public byte[] Salt { get; set; }
        public string EmailValidationId { get; set; }
    }
}
