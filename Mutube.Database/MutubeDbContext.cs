using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Mutube.Database.Models;

namespace Mutube.Database
{
    public class MutubeDbContext : IdentityDbContext<User, Role, long,
        UserClaim, UserRole, UserLogin, RoleClaim, UserToken>
    {
        public DbSet<Playlist> Playlists { get; set; }

        public MutubeDbContext()
        {
        }

        public MutubeDbContext(DbContextOptions<MutubeDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
