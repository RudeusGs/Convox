using server.Domain.Base;

namespace server.Domain.Entities
{
    // Thành viên của phòng con
    public class BreakoutRoomMember : EntityBase
    {
        public int BreakoutRoomId { get; set; }
        public int UserId { get; set; }
        public DateTime JoinedAt { get; set; } = DateTime.UtcNow;
        public DateTime? LeftAt { get; set; }
    }
}
