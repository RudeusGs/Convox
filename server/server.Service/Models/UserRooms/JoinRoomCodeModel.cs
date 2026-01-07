using server.Domain.Enums;

namespace server.Service.Models.UserRooms
{
    public class JoinRoomCodeModel
    {
        public int UserId { get; set; }
        public string RoomCode { get; set; }
        public int RoomId { get; set; }
        public string? Password { get; set; }
        public RoomRole Role { get; set; } = RoomRole.RegularUser;
    }
}
