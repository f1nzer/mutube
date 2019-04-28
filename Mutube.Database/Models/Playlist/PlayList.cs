using System;

namespace Mutube.Database.Models
{
    public class Playlist
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public User CreatedUser { get; set; }
        public string Description { get; set; }
    }
}
