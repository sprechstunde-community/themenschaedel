using System;
using System.Collections.Generic;
using System.Text;

namespace Themenschaedel.Shared.Models
{
    public class Person
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool Host { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
