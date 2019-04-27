using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Mutube.Database.Models.Identity;

namespace Mutube.Database
{
    public class MutubeDbContext : IdentityDbContext<User, Role, long,
        UserClaim, UserRole, UserLogin, RoleClaim, UserToken>
    {
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
