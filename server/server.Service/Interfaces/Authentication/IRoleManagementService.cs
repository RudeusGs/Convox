namespace server.Service.Interfaces.Authentication
{
    /// <summary>
    /// IRoleManagementService: Quản lý role của user.
    /// </summary>
    public interface IRoleManagementService
    {
        /// <summary>
        /// Đảm bảo role RegularUser tồn tại, nếu không thì tạo mới.
        /// </summary>
        Task EnsureRegularRoleExistsAsync();

        /// <summary>
        /// Gán role RegularUser cho user.
        /// </summary>
        Task<(bool succeeded, List<string> errors)> AddToRegularRoleAsync(int userId);
    }
}