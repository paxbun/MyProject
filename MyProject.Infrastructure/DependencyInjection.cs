using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MyProject.Core.Services;
using MyProject.Infrastructure.Services;
using MyProject.Models;

namespace MyProject.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services, IConfiguration configuration, IHostEnvironment environment)
        {
            services.AddDbContext<CoreDbContext>(options =>
            {
                options.UseMySQL(configuration["DbConnectionString"]).EnableSensitiveDataLogging();
            });
            services.AddScoped<ICoreDbContext, CoreDbContext>(provider =>
            {
                var dbContext = provider.GetService<CoreDbContext>();
                var configuration = provider.GetService<IConfiguration>();
                dbContext.PopulateSeedData(new[] {
                    new AdministratorUser
                    {
                        Username = configuration["AdminUsername"],
                        Password = configuration["AdminPassword"],
                        RealName = configuration["AdminRealName"],
                        Type = UserType.Administrator
                    }
                });
                return dbContext;
            });
            services.AddSingleton<IUserIdentityService, UserIdentityService>();
            if (environment.IsDevelopment())
            {
                services.AddSingleton<ICoreLogger, ConsoleLogger>();
            }
            else
            {
                services.AddSingleton<ICoreLogger, AzureTableStorageLogger>();
            }
            return services;
        }
    }
}
