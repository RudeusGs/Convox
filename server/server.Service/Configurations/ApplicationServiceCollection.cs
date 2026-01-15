using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using server.Domain.Entities;
using server.Service.Common.IServices;
using server.Service.Common.Services;
using server.Service.Interfaces;
using server.Service.Interfaces.Authentication;
using server.Service.Services;
using server.Service.Services.Authentication;
using server.Service.Services.Badges;
using server.Service.Services.BreakoutRooms;
using server.Service.Services.Chats;
using server.Service.Services.Quizzes;
using server.Service.Services.Rooms;

namespace server.Service.Configurations
{
    public static class ApplicationServiceCollection
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            #region Common services

            // User Management Service
            services.AddScoped<UserManager<User>>();
            services.AddScoped<SignInManager<User>, SignInManager<User>>();
            services.AddScoped<IUserService, UserService>();

            #endregion

            services.AddHttpContextAccessor();

            #region Authentication services
            services.AddScoped<IJwtTokenService, JwtTokenService>();
            services.AddScoped<IRoleManagementService, RoleManagementService>();
            services.AddScoped<IAuthenticateService, AuthenticateService>();

            #endregion

            #region Business services
            services.AddScoped<IRoomService, RoomService>();
            services.AddScoped<IBadgeService, BadgeService>();
            services.AddScoped<IUserBadgeService, UserBadgeService>();
            services.AddScoped<IUserRoomService, UserRoomService>();
            services.AddScoped<IQuizService, QuizService>();
            services.AddScoped<IRoomChatService, RoomChatService>();
            services.AddScoped<IBreakroomChatService, BreakroomChatService>();
            services.AddScoped<IP2PChatService, P2PChatService>();
            services.AddScoped<IChatUploadService, ChatUploadService>();
            services.AddScoped<IBreakoutRoomService, BreakoutRoomService>();
            #endregion

            return services;
        }
    }
}