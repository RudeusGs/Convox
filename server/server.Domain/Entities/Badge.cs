using server.Domain.Base;

namespace server.Domain.Entities
{
    /// <summary>
    /// Badge / Gamification entity.
    /// 
    /// Lưu ý:
    /// - Icon: URL của ảnh từ ImgBB (bắt buộc, max 2048 ký tự)
    /// - ExperiencePoints: Số điểm kinh nghiệm cần để đạt badge (bắt buộc, >= 0)
    /// - Description: Mô tả badge (tùy chọn)
    /// </summary>
    public class Badge : EntityBase
    {
        /// <summary>
        /// Tên badge (bắt buộc, unique).
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// URL icon ảnh từ ImgBB (bắt buộc).
        /// Max length: 2048
        /// </summary>
        public string Icon { get; set; }

        /// <summary>
        /// Mô tả badge (tùy chọn).
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Số điểm kinh nghiệm cần để đạt được badge (bắt buộc).
        /// Min: 0, Max: 999999
        /// </summary>
        public int ExperiencePoints { get; set; }
    }
}