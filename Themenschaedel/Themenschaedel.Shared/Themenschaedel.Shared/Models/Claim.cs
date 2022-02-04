using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Themenschaedel.Shared.Models
{
    public class Claim
    {
        [Column("id")]
        public int Id { get; set; }
        [Column("claimed_at")]
        public DateTime ClaimedAt { get; set; }
        [Column("valid_until")]
        public DateTime ValidUntil { get; set; }
        [Column("created_at")]
        public DateTime CreatedAt { get; set; }
        [Column("update_at")]
        public DateTime UpdatedAt { get; set; }
        [Column("id_users")]
        public int UserId { get; set; }
        [Column("id_episodes")]
        public int EpisodeId { get; set; }
    }
}
