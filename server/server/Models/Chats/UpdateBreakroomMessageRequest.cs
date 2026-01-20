namespace server.Models.Chats
{
    public class UpdateBreakroomMessageRequest
    {
        public string? MessageContent { get; set; }
        public List<string>? ImageUrls { get; set; }
    }
}
