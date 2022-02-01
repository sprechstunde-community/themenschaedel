using System;
using System.Collections.Generic;
using System.Text;

namespace Themenschaedel.Shared.Models
{
    public class Episode
    {
        public int Id { get; set; }
        public string UUID { get; set; }
        public string Title { get; set; }
        public int EpisodeNumber { get; set; }
        public string Subtitle { get; set; }
        public string Description { get; set; }
        public string MediaFile { get; set; }
        public string SpotifyFile { get; set; }
        public int Duration { get; set; }
        public string Type { get; set; }
        public string Image { get; set; }
        public bool Explicit { get; set; }
        public bool Verified { get; set; }
        public DateTime PublishedAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class EpisodeExtended : Episode
    {
        public List<TopicExtended> Topic { get; set; } = new List<TopicExtended>();
        public List<Person> Person { get; set; } = new List<Person>();
    }
}
