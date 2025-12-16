using server.Domain.Base;

namespace server.Domain.Entities
{
    // Bài tập dành cho chủ phòng tạo
    public class Assignment : EntityBase
    {
        public int RoomId { get; set; } // Id phòng
        public int UserId { get; set; } // Người tạo
        public string Title { get; set; } // Tiêu đề
        public string? Description { get; set; } // Mô tả
        public DateTime DueDate { get; set; } // Hạn nộp
    }
}
