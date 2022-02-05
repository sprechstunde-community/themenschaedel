using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Themenschaedel.Shared.Models
{
    public class Subtopic
    {
        [Column("id")]
        public int Id { get; set; }
        [Column("name")]
        public string Name { get; set; }
        [Column("created_at")]
        public DateTime CreatedAt { get; set; }
        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; }
        [Column("id_topic")]
        public int TopicId { get; set; }
        [Column("id_user")]
        public int UserId { get; set; }
    }
}
