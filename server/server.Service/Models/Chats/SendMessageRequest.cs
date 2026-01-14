namespace server.Models.Chats
{
    public class SendMessageRequest
    {
        public string MessageContent { get; set; } = string.Empty;
        public List<string>? ImageUrls { get; set; }
    }
}
