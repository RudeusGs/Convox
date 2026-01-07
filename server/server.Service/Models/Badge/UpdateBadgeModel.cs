using Microsoft.AspNetCore.Http;

namespace server.Service.Models.Badge
{
    /// <summary>
    /// Model để cập nhật badge.
    /// 
    /// - Id: ID badge cần cập nhật (bắt buộc)
    /// - Name: Tên badge (bắt buộc, không rỗng)
    /// - Description: Mô tả badge (tùy chọn)
    /// - ExperiencePoints: Điểm kinh nghiệm cần để đạt badge (bắt buộc, >= 0)
    /// - IconFile: File icon ảnh mới (tùy chọn, nếu có sẽ upload lên ImgBB)
    /// 
    /// Lưu ý:
    /// - Dùng multipart/form-data khi có IconFile
    /// - Nếu IconFile = null thì giữ icon cũ
    /// - Nếu ExperiencePoints = 0 thì cập nhật thành 0
    /// </summary>
    public class UpdateBadgeModel
    {
        /// <summary>
        /// ID badge cần cập nhật (bắt buộc).
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Tên badge mới (bắt buộc).
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Mô tả badge mới (tùy chọn).
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Số điểm kinh nghiệm cần để đạt badge (bắt buộc, >= 0).
        /// </summary>
        public int ExperiencePoints { get; set; }

        /// <summary>
        /// File icon ảnh mới (tùy chọn).
        /// Nếu null thì giữ icon cũ.
        /// Hỗ trợ: .jpg, .jpeg, .png, .gif, .webp, .bmp
        /// Max size: 32MB
        /// </summary>
        public IFormFile? IconFile { get; set; }
    }
}