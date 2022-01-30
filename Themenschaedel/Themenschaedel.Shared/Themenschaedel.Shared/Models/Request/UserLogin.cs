using System;
using System.Collections.Generic;
using System.Text;

namespace Themenschaedel.Shared.Models.Request
{
    public class UserLogin
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }

    [Serializable]
    public class UserDoesNotExistException : Exception
    {
        public UserDoesNotExistException()
        { }

        public UserDoesNotExistException(string message)
            : base(message)
        { }

        public UserDoesNotExistException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }

    [Serializable]
    public class PasswordOrEmailIncorrectException : Exception
    {
        public PasswordOrEmailIncorrectException()
        { }

        public PasswordOrEmailIncorrectException(string message)
            : base(message)
        { }

        public PasswordOrEmailIncorrectException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }

    [Serializable]
    public class UserEmailNotVerifiedException : Exception
    {
        public UserEmailNotVerifiedException()
        { }

        public UserEmailNotVerifiedException(string message)
            : base(message)
        { }

        public UserEmailNotVerifiedException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}
