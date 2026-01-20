namespace server.Models.Chats
{
    public class UpdateP2PMessageRequest
    {
        public string? MessageContent { get; set; }
        public List<string>? ImageUrls { get; set; }
    }
}
