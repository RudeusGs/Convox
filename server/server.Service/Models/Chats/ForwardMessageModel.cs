namespace server.Service.Models.Chats
{
    /// <summary>
    /// Model forward tin nh?n
    /// </summary>
    public class ForwardMessageModel
    {
        public int SourceMessageId { get; set; } // ID tin nh?n g?c
        public string SourceType { get; set; } = string.Empty; // "room", "p2p", "breakroom"
        public int SourceId { get; set; } // RoomId / ReceiverId / BreakroomId ngu?n
        public int ForwarderId { get; set; } // User th?c hi?n forward
    }

    /// <summary>
    /// Forward ??n Room
    /// </summary>
    public class ForwardToRoomModel : ForwardMessageModel
    {
        public int TargetRoomId { get; set; }
    }

    /// <summary>
    /// Forward ??n P2P
    /// </summary>
    public class ForwardToP2PModel : ForwardMessageModel
    {
        public int TargetReceiverId { get; set; }
    }

    /// <summary>
    /// Forward ??n Breakroom
    /// </summary>
    public class ForwardToBreakroomModel : ForwardMessageModel
    {
        public int TargetBreakroomId { get; set; }
    }
}
