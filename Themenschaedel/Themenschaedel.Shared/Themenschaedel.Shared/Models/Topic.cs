using System;
using System.Collections.Generic;
using System.Text;

namespace Themenschaedel.Shared.Models
{
    // ToDo: Add user prop
    public class Topic
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int TimestampStart { get; set; }
        public int TimestampEnd { get; set; }
        public int Duration { get; set; }
        public bool CommunityContributed { get; set; }
        public bool Ad { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public int EpisodeId { get; set; }
        public int UserId { get; set; }
    }

    public class TopicExtended : Topic
    {
        public List<Subtopic> Subtopic { get; set; } = new List<Subtopic>();
    }
}
