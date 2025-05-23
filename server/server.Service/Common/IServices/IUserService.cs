namespace server.Service.Common.IServices
{
    public interface IUserService
    {
        string RoleName { get; }
        string UserName { get; }
        int UserId { get; }

        void DeserializeUserId(string userSerialized);
    }
}
