using Microsoft.Extensions.DependencyInjection;

namespace server.Infrastructure.Extensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddRealtime(this IServiceCollection services)
        {
            services.AddSignalR();
            return services;
        }
    }
}
