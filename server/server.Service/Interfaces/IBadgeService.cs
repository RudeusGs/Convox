using server.Service.Models;
using server.Service.Models.Badge;

namespace server.Service.Interfaces
{
    /// <summary>
    /// Interface cho BadgeService.
    /// Cung cấp các operation CRUD cho Badge + upload icon lên ImgBB.
    /// </summary>
    public interface IBadgeService
    {
        /// <summary>
        /// Lấy tất cả badge.
        /// </summary>
        Task<ApiResult> GetAll();

        /// <summary>
        /// Lấy badge theo ID.
        /// </summary>
        Task<ApiResult> GetById(int id);

        /// <summary>
        /// Tạo badge mới.
        /// Sẽ upload IconFile lên ImgBB và lưu URL vào Badge.Icon.
        /// </summary>
        Task<ApiResult> Add(AddBadgeModel modelBadge, CancellationToken cancellationToken = default);

        /// <summary>
        /// Cập nhật badge.
        /// Nếu IconFile có thì upload lên ImgBB, nếu null thì giữ icon cũ.
        /// </summary>
        Task<ApiResult> Update(UpdateBadgeModel modelBadge, CancellationToken cancellationToken = default);

        /// <summary>
        /// Xóa badge theo ID.
        /// </summary>
        Task<ApiResult> Delete(int id);
    }
}
