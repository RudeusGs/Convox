using server.Service.Models;

namespace server.Service.Interfaces
{
    /// <summary>
    /// Interface cho UserBadgeService.
    /// Quản lý huy hiệu của user với hệ thống mốc kinh nghiệm cố định.
    /// </summary>
    public interface IUserBadgeService
    {
        /// <summary>
        /// Lấy danh sách huy hiệu của user.
        /// </summary>
        Task<ApiResult> GetByUserId(int userId);

        /// <summary>
        /// Lấy danh sách user sở hữu huy hiệu cụ thể.
        /// </summary>
        Task<ApiResult> GetByBadgeId(int badgeId);

        /// <summary>
        /// Thêm điểm kinh nghiệm cho user.
        /// Tự động unlock badge khi đạt mốc.
        /// </summary>
        Task<ApiResult> AddPoints(int userId, int experiencePoints);

        /// <summary>
        /// Lấy tổng điểm kinh nghiệm của user.
        /// </summary>
        Task<ApiResult> GetTotalPoints(int userId);

        /// <summary>
        /// Gỡ huy hiệu khỏi user.
        /// </summary>
        Task<ApiResult> Remove(int userId, int badgeId);
    }
}