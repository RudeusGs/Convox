using server.Domain.Enums;

namespace server.Service.Models.UserRooms
{
    public class ChangeUserRoleInRoomModel
    {
        public int UserId { get; set; }
        public int RoomId { get; set; }
        public RoomRole NewRole { get; set; }
    }
}
