using ASPNETCore_WebAPI_JWT_RefreshToken.Models;
using ASPNETCore_WebAPI_JWT_RefreshToken.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestProject
{
    public class RepositoryContextTests
    {
        [Fact]
        public void OnModelCreating_AppliesUserRoleSeedConfiguration()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<RepositoryContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            // Create a mock ModelBuilder to capture the configurations applied
            var modelBuilderMock = new Mock<ModelBuilder>(new ConventionSet());
            modelBuilderMock.Setup(m => m.ApplyConfiguration(It.IsAny<IEntityTypeConfiguration<IdentityRole>>()));

            using (var context = new TestRepositoryContext(options))
            {
                // Act
                context.ExposeOnModelCreating(modelBuilderMock.Object);

                // Assert
                modelBuilderMock.Verify(m => m.ApplyConfiguration(It.IsAny<IEntityTypeConfiguration<IdentityRole>>()), Times.Once);
            }
        }
    }
}
