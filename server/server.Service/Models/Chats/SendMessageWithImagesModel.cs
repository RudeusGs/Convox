namespace server.Service.Models.Chats
{
    public class SendMessageWithImagesModel
    {
        public int SenderId { get; set; }
        public string MessageContent { get; set; } = string.Empty;
        public List<string> ImageUrls { get; set; } = new();
        
        // Reply feature
        public int? ReplyToMessageId { get; set; }
        
        // Mention feature
        public List<int>? MentionedUserIds { get; set; }
    }
}
