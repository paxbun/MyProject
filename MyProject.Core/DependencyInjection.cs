using MediatR;
using Microsoft.Extensions.DependencyInjection;
using MyProject.Core.Behaviors;
using MyProject.Core.Services;
using System.Reflection;

namespace MyProject.Core
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddCore(this IServiceCollection services)
        {
            services.AddMediatR(Assembly.GetExecutingAssembly());
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(AuthorizationBehavior<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LogBehavior<,>));
            services.AddCoreLoggerFormatters();
            return services;
        }
    }
}
