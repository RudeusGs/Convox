using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using server.Domain.Constants;
using server.Domain.Entities;
using server.Infrastructure.Persistence;
using server.Service.Interfaces;
using server.Service.Models;
using server.Service.Models.Authenticate;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace server.Service.Services
{
    public class AuthenticateService : IAuthenticateService
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly RoleManager<IdentityRole<int>> _roleManager;
        private readonly DataContext _dbContext;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthenticateService> _logger;

        public AuthenticateService(
            DataContext dbContext,
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            RoleManager<IdentityRole<int>> roleManager,
            IConfiguration configuration,
            ILogger<AuthenticateService> logger)
        {
            _dbContext = dbContext;
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _configuration = configuration;
            _logger = logger;
        }

        public Task<ApiResult> GetRoleUser(int id)
            => Task.FromResult(ApiResult.Fail("Not implemented"));

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
                var token = GenerateUserToken(user, roles);

                return ApiResult.Success(token);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Login failed. Username={Username}", model?.UserName);
                return ApiResult.Fail("Đã có lỗi hệ thống khi đăng nhập.", errorCode: "LOGIN_EXCEPTION");
            }
        }

        public async Task<ApiResult> Register(RegisterModel model)
        {
            await using var tx = await _dbContext.Database.BeginTransactionAsync();

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
                    CreatedDate = DateTime.UtcNow.AddHours(7),
                    IsAuthen = false
                };

                var createResult = await _userManager.CreateAsync(user, model.Password);
                if (!createResult.Succeeded)
                {
                    var errors = createResult.Errors.Select(e => e.Description).ToList();
                    return ApiResult.Fail("Đăng ký thất bại.", errorCode: "CREATE_USER_FAILED", errors: errors);
                }

                await EnsureRegularRoleExistsAsync();

                var addRoleResult = await _userManager.AddToRoleAsync(user, RoleConstants.REGULAR_USER.ToString());
                if (!addRoleResult.Succeeded)
                {
                    var errors = addRoleResult.Errors.Select(e => e.Description).ToList();
                    return ApiResult.Fail("Gán quyền thất bại.", errorCode: "ADD_ROLE_FAILED", errors: errors);
                }

                await _dbContext.SaveChangesAsync();
                await tx.CommitAsync();

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
                await tx.RollbackAsync();
                _logger.LogError(ex, "Register failed. Username={Username}, Email={Email}", model?.UserName, model?.Email);
                return ApiResult.Fail("Đã có lỗi hệ thống khi đăng ký.", errorCode: "REGISTER_EXCEPTION");
            }
        }

        private async Task EnsureRegularRoleExistsAsync()
        {
            var roleName = RoleConstants.REGULAR_USER.ToString();
            if (!await _roleManager.RoleExistsAsync(roleName))
            {
                var result = await _roleManager.CreateAsync(new IdentityRole<int>(roleName));
                if (!result.Succeeded)
                {
                    var errors = string.Join(" | ", result.Errors.Select(e => e.Description));
                    throw new InvalidOperationException($"Cannot create role '{roleName}'. {errors}");
                }
            }
        }

        private UserToken GenerateUserToken(User user, IEnumerable<string> roles)
        {
            var secret = _configuration["Jwt:Secret"];
            if (string.IsNullOrWhiteSpace(secret))
                throw new InvalidOperationException("Missing Jwt:Secret in configuration.");

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var now = DateTime.UtcNow;
            var expires = now.AddDays(7);

            var claims = BuildClaims(user, roles);

            var jwt = new JwtSecurityToken(
                issuer: _configuration["Jwt:ValidIssuer"],
                audience: _configuration["Jwt:ValidAudience"],
                claims: claims,
                notBefore: now,
                expires: expires,
                signingCredentials: creds
            );

            var token = new JwtSecurityTokenHandler().WriteToken(jwt);

            return new UserToken
            {
                UserId = user.Id,
                Username = user.UserName,
                Email = user.Email,
                FullName = user.FullName,
                Token = token,
                Expires = expires
            };
        }

        private static IEnumerable<Claim> BuildClaims(User user, IEnumerable<string> roles)
        {
            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64),
                new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new(ClaimTypes.Name, user.UserName ?? string.Empty),
                new(ClaimTypes.Email, user.Email ?? string.Empty),
            };

            foreach (var r in roles.Distinct())
                claims.Add(new Claim(ClaimTypes.Role, r));

            return claims;
        }
    }
}
