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
            services.AddScoped<UserManager<Account>>();
            services.AddScoped<SignInManager<Account>, SignInManager<Account>>();
            services.AddScoped<IUserService, UserService>();

            #endregion 
            services.AddHttpContextAccessor();
            #region Business services

            #endregion
            return services;
        }
    }
}
