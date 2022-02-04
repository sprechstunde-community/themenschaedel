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
}
