using System;

namespace Themenschaedel.Shared.Props
{
    public enum SearchType {Episode, Topic, Subtopic}
    public class Search
    {
        public string UniqueGuid { get; set; }
        public SearchType Type { get; set; }
        public Episode Episode { get; set; }
        public Topic Topic {get; set; }
        public Subtopics Subtopic {get; set; }

        public Search(Episode episode)
        {
            UniqueGuid = Guid.NewGuid().ToString();
            Type = SearchType.Episode;
            this.Episode = episode;
        }
        public Search(Episode episode, Topic topic)
        {
            UniqueGuid = Guid.NewGuid().ToString();
            Type = SearchType.Topic;
            this.Episode = episode;
            this.Topic = topic;
        }
        public Search(Episode episode, Topic topic, Subtopics subtopic)
        {
            UniqueGuid = Guid.NewGuid().ToString();
            Type = SearchType.Subtopic;
            this.Episode = episode;
            this.Topic = topic;
            this.Subtopic = subtopic;
        }
    }
}