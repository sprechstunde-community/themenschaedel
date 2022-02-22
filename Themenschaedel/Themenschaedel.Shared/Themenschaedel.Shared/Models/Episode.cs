using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
using Themenschaedel.Shared.Models.Response;

namespace Themenschaedel.Shared.Models
{
    public enum EpisodeClaimStatus
    {
        not_claimed,
        claimed,
        done,
        unverified
    }

    public class Episode
    {
        public Episode() { }

        public Episode(Episode episode)
        {
            Id = episode.Id;
            UUID = episode.UUID;
            Title = episode.Title;
            EpisodeNumber = episode.EpisodeNumber;
            Subtitle = episode.Subtitle;
            Description = episode.Description;
            MediaFile = episode.MediaFile;
            SpotifyFile = episode.SpotifyFile;
            Duration = episode.Duration;
            Type = episode.Type;
            Image = episode.Image;
            Explicit = episode.Explicit;
            Verified = episode.Verified;
            PublishedAt = episode.PublishedAt;
            CreatedAt = episode.CreatedAt;
            UpdatedAt = episode.UpdatedAt;
        }

        [JsonPropertyName("id")]
        public int Id { get; set; }
        [JsonPropertyName("uuid")]
        public string UUID { get; set; }
        [JsonPropertyName("title")]
        public string Title { get; set; }
        [JsonPropertyName("episodeNumber")]
        public int EpisodeNumber { get; set; }
        [JsonPropertyName("subtitle")]
        public string Subtitle { get; set; }
        [JsonPropertyName("description")]
        public string Description { get; set; }
        [JsonPropertyName("media_file")]
        public string MediaFile { get; set; }
        [JsonPropertyName("spotify_file")]
        public string SpotifyFile { get; set; }
        [JsonPropertyName("duration")]
        public int Duration { get; set; }
        [JsonPropertyName("type")]
        public string Type { get; set; }
        [JsonPropertyName("image")]
        public string Image { get; set; }
        [JsonPropertyName("explicit")]
        public bool Explicit { get; set; }
        [JsonPropertyName("verified")]
        public bool Verified { get; set; }
        [JsonPropertyName("published_at")]
        public DateTime PublishedAt { get; set; }
        [JsonPropertyName("created_at")]
        public DateTime CreatedAt { get; set; }
        [JsonPropertyName("updated_at")]
        public DateTime UpdatedAt { get; set; }
    }

    public class EpisodeExtended : Episode
    {
        public EpisodeExtended() { }

        public EpisodeExtended(EpisodeExtended episode) : base(episode)
        {
            Claimed = episode.Claimed;
            Flags = episode.Flags;
            Upvotes = episode.Upvotes;
            Downvotes = episode.Downvotes;
            topic_count = episode.topic_count;
            Upvoted = episode.Upvoted;
        }

        [JsonPropertyName("claimed")]
        public bool Claimed { get; set; }
        [JsonPropertyName("flags")]
        public int Flags { get; set; }
        [JsonPropertyName("upvotes")]
        public int Upvotes { get; set; }
        [JsonPropertyName("downvotes")]
        public int Downvotes { get; set; }
        [JsonPropertyName("topic_count")]
        public Int64 topic_count { get; set; }

        [JsonPropertyName("upvoted")]
        public bool? Upvoted { get; set; } = null;
    }

    public class EpisodeExtendedExtra : EpisodeExtended
    {
        public EpisodeExtendedExtra() { }

        public EpisodeExtendedExtra(EpisodeExtendedExtra episodes) : base(episodes)
        {
            Topic = episodes.Topic;
            Person = episodes.Person;
        }

        [JsonPropertyName("topic")]
        public List<TopicExtended> Topic { get; set; } = new List<TopicExtended>();
        [JsonPropertyName("person")]
        public List<Person> Person { get; set; } = new List<Person>();
    }

    public class EpisodeClient : EpisodeExtended
    {
        public EpisodeClient() { }

        public EpisodeClient(EpisodeExtended episode) : base(episode) { }
        public EpisodeClient(EpisodeClientExtra episode) : base(episode) { }

        //Data not from the API
        public string ThumbnailCSS { get; set; }
        public string VideoCSS { get; set; }
        public double AnimationDelay { get; set; }
        public string AnimationDelayCSS => $"--delay: {AnimationDelay.ToString()}ms";

        public EpisodeClaimStatus ClaimStatus => EpisodeMisc.GetClaimStatus(this);
    }


    public class EpisodeClientExtra : EpisodeExtendedExtra
    {
        public EpisodeClientExtra() { }

        public EpisodeClientExtra(EpisodeExtendedExtra EpisodeExtendedExtra) : base(EpisodeExtendedExtra) { }
        public EpisodeClientExtra(EpisodeClientExtra EpisodeExtendedExtra) : base(EpisodeExtendedExtra) { }

        //Data not from the API
        public string ThumbnailCSS { get; set; }
        public string VideoCSS { get; set; }
        public double AnimationDelay { get; set; }
        public string AnimationDelayCSS => $"--delay: {AnimationDelay.ToString()}ms";

        public EpisodeClaimStatus ClaimStatus => EpisodeMisc.GetClaimStatus(this);

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

    public class EpisodeWithValidUntilClaim : Episode
    {
        public DateTime valid_until { get; set; }
    }

    public static class EpisodeMisc
    {
        public static EpisodeClaimStatus GetClaimStatus(EpisodeClient episode)
        {
            if (episode.Claimed)
            {
                return EpisodeClaimStatus.claimed;
            }
            else if (episode.topic_count > 0 && episode.Verified)
            {
                return EpisodeClaimStatus.done;
            }
            else if (episode.topic_count > 0 && !episode.Verified)
            {
                return EpisodeClaimStatus.unverified;
            }
            else
            {
                return EpisodeClaimStatus.not_claimed;
            }
        }

        public static EpisodeClaimStatus GetClaimStatus(EpisodeClientExtra episode)
        {
            if (episode.Claimed)
            {
                return EpisodeClaimStatus.claimed;
            }
            else if (episode.topic_count > 0 && episode.Verified)
            {
                return EpisodeClaimStatus.done;
            }
            else if (episode.topic_count > 0 && !episode.Verified)
            {
                return EpisodeClaimStatus.unverified;
            }
            else
            {
                return EpisodeClaimStatus.not_claimed;
            }
        }
    }
}
