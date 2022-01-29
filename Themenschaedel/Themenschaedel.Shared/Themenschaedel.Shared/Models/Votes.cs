using System;
using System.Collections.Generic;
using System.Text;

namespace Themenschaedel.Shared.Models
{
    public class Votes
    {
        public int Id { get; set; }
        public bool Positive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public int EpisodeId { get; set; }
        public int UserId { get; set; }
    }
}
