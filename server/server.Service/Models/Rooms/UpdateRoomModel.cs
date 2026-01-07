namespace server.Service.Models.Rooms
{
    public class UpdateRoomModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Password { get; set; }
    }
}
