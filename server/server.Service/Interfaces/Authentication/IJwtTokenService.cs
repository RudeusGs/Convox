using server.Domain.Entities;
using server.Service.Models.Authenticate;

namespace server.Service.Interfaces.Authentication
{
    /// <summary>
    /// IJwtTokenService: Quản lý JWT token generation.
    /// </summary>
    public interface IJwtTokenService
    {
        /// <summary>
        /// Tạo JWT token cho user.
        /// </summary>
        UserToken GenerateToken(User user, IEnumerable<string> roles);
    }
}