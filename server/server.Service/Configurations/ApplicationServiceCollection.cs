using server.Domain.Entities;
using server.Service.Common.IServices;
using server.Service.Common.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace server.Service.Configurations
{
    public static class ApplicationServiceCollection
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            #region Common services
            // JWT
            services.AddScoped<IJwtService, JwtService>();

            // User Management Service.
            services.AddScoped<UserManager<User>>();
            services.AddScoped<SignInManager<User>, SignInManager<User>>();
            services.AddScoped<IUserService, UserService>();

            #endregion 
            services.AddHttpContextAccessor();
            #region Business services

            #endregion
            return services;
        }
    }
}
