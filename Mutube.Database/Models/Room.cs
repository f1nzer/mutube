using System;

namespace Mutube.Database.Models
{
    public class Room
    {
        public Guid Id { get; set; }
        public string DisplayName { get; set; }
        public bool IsPrivate { get; set; }
        public virtual User Owner { get; set; }
    }
}