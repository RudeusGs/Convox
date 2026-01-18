namespace server.Service.Models.BreakoutRooms
{
    public class AddBreakoutRoomModel
    {
        public string RoomName { get; set; }
        public int ParentRoomId { get; set; }
        public DateTime? ExpireAt { get; set; }
    }
}
