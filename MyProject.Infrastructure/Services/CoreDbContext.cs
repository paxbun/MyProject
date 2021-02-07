using MyProject.Core.Services;
using MyProject.Models;
using Microsoft.EntityFrameworkCore;
using MySql.Data.EntityFrameworkCore.Extensions;

namespace MyProject.Infrastructure.Services
{
    public class CoreDbContext : DbContext, ICoreDbContext
    {
        public CoreDbContext(DbContextOptions<CoreDbContext> options)
            : base(options)
        { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(User).Assembly);

            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                modelBuilder.Entity(entityType.Name).ForMySQLHasCollation("utf8mb4_general_ci");
            }
        }
    }
}
