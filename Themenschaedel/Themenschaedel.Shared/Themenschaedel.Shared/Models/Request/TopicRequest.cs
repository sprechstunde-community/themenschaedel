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

        public TopicPostRequest(TopicExtended topic)
        {
            Name = topic.Name;
            TimestampStart = topic.TimestampStart;
            TimestampEnd = topic.TimestampEnd;
            Ad = topic.Ad;
            CommunityContributed = topic.CommunityContributed;
            foreach (Subtopic item in topic.Subtopic)
            {
                Subtopics.Add(new SubtopicPostRequest()
                {
                    Name = item.Name
                });
            }
        }

        public TopicPostRequest(TopicPostRequestClient topic)
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

    public class TopicPostRequestClient : TopicPostRequest
    {
        public TopicPostRequestClient(){}

        public TopicPostRequestClient(TopicPostRequest topic) : base(topic) { }
        public TopicPostRequestClient(TopicExtended topic) : base(topic) { }

        public string GetStartTimestamp()
        {
            TimeSpan t = TimeSpan.FromSeconds(TimestampStart);

            string hour = t.Hours.ToString("D2");
            string minutes = t.Minutes.ToString("D2");
            string seconds = t.Seconds.ToString("D2");

            if (hour != "00")
            {
                return $"{hour}:{minutes}:{seconds}";
            }
            else if (minutes != "00")
            {
                return $"{minutes}:{seconds}";
            }
            else
            {
                return $"{seconds}";
            }
        }

        public string GetFullStartTimestamp()
        {
            TimeSpan t = TimeSpan.FromSeconds(TimestampStart);

            string hour = t.Hours.ToString("D2");
            string minutes = t.Minutes.ToString("D2");
            string seconds = t.Seconds.ToString("D2");

            return $"{hour}:{minutes}:{seconds}";
        }

        public TimeSpan GetStartTimespan() => TimeSpan.FromSeconds(TimestampStart);

        public bool SetStartFromString(string time)
        {
            TimestampStart = 0;
            string[] times = time.Split(':');
            if (times.Length > 3 || times.Length < 1) return false;
            try
            {
                if (times.Length == 3)
                {
                    TimestampStart += Int32.Parse(times[0]) * 3600;
                    TimestampStart += Int32.Parse(times[1]) * 60;
                    TimestampStart += Int32.Parse(times[2]);
                }
                else if (times.Length == 2)
                {
                    TimestampStart += Int32.Parse(times[1]) * 60;
                    TimestampStart += Int32.Parse(times[2]);
                }
                else
                {
                    TimestampStart += Int32.Parse(times[0]);
                }
            }
            catch (Exception e)
            {
                return false;
            }
            return true;
        }

        public string GetEndTimestamp(int endOfNextTopic)
        {
            int timestamp = 0;
            if (TimestampEnd == null || TimestampEnd == 0)
            {
                timestamp = endOfNextTopic - 1;
            }
            else
            {
                timestamp = TimestampEnd;
            }

            TimeSpan t = TimeSpan.FromSeconds(timestamp);

            string hour = t.Hours.ToString("D2");
            string minutes = t.Minutes.ToString("D2");
            string seconds = t.Seconds.ToString("D2");

            if (hour != "00")
            {
                return $"{hour}:{minutes}:{seconds}";
            }
            else if (minutes != "00")
            {
                return $"{minutes}:{seconds}";
            }
            else
            {
                return $"{seconds}";
            }
        }

        public string GetFullEndTimestamp()
        {
            TimeSpan t = TimeSpan.FromSeconds(TimestampEnd);

            string hour = t.Hours.ToString("D2");
            string minutes = t.Minutes.ToString("D2");
            string seconds = t.Seconds.ToString("D2");

            return $"{hour}:{minutes}:{seconds}";
        }

        public TimeSpan GetEndTimespan() => TimeSpan.FromSeconds(TimestampEnd);

        public bool SetEndFromString(string time)
        {
            TimestampEnd = 0;
            string[] times = time.Split(':');
            if (times.Length > 3 || times.Length < 1) return false;
            try
            {
                if (times.Length == 3)
                {
                    TimestampEnd += Int32.Parse(times[0]) * 3600;
                    TimestampEnd += Int32.Parse(times[1]) * 60;
                    TimestampEnd += Int32.Parse(times[2]);
                }
                else if (times.Length == 2)
                {
                    TimestampEnd += Int32.Parse(times[1]) * 60;
                    TimestampEnd += Int32.Parse(times[2]);
                }
                else
                {
                    TimestampEnd += Int32.Parse(times[0]);
                }
            }
            catch (Exception e)
            {
                return false;
            }
            return true;
        }
    }

    public class SubtopicPostRequest
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }
    }

    public class TopicRequest
    {
        public TopicRequest() {}

        public TopicRequest(List<PeopleInEpisode> people, List<TopicPostRequest> topics)
        {
            People = people;
            Topics = topics;
        }

        [JsonPropertyName("people")]
        public List<PeopleInEpisode> People { get; set; } = new List<PeopleInEpisode>();

        [JsonPropertyName("topics")]
        public List<TopicPostRequest> Topics { get; set; } = new List<TopicPostRequest>();
    }

    public class PeopleInEpisode
    {
        [JsonPropertyName("person_id")]
        public int PersonId { get; set; }
    }

    public class ProcessedTopicPostRequest : TopicPostRequest
    {
        public ProcessedTopicPostRequest() { }
        public ProcessedTopicPostRequest(TopicPostRequest topic) : base(topic) { }

        public int Duration => TimestampEnd - TimestampStart;
    }

    public class TopicReasingPostRequest
    {
        [JsonPropertyName("user_id")]
        public int UserId { get; set; }
        [JsonPropertyName("episode_id")]
        public int EpisodeId { get; set; }
    }
}
