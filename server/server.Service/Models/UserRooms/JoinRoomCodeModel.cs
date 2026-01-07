using server.Domain.Enums;

namespace server.Service.Models.UserRooms
{
    public class JoinRoomCodeModel
    {
        public string RoomCode { get; set; }
        public string? Password { get; set; }
    }
}
