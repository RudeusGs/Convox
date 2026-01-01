namespace server.Service.Models.Room
{
    public class UpdateRoomModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Password { get; set; }
        public string? Avatar { get; set; }
    }
}
