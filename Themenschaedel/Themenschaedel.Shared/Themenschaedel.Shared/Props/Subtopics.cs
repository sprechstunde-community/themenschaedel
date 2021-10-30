namespace Themenschaedel.Shared.Props
{
    public class Subtopics
    {
        public int id { get; set; }
        public string name { get; set; }
        public int topic_id { get; set; }
        public int user_id { get; set; }
        public string created_at { get; set; }
        public string updated_at { get; set; }
    }

    public class SubtopicResponse
    {
        public SubtopicResponseWorkaround data { get; set; }
    }
    
    public class SubtopicResponseWorkaround : Subtopics
    {
        public User user { get; set; }
        public MinimalTopic topic { get; set; }
    }
}