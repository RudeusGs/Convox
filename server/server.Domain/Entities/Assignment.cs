using server.Domain.Base;

namespace server.Domain.Entities
{
    // Bài tập
    public class Assignment : EntityBase
    {
        public int RoomId { get; set; } // Id phòng
        public string Title { get; set; } // Tiêu đề
        public string Description { get; set; } // Mô tả
        public DateTime DueDate { get; set; } // Hạn nộp
    }
}
