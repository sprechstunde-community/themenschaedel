using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using System.Text.Json.Serialization;

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

        [JsonPropertyName("id")]
        [Column("id")]
        public Int64 Id { get; set; }
        [JsonPropertyName("name")]
        [Column("name")]
        public string Name { get; set; }
        [JsonPropertyName("timestamp_start")]
        [Column("timestamp_start")]
        public int TimestampStart { get; set; }
        [JsonPropertyName("timestamp_end")]
        [Column("timestamp_end")]
        public int TimestampEnd { get; set; }
        [JsonPropertyName("duration")]
        [Column("duration")]
        public int Duration { get; set; }
        [JsonPropertyName("community_contributed")]
        [Column("community_contributed")]
        public bool CommunityContributed { get; set; }
        [JsonPropertyName("ad")]
        [Column("ad")]
        public bool Ad { get; set; }
        [JsonPropertyName("created_at")]
        [Column("created_at")]
        public DateTime CreatedAt { get; set; }
        [JsonPropertyName("updated_at")]
        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; }
        [JsonPropertyName("id_episodes")]
        [Column("id_episodes")]
        public int EpisodeId { get; set; }
        [JsonPropertyName("id_user")]
        [Column("id_user")]
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

        [JsonPropertyName("subtopic")]
        public List<Subtopic> Subtopic { get; set; } = new List<Subtopic>();
    }
}
