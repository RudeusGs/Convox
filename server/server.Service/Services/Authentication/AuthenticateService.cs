using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using server.Domain.Entities;
using server.Service.Interfaces;
using server.Service.Interfaces.Authentication;
using server.Service.Models;
using server.Service.Models.Authenticate;

namespace server.Service.Services.Authentication
{
    /// <summary>
    /// AuthenticateService: Xử lý login & register.
    /// </summary>
    public class AuthenticateService : IAuthenticateService
    {
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly IRoleManagementService _roleManagementService;
        private readonly ILogger<AuthenticateService> _logger;

        public AuthenticateService(
            SignInManager<User> signInManager,
            UserManager<User> userManager,
            IJwtTokenService jwtTokenService,
            IRoleManagementService roleManagementService,
            ILogger<AuthenticateService> logger)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _jwtTokenService = jwtTokenService;
            _roleManagementService = roleManagementService;
            _logger = logger;
        }

        /// <summary>
        /// Đăng nhập user.
        /// </summary>
        public async Task<ApiResult> Login(LoginModel model)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(model.UserName) || string.IsNullOrWhiteSpace(model.Password))
                    return ApiResult.Fail("Thiếu UserName hoặc Password.", errorCode: "VALIDATION_ERROR");

                var user = await _userManager.FindByNameAsync(model.UserName);
                if (user is null)
                    return ApiResult.Fail("Sai tài khoản hoặc mật khẩu.", errorCode: "INVALID_CREDENTIALS");

                var signIn = await _signInManager.PasswordSignInAsync(
                    model.UserName, model.Password, isPersistent: false, lockoutOnFailure: false);

                if (!signIn.Succeeded)
                    return ApiResult.Fail("Sai tài khoản hoặc mật khẩu.", errorCode: "INVALID_CREDENTIALS");

                var roles = await _userManager.GetRolesAsync(user);
                var token = _jwtTokenService.GenerateToken(user, roles);

                return ApiResult.Success(token);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Login failed. Username={Username}", model?.UserName);
                return ApiResult.Fail("Đã có lỗi hệ thống khi đăng nhập.", errorCode: "LOGIN_EXCEPTION");
            }
        }

        /// <summary>
        /// Đăng ký user mới với role RegularUser.
        /// </summary>
        public async Task<ApiResult> Register(RegisterModel model)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(model.UserName) ||
                    string.IsNullOrWhiteSpace(model.Email) ||
                    string.IsNullOrWhiteSpace(model.Password))
                {
                    return ApiResult.Fail("Thiếu thông tin đăng ký bắt buộc.", errorCode: "VALIDATION_ERROR");
                }

                if (await _userManager.FindByNameAsync(model.UserName) is not null)
                    return ApiResult.Fail($"{model.UserName} đã được sử dụng. Vui lòng thử lại!", errorCode: "USERNAME_EXISTS");

                if (await _userManager.FindByEmailAsync(model.Email) is not null)
                    return ApiResult.Fail($"{model.Email} đã được sử dụng. Vui lòng thử lại!", errorCode: "EMAIL_EXISTS");

                var user = new User
                {
                    UserName = model.UserName.Trim(),
                    Email = model.Email.Trim(),
                    FullName = model.FullName?.Trim(),
                    CreatedDate = DateTime.Now,
                    IsAuthen = false
                };

                var createResult = await _userManager.CreateAsync(user, model.Password);
                if (!createResult.Succeeded)
                {
                    var errors = createResult.Errors.Select(e => e.Description).ToList();
                    return ApiResult.Fail("Đăng ký thất bại.", errorCode: "CREATE_USER_FAILED", errors: errors);
                }
                await _roleManagementService.EnsureRegularRoleExistsAsync();
                var (roleSucceeded, roleErrors) = await _roleManagementService.AddToRegularRoleAsync(user.Id);
                if (!roleSucceeded)
                    return ApiResult.Fail("Gán quyền thất bại.", errorCode: "ADD_ROLE_FAILED", errors: roleErrors);

                return ApiResult.Success(
                    data: new
                    {
                        user.Id,
                        user.UserName,
                        user.Email,
                        user.FullName
                    },
                    message: "Đăng ký thành công.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Register failed. Username={Username}, Email={Email}", model?.UserName, model?.Email);
                return ApiResult.Fail("Đã có lỗi hệ thống khi đăng ký.", errorCode: "REGISTER_EXCEPTION");
            }
        }

        /// <summary>
        /// Lấy role của user.
        /// </summary>
        public Task<ApiResult> GetRoleUser(int id)
            => Task.FromResult(ApiResult.Fail("Not implemented"));
    }
}