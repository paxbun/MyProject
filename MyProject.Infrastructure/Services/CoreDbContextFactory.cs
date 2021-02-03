using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.Reflection;

namespace MyProject.Infrastructure.Services
{
    public class CoreDbContextFactory : IDesignTimeDbContextFactory<CoreDbContext>
    {
        public CoreDbContextFactory()
        {
        }

        public CoreDbContext CreateDbContext(string[] args)
        {
            var config = new ConfigurationBuilder().AddUserSecrets(Assembly.GetExecutingAssembly()).Build();
            var connectionString = config["DbConnectionString"];
            var options = new DbContextOptionsBuilder<CoreDbContext>();
            options.UseMySQL(connectionString);
            return new CoreDbContext(options.Options);
        }
    }
}
