using server.Domain.Base;

namespace server.Domain.Entities
{
    // Notification
    public class Notification : EntityBase
    {
        public int UserId { get; set; } // Gửi cho ai
        public string Title { get; set; } // Tiêu đề
        public string Message { get; set; } // Nội dung
        public bool IsRead { get; set; } = false; // Đọc chưa
        public DateTime NotifyTime { get; set; } // Thời gian gửi
    }
}
