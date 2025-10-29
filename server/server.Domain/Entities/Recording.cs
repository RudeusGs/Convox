using server.Domain.Base;

namespace server.Domain.Entities
{
    // Ghi hình buổi học
    public class Recording : EntityBase
    {
        public int RoomId { get; set; } // Id phòng
        public string FilePath { get; set; } // File video
        public string Title { get; set; } // Tên buổi học
    }
}
