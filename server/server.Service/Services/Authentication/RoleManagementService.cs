using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using server.Domain.Constants;
using server.Domain.Entities;
using server.Service.Interfaces.Authentication;

namespace server.Service.Services.Authentication
{
    /// <summary>
    /// RoleManagementService: Xử lý role operations.
    /// </summary>
    public class RoleManagementService : IRoleManagementService
    {
        private readonly RoleManager<IdentityRole<int>> _roleManager;
        private readonly UserManager<User> _userManager;
        private readonly ILogger<RoleManagementService> _logger;

        public RoleManagementService(
            RoleManager<IdentityRole<int>> roleManager,
            UserManager<User> userManager,
            ILogger<RoleManagementService> logger)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _logger = logger;
        }

        /// <summary>
        /// Đảm bảo role RegularUser tồn tại.
        /// </summary>
        public async Task EnsureRegularRoleExistsAsync()
        {
            var roleName = RoleConstants.REGULAR_USER.ToString();
            if (!await _roleManager.RoleExistsAsync(roleName))
            {
                var result = await _roleManager.CreateAsync(new IdentityRole<int>(roleName));
                if (!result.Succeeded)
                {
                    var errors = string.Join(" | ", result.Errors.Select(e => e.Description));
                    _logger.LogError("Cannot create role '{RoleName}'. {Errors}", roleName, errors);
                    throw new InvalidOperationException($"Cannot create role '{roleName}'. {errors}");
                }
            }
        }

        /// <summary>
        /// Gán role RegularUser cho user.
        /// </summary>
        public async Task<(bool succeeded, List<string> errors)> AddToRegularRoleAsync(int userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
                return (false, new List<string> { "User không tìm thấy" });

            var result = await _userManager.AddToRoleAsync(user, RoleConstants.REGULAR_USER.ToString());
            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description).ToList();
                return (false, errors);
            }

            return (true, new List<string>());
        }
    }
}