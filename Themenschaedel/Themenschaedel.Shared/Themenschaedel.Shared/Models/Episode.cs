using System;
using System.Collections.Generic;
using System.Text;

namespace Themenschaedel.Shared.Models
{
    public enum EpisodeClaimStatus
    {
        not_claimed,
        claimed,
        done
    }

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
        public bool Claimed { get; set; }
        public int Flags { get; set; }
        public int Upvotes { get; set; }
        public int Downvotes { get; set; }
        public Int64 topic_count { get; set; }
    }

    public class EpisodeExtendedExtra : EpisodeExtended
    {
        public List<TopicExtended> Topic { get; set; } = new List<TopicExtended>();
        public List<Person> Person { get; set; } = new List<Person>();
    }

    public class EpisodeClient : EpisodeExtended
    {
        //Data not from the API
        public string ThumbnailCSS { get; set; }
        public string VideoCSS { get; set; }
        public double AnimationDelay { get; set; }
        public string AnimationDelayCSS => $"--delay: {AnimationDelay.ToString()}ms";

        public EpisodeClaimStatus ClaimStatus => GetClaimStatus();

        public EpisodeClaimStatus GetClaimStatus()
        {
            if (Claimed)
            {
                return EpisodeClaimStatus.claimed;
            }
            else if (topic_count > 0)
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
