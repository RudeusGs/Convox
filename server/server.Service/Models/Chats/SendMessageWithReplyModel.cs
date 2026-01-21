namespace server.Service.Models.Chats
{
    /// <summary>
    /// Model g?i tin nh?n v?i Reply
    /// </summary>
    public class SendMessageWithReplyModel
    {
        public int SenderId { get; set; }
        public string MessageContent { get; set; } = string.Empty;
        public List<string>? ImageUrls { get; set; }
        public int? ReplyToMessageId { get; set; }
        public List<int>? MentionedUserIds { get; set; }
    }

    /// <summary>
    /// Model g?i tin nh?n Room v?i Reply
    /// </summary>
    public class SendRoomMessageWithReplyModel : SendMessageWithReplyModel
    {
        public int RoomId { get; set; }
    }

    /// <summary>
    /// Model g?i tin nh?n P2P v?i Reply
    /// </summary>
    public class SendP2PMessageWithReplyModel : SendMessageWithReplyModel
    {
        public int ReceiverId { get; set; }
    }

    /// <summary>
    /// Model g?i tin nh?n Breakroom v?i Reply
    /// </summary>
    public class SendBreakroomMessageWithReplyModel : SendMessageWithReplyModel
    {
        public int BreakroomId { get; set; }
    }
}
