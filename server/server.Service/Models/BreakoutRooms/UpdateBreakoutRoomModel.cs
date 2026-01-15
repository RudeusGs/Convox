namespace server.Service.Models.BreakoutRooms
{
    public class UpdateBreakoutRoomModel
    {
        public int Id { get; set; }
        public string RoomName { get; set; }
        public DateTime? ExpireAt { get; set; }
    }
}
