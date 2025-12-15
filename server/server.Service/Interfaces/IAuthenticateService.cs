using server.Service.Models;
using server.Service.Models.Authenticate;

namespace server.Service.Interfaces
{
    public interface IAuthenticateService
    {
        Task<ApiResult> Login(LoginModel loginModel);
        Task<ApiResult> Register(RegisterModel registerModel);
        Task<ApiResult> GetRoleUser(int id);
    }
}
