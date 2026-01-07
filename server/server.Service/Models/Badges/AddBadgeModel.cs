using Microsoft.AspNetCore.Http;

namespace server.Service.Models.Badges
{
    /// <summary>
    /// Model để tạo badge mới.
    /// 
    /// - Name: Tên badge (bắt buộc, không rỗng)
    /// - Description: Mô tả badge (tùy chọn)
    /// - ExperiencePoints: Điểm kinh nghiệm cần để đạt badge (bắt buộc, >= 0)
    /// - IconFile: File icon ảnh (bắt buộc, sẽ upload lên ImgBB)
    /// 
    /// Lưu ý:
    /// - Dùng multipart/form-data khi gửi request (do có file upload)
    /// - IconFile sẽ được upload và lưu URL vào Badge.Icon
    /// </summary>
    public class AddBadgeModel
    {
        /// <summary>
        /// Tên badge (bắt buộc).
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Mô tả badge (tùy chọn).
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Số điểm kinh nghiệm cần để đạt badge (bắt buộc, >= 0).
        /// </summary>
        public int ExperiencePoints { get; set; }

        /// <summary>
        /// File icon ảnh (bắt buộc, sẽ upload lên ImgBB).
        /// Hỗ trợ: .jpg, .jpeg, .png, .gif, .webp, .bmp
        /// Max size: 32MB
        /// </summary>
        public IFormFile IconFile { get; set; }
    }
}