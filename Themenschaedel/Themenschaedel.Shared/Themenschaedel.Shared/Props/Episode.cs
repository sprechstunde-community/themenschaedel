using System;
using System.Collections.Generic;
using System.Text;

namespace Themenschaedel.Shared.Props
{
    public enum EpisodeClaimStatus { not_claimed, claimed, done }

    public class MinimalEpisode
    {
        public int id { get; set; }
        public string guid { get; set; }
        public int episode_number { get; set; }
        public string title { get; set; }
        public string subtitle { get; set; }
        public string description { get; set; }
        public string image { get; set; }
        public int duration { get; set; }
        public DateTimeOffset published_at { get; set; }
        public DateTimeOffset created_at { get; set; }
        public DateTimeOffset updated_at { get; set; }
    }
    
    public class AlternateMinimalEpisode
    {
        public int id { get; set; }
        public string guid { get; set; }
        public int episode_number { get; set; }
        public string title { get; set; }
        public string subtitle { get; set; }
        public string description { get; set; }
        public string image { get; set; }
        public int duration { get; set; }
        public string media_file { get; set; }
        public string type { get; set; }
        public int Explicit { get; set; }
        public DateTimeOffset published_at { get; set; }
        public DateTimeOffset created_at { get; set; }
        public DateTimeOffset updated_at { get; set; }
    }
    
    public class Episode : MinimalEpisode
    {
        public List<Hosts> hosts { get; set; } = new List<Hosts>();
        public List<Topic> topics { get; set; } = new List<Topic>();
        public bool claimed { get; set; }
        public int upvotes { get; set; }
        public int downvotes { get; set; }
        public int flags { get; set; }

        //Data not from the API
        public string ThumbnailCSS { get; set; }
        public string VideoCSS { get; set; }
        public double AnimationDelay { get; set; }
        public string AnimationDelayCSS => $"--delay: {AnimationDelay.ToString()}ms";

        public EpisodeClaimStatus ClaimStatus => GetClaimStatus();

        private EpisodeClaimStatus GetClaimStatus()
        {
            if (claimed)
            {
                return EpisodeClaimStatus.claimed;
            }
            else if (topics.Count > 0)
            {
                return EpisodeClaimStatus.done;
            }
            else
            {
                return EpisodeClaimStatus.not_claimed;
            }
        }
    }
}
