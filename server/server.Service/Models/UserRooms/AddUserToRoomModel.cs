using server.Domain.Enums;

namespace server.Service.Models.UserRooms
{
    public class AddUserToRoomModel
    {
        public int UserId { get; set; }
        public int RoomId { get; set; }
        public RoomRole Role { get; set; } = RoomRole.RegularUser;
    }
}
