using server.Domain.Base;

namespace server.Domain.Entities
{
    // Chat message
    public class ChatMessage : EntityBase
    {
        public int RoomId { get; set; } // Id phòng gửi tin
        public int SenderId { get; set; } // Ai gửi
        public string Message { get; set; } // Nội dung
        public string MessageType { get; set; } // text, emoji, system...
    }
}
