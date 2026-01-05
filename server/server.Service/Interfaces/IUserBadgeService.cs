using server.Service.Models;

namespace server.Service.Interfaces
{
    /// <summary>
    /// IUserBadgeService: Quản lý badge của user.
    /// </summary>
    public interface IUserBadgeService
    {
        /// <summary>
        /// Lấy danh sách badge của user.
        /// </summary>
        Task<ApiResult> GetByUserId(int userId);

        /// <summary>
        /// Lấy danh sách user theo badge.
        /// </summary>
        Task<ApiResult> GetByBadgeId(int badgeId);

        /// <summary>
        /// Gán badge cho user (khởi tạo với tier Newbie, 0 điểm).
        /// </summary>
        Task<ApiResult> Add(int userId, int badgeId);

        /// <summary>
        /// Cộng điểm vào badge của user và tự động nâng tier nếu đạt ngưỡng.
        /// </summary>
        Task<ApiResult> AddPoints(int userId, int badgeId, int points);

        /// <summary>
        /// Gỡ badge khỏi user.
        /// </summary>
        Task<ApiResult> Remove(int userId, int badgeId);
    }
}