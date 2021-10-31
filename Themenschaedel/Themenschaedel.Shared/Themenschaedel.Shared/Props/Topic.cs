using System;
using System.Collections.Generic;
using System.Text;

namespace Themenschaedel.Shared.Props
{
    public class MinimalTopic
    {
        public int id { get; set; }
        public int episode_id { get; set; }
        public string user_id { get; set; }
        public string name { get; set; }
        public int start { get; set; }
        public int end { get; set; }
        public bool ad { get; set; }
        public bool community_contribution { get; set; }
        public string created_at { get; set; }
        public string updated_at { get; set; }
    }

    public class Topic : MinimalTopic
    {
        public List<Subtopics> subtopics { get; set; } = new List<Subtopics>();
        public User user { get; set; }

        public string GetStartTimestamp()
        {
            TimeSpan t = TimeSpan.FromSeconds(start);

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
            TimeSpan t = TimeSpan.FromSeconds(start);

            string hour = t.Hours.ToString("D2");
            string minutes = t.Minutes.ToString("D2");
            string seconds = t.Seconds.ToString("D2");

            return $"{hour}:{minutes}:{seconds}";
        }

        public TimeSpan GetStartTimespan() => TimeSpan.FromSeconds(start);

        public bool SetStartFromString(string time)
        {
            start = 0;
            string[] times = time.Split(':');
            if (times.Length > 3 || times.Length < 1) return false;
            try
            {
                if (times.Length == 3)
                {
                    start += Int32.Parse(times[0]) * 3600;
                    start += Int32.Parse(times[1]) * 60;
                    start += Int32.Parse(times[2]);
                }
                else if (times.Length == 2)
                {
                    start += Int32.Parse(times[1]) * 60;
                    start += Int32.Parse(times[2]);
                }
                else
                {
                    start += Int32.Parse(times[0]);
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
            if (end == null || end == 0)
            {
                timestamp = endOfNextTopic - 1;
            }
            else
            {
                timestamp = end;
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
            TimeSpan t = TimeSpan.FromSeconds(end);

            string hour = t.Hours.ToString("D2");
            string minutes = t.Minutes.ToString("D2");
            string seconds = t.Seconds.ToString("D2");

            return $"{hour}:{minutes}:{seconds}";
        }

        public TimeSpan GetEndTimespan() => TimeSpan.FromSeconds(end);

        public bool SetEndFromString(string time)
        {
            end = 0;
            string[] times = time.Split(':');
            if (times.Length > 3 || times.Length < 1) return false;
            try
            {
                if (times.Length == 3)
                {
                    end += Int32.Parse(times[0]) * 3600;
                    end += Int32.Parse(times[1]) * 60;
                    end += Int32.Parse(times[2]);
                }
                else if (times.Length == 2)
                {
                    end += Int32.Parse(times[1]) * 60;
                    end += Int32.Parse(times[2]);
                }
                else
                {
                    end += Int32.Parse(times[0]);
                }
            }
            catch (Exception e)
            {
                return false;
            }
            return true;
        }
    }

    public class TopicResponse
    {
        public TopicResponseWorkaround data { get; set; }
    }

    public class TopicResponseWorkaround : MinimalTopic
    {
        public User user { get; set; }
        public MinimalEpisode episode { get; set; }
        public List<Subtopics> subtopics { get; set; }
    }

    public class GetTopicWorkaround
    {
        public Topic data { get; set; }
    }

    public class MinimalTopicResponse
    {
        public Topic data { get; set; }
    }
    
    public class ListTopicResponse
    {
        public List<Topic> data { get; set; }
    }
}