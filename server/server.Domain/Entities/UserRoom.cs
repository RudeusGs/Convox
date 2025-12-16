using server.Domain.Base;
using server.Domain.Enums;
namespace server.Domain.Entities
{
    // User <-> Room với role
    public class UserRoom : EntityBase
    {
        public int UserId { get; set; } // Id user
        public int RoomId { get; set; } // Id phòng
        public RoomRole Role { get; set; } // quyền của thành viên
        public bool IsBan { get; set; } = false; // nếu bị ban thì không được sử dụng phòng
    }
}
