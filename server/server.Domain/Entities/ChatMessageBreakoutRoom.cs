using server.Domain.Base;

namespace server.Domain.Entities
{
    public class ChatMessageBreakoutRoom : EntityBase
    {
        public int BreakoutRoomId { get; set; } // Id phòng con gửi tin
        public int UserId { get; set; } // Người gửi
        public string Message { get; set; } // Nội dung
        public string MessageType { get; set; } // text, emoji, system...
        public string? ImageUrl { get; set; }
    }
}
