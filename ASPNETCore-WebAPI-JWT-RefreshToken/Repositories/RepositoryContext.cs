using ASPNETCore_WebAPI_JWT_RefreshToken.Models;
using ASPNETCore_WebAPI_JWT_RefreshToken.Repositories.Seed;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ASPNETCore_WebAPI_JWT_RefreshToken.Repositories
{
    public class RepositoryContext : IdentityDbContext<User>
    {
        public RepositoryContext(DbContextOptions options) : base(options)
        {
            
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.ApplyConfiguration(new UserRoleSeed());
        }
    }
}
