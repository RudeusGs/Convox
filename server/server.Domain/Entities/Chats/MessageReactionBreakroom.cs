using server.Domain.Base;

namespace server.Domain.Entities.Chats
{
    /// <summary>
    /// Reaction cho tin nh?n trong Breakroom
    /// </summary>
    public class MessageReactionBreakroom : EntityBase
    {
        public int MessageId { get; set; } // FK to ChatMessageBreakoutRoom
        public int UserId { get; set; } // Ng??i th? reaction
        public string Emoji { get; set; } = string.Empty; // Emoji code
    }
}
