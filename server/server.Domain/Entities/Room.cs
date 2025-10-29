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

        // Danh sách Id của user trong phòng
        public List<int> UserIds { get; set; } = new List<int>();
    }

}
