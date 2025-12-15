namespace server.Service.Models.Authenticate
{
    public class UserToken
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public string? FullName { get; set; }
        public string Email { get; set; }
        public string Token { get; set; }
        public DateTime Expires { get; set; }
    }
}
