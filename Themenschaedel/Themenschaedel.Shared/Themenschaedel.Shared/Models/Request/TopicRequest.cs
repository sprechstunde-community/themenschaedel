using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Themenschaedel.Shared.Models.Request
{
    public class TopicPostRequest
    {
        public TopicPostRequest() { }

        public TopicPostRequest(TopicPostRequest topic)
        {
            Name = topic.Name;
            TimestampStart = topic.TimestampStart;
            TimestampEnd = topic.TimestampEnd;
            Ad = topic.Ad;
            CommunityContributed = topic.CommunityContributed;
            Subtopics = topic.Subtopics;
        }

        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("start")]
        public int TimestampStart { get; set; }
        [JsonPropertyName("end")]
        public int TimestampEnd { get; set; }
        [JsonPropertyName("ad")]
        public bool Ad { get; set; }
        [JsonPropertyName("community_contribution")]
        public bool CommunityContributed { get; set; }
        [JsonPropertyName("subtopics")]
        public List<SubtopicPostRequest> Subtopics { get; set; } = new List<SubtopicPostRequest>();
    }

    public class SubtopicPostRequest
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }
    }

    public class ProcessedTopicPostRequest : TopicPostRequest
    {
        public ProcessedTopicPostRequest(){}
        public ProcessedTopicPostRequest(TopicPostRequest topic) : base(topic) { }

        public int Duration => TimestampEnd - TimestampStart;
    }
}
