using server.Domain.Base;

namespace server.Domain.Entities.Chats
{
    /// <summary>
    /// Tin nh?n ???c ghim trong Room
    /// </summary>
    public class PinnedMessage : EntityBase
    {
        public int RoomId { get; set; } // Phòng ch?a tin nh?n ghim
        public int MessageId { get; set; } // FK to ChatMessage
        public int PinnedByUserId { get; set; } // Ng??i ghim
    }
}
