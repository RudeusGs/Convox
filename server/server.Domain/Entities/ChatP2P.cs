using server.Domain.Base;

namespace server.Domain.Entities
{
    public class ChatP2P : EntityBase
    {
        public int ReceiverId { get; set; } // Id người nhận
        public int SenderId { get; set; } // Người gửi
        public string Message { get; set; } // Nội dung
        public string MessageType { get; set; } // text, emoji, system...
    }
}
