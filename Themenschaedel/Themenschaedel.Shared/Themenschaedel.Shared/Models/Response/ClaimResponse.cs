using System;
using System.Collections.Generic;
using System.Text;

namespace Themenschaedel.Shared.Models.Response
{
    public class ClaimResponse
    {
        public int EpisodeId { get; set; }
        public int UserId { get; set; }
        public DateTime ValidUntil { get; set; }
    }

    [Serializable]
    public class EpisodeUnclaimedButAlreadyHasTopcisException : Exception
    {
        public EpisodeUnclaimedButAlreadyHasTopcisException()
        { }

        public EpisodeUnclaimedButAlreadyHasTopcisException(string message)
            : base(message)
        { }

        public EpisodeUnclaimedButAlreadyHasTopcisException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }

    [Serializable]
    public class ClaimNotNearEnoughToInvalidationException : Exception
    {
        public ClaimNotNearEnoughToInvalidationException()
        { }

        public ClaimNotNearEnoughToInvalidationException(string message)
            : base(message)
        { }

        public ClaimNotNearEnoughToInvalidationException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }

    [Serializable]
    public class TimeExtendedTooOftenException : Exception
    {
        public TimeExtendedTooOftenException()
        { }

        public TimeExtendedTooOftenException(string message)
            : base(message)
        { }

        public TimeExtendedTooOftenException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}
