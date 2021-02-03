using MyProject.Core.Services;
using MyProject.Models;
using Microsoft.EntityFrameworkCore;
using MySql.Data.EntityFrameworkCore.Extensions;

namespace MyProject.Infrastructure.Services
{
    public class CoreDbContext : DbContext, ICoreDbContext
    {
        public virtual DbSet<User> Users { get; set; }

        public CoreDbContext(DbContextOptions<CoreDbContext> options)
            : base(options)
        { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new UserEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new GeneralUserEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new AdministratorUserEntityTypeConfiguration());

            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                modelBuilder.Entity(entityType.Name).ForMySQLHasCollation("utf8_general_ci");
            }
        }
    }
}
