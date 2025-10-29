using server.Domain.Base;


namespace server.Domain.Entities
{
    // User <-> Room với role
    public class UserRoom : EntityBase
    {
        public int UserId { get; set; } // Id user
        public int RoomId { get; set; } // Id phòng
        public string RoleName { get; set; } // GroupLeader, GroupDeputy, RegularUser
        public bool IsTemporaryRole { get; set; } = false; // Role tạm thời
        public DateTime? TemporaryExpire { get; set; } // Hết hạn role tạm thời
    }
}
