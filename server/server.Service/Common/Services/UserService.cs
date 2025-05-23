using server.Service.Common.IServices;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace server.Service.Common.Services
{
    public class UserService : IUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private string _userName;
        private int _userId;

        public UserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            _userName = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            int.TryParse(_httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Name), out _userId);
        }

        public string UserName => _userName ?? _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
        public string RoleName => _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Role);
        public int UserId => _userId;

        public void DeserializeUserId(string userSerialized)
        {
            if (userSerialized == null)
            {
                return;
            }
            var userByte = Convert.FromBase64String(userSerialized);
            using var mStream = new MemoryStream(userByte);
            using var bReader = new BinaryReader(mStream);
            var claims = new ClaimsPrincipal(bReader);
            _userName = claims.FindFirstValue(ClaimTypes.NameIdentifier);
        }   
    }
}
