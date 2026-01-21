using server.Domain.Base;

namespace server.Domain.Entities.Chats
{
    public class ChatMessageBreakoutRoom : EntityBase
    {
        public int BreakoutRoomId { get; set; } // Id phòng con gửi tin
        public int UserId { get; set; } // Người gửi
        public string Message { get; set; } = string.Empty; // Nội dung
        public string MessageType { get; set; } = "text"; // text, emoji, image, mixed, system, forwarded
        public string? ImageUrl { get; set; }
        
        // Reply feature
        public int? ReplyToMessageId { get; set; } // ID tin nhắn được reply
        
        // Forward feature
        public bool IsForwarded { get; set; } = false;
        public int? ForwardedFromMessageId { get; set; }
        public string? ForwardedFromSource { get; set; } // "room_1", "p2p_2", "breakroom_3"
        
        // Mention feature
        public string? MentionedUserIds { get; set; }
        
        public bool IsEdited => UpdatedDate.HasValue && UpdatedDate > CreatedDate;
        public bool HasReply => ReplyToMessageId.HasValue;
        public bool HasMentions => !string.IsNullOrEmpty(MentionedUserIds);
    }
}
