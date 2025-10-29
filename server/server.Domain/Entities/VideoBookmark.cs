using server.Domain.Base;

namespace server.Domain.Entities
{
    // Bookmark video
    public class VideoBookmark : EntityBase
    {
        public int RecordingId { get; set; } // Id recording
        public int UserId { get; set; } // Ai đánh dấu
        public TimeSpan Time { get; set; } // Thời gian highlight
        public string Note { get; set; } // Ghi chú
    }
}
