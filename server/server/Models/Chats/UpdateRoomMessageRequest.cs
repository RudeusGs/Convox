namespace server.Models.Chats
{
    public class UpdateRoomMessageRequest
    {
        public string? MessageContent { get; set; }
        public List<string>? ImageUrls { get; set; }
    }
}
