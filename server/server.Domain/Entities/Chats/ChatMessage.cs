using server.Domain.Base;

namespace server.Domain.Entities.Chats
{
    // Chat message
    public class ChatMessage : EntityBase
    {
        public int RoomId { get; set; } // Id phòng gửi tin
        public int UserId { get; set; } // Người gửi
        public string Message { get; set; } = string.Empty; // Nội dung
        public string MessageType { get; set; } = "text"; // text, emoji, image, mixed, system, forwarded
        public string? ImageUrl { get; set; }
        
        // Reply feature
        public int? ReplyToMessageId { get; set; } // ID tin nhắn được reply (null = không reply)
        
        // Forward feature
        public bool IsForwarded { get; set; } = false; // Tin nhắn được forward từ nơi khác
        public int? ForwardedFromMessageId { get; set; } // ID tin nhắn gốc (nếu forward)
        public string? ForwardedFromSource { get; set; } // Nguồn forward: "room_1", "p2p_2", "breakroom_3"
        
        // Mention feature
        public string? MentionedUserIds { get; set; } // Comma-separated user IDs được mention (@user)
        
        public bool IsEdited => UpdatedDate.HasValue && UpdatedDate > CreatedDate;
        public bool HasReply => ReplyToMessageId.HasValue;
        public bool HasMentions => !string.IsNullOrEmpty(MentionedUserIds);
    }
}
