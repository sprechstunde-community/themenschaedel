using System.Collections.Generic;

namespace Themenschaedel.Shared.Props
{
    public class GetTopicWorkaroundWrapper
    {
        public List<Topic> data { get; set; }
        public TopicLinks links { get; set; }
        public TopicMeta meta { get; set; }
    }

    public class TopicLinks
    {
        public string first { get; set; }
        public string last { get; set; }
        public string prev { get; set; }
        public string next { get; set; }
    }

    public class TopicMeta
    {
        public int current_page { get; set; }
        public string from { get; set; }
        public int last_page { get; set; }
        public List<TopicUselessLinks> links { get; set; }
        public string path { get; set; }
        public int per_page { get; set; }
        public string to { get; set; }
        public int total { get; set; }
    }

    public class TopicUselessLinks
    {
        public string url { get; set; }
        public string label { get; set; }
        public bool active { get; set; }
    }
}