using server.Domain.Base;

namespace server.Domain.Entities.Chats
{
    /// <summary>
    /// Tin nh?n ???c ghim trong Breakroom
    /// </summary>
    public class PinnedMessageBreakroom : EntityBase
    {
        public int BreakroomId { get; set; } // Phòng con ch?a tin nh?n ghim
        public int MessageId { get; set; } // FK to ChatMessageBreakoutRoom
        public int PinnedByUserId { get; set; } // Ng??i ghim
    }
}
