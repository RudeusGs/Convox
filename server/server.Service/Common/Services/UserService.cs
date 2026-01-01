using Microsoft.AspNetCore.Http;
using server.Service.Common.IServices;
using System.Security.Claims;

namespace server.Service.Common.Services
{
    public class UserService : IUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        private ClaimsPrincipal? User => _httpContextAccessor.HttpContext?.User;

        public string RoleName => User?.FindFirstValue(ClaimTypes.Role);

        public string UserName => User?.FindFirstValue(ClaimTypes.Name);

        public int UserId =>
            int.TryParse(User?.FindFirstValue(ClaimTypes.NameIdentifier), out var id) ? id : 0;

        public void DeserializeUserId(string userSerialized) { }
    }
}
