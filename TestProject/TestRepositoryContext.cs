using ASPNETCore_WebAPI_JWT_RefreshToken.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestProject
{
    public class TestRepositoryContext : RepositoryContext
    {
        public TestRepositoryContext(DbContextOptions options) : base(options)
        {
        }

        public new void ExposeOnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
    }
}
