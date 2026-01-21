using server.Domain.Base;

namespace server.Domain.Entities.Chats
{
    /// <summary>
    /// Reaction cho tin nh?n P2P
    /// </summary>
    public class MessageReactionP2P : EntityBase
    {
        public int MessageId { get; set; } // FK to ChatP2P
        public int UserId { get; set; } // Ng??i th? reaction
        public string Emoji { get; set; } = string.Empty; // Emoji code
    }
}
