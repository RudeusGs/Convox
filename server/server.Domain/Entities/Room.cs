using server.Domain.Base;

namespace server.Domain.Entities
{
    // Phòng học
    public class Room : EntityBase
    {
        public string Name { get; set; } // Tên phòng học
        public int OwnerId { get; set; } // ID của người tạo phòng
        public string? Password { get; set; } // Mật khẩu phòng (nếu có)
        public bool IsLocked { get; set; } = false; // Phòng khóa hay mở
        public string? Avatar { get; set; } // Hình đại diện của phòng(có hoặc mặc định)
    }

}
