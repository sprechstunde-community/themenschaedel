using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using System.Text.Json.Serialization;

namespace Themenschaedel.Shared.Models
{
    public class Subtopic
    {
        [JsonPropertyName("id")]
        [Column("id")]
        public int Id { get; set; }
        [JsonPropertyName("name")]
        [Column("name")]
        public string Name { get; set; }
        [JsonPropertyName("created_at")]
        [Column("created_at")]
        public DateTime CreatedAt { get; set; }
        [JsonPropertyName("updated_at")]
        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; }
        [JsonPropertyName("id_topic")]
        [Column("id_topic")]
        public int TopicId { get; set; }
        [JsonPropertyName("id_user")]
        [Column("id_user")]
        public int UserId { get; set; }
    }
}
