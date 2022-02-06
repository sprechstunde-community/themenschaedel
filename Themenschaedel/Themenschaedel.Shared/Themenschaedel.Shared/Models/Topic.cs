using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Themenschaedel.Shared.Models
{
    // ToDo: Add user prop
    public class Topic
    {
        public Topic(){}

        public Topic(Topic topic)
        {
            Id = topic.Id;
            Name = topic.Name;
            TimestampStart = topic.TimestampStart;
            TimestampEnd = topic.TimestampEnd;
            Duration = topic.Duration;
            CommunityContributed = topic.CommunityContributed;
            Ad = topic.Ad;
            CreatedAt = topic.CreatedAt;
            UpdatedAt = topic.UpdatedAt;
            EpisodeId = topic.EpisodeId;
            UserId = topic.UserId;
        }

        [Column("id")]
        public Int64 Id { get; set; }
        [Column("name")]
        public string Name { get; set; }
        [Column("timestamp_start")]
        public int TimestampStart { get; set; }
        [Column("timestamp_end")]
        public int TimestampEnd { get; set; }
        [Column("duration")]
        public int Duration { get; set; }
        [Column("community_contributed")]
        public bool CommunityContributed { get; set; }
        [Column("ad")]
        public bool Ad { get; set; }
        [Column("created_at")]
        public DateTime CreatedAt { get; set; }
        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; }
        [Column("id_episodes")]
        public int EpisodeId { get; set; }
        [Column("id_user")]
        public int UserId { get; set; }
    }

    public class TopicTest
    {
        public TopicTest() { }

        public TopicTest(Topic topic)
        {
            Id = topic.Id;
            Name = topic.Name;
            TimestampStart = topic.TimestampStart;
            TimestampEnd = topic.TimestampEnd;
            Duration = topic.Duration;
            CommunityContributed = topic.CommunityContributed;
            Ad = topic.Ad;
            CreatedAt = topic.CreatedAt;
            UpdatedAt = topic.UpdatedAt;
            EpisodeId = topic.EpisodeId;
            UserId = topic.UserId;
        }
        
        public Int64 Id { get; set; }
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
        public TopicExtended() { }

        public TopicExtended(Topic topic) : base(topic) { }

        public TopicExtended(Topic topic, List<Subtopic> subtopics) : base(topic)
        {
            Subtopic = subtopics;
        }

        public List<Subtopic> Subtopic { get; set; } = new List<Subtopic>();
    }
}
