using System;
using System.Collections.Generic;
using System.Text;

namespace Themenschaedel.Shared.Models.Response
{
    public class LoginResponse
    {
        public string AccessToken { get; set; }
        public string TokenType { get; set; }
        public DateTime ValidUntil { get; set; }
    }
}
