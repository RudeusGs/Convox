using server.Service.Models;
using server.Service.Models.Authenticate;

namespace server.Service.Interfaces
{
    public interface IAuthenticateService
    {
        /// <summary>
        /// Đăng nhập user.
        /// </summary>
        Task<ApiResult> Login(LoginModel loginModel);

        /// <summary>
        /// Đăng ký user mới.
        /// </summary>
        Task<ApiResult> Register(RegisterModel registerModel);

        /// <summary>
        /// Lấy role của user.
        /// </summary>
        Task<ApiResult> GetRoleUser(int id);
    }
}