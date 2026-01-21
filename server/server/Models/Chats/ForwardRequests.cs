namespace server.Models.Chats
{
    public class ForwardToRoomRequest
    {
        public int SourceMessageId { get; set; }
        public string SourceType { get; set; } = string.Empty;
        public int SourceId { get; set; }
        public int TargetRoomId { get; set; }
    }

    public class ForwardToP2PRequest
    {
        public int SourceMessageId { get; set; }
        public string SourceType { get; set; } = string.Empty;
        public int SourceId { get; set; }
        public int TargetReceiverId { get; set; }
    }

    public class ForwardToBreakroomRequest
    {
        public int SourceMessageId { get; set; }
        public string SourceType { get; set; } = string.Empty;
        public int SourceId { get; set; }
        public int TargetBreakroomId { get; set; }
    }
}
