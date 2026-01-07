using server.Service.Models;
using server.Service.Models.UserRooms;

namespace server.Service.Interfaces
{
    public interface IUserRoomService
    {
        Task<ApiResult> AddUserToRoom(AddUserToRoomModel model);
        Task<ApiResult> RemoveUserFromRoom(int userId, int roomId);
        Task<ApiResult> ChangeUserRoleInRoom(ChangeUserRoleInRoomModel model);
        Task<ApiResult> BanUserFromRoom(int userId, int roomId);
        Task<ApiResult> UnbanUserFromRoom(int userId, int roomId);
        Task<ApiResult> GetUsersInRoom(int roomId);
        Task<ApiResult> GetRoomsForUser(int userId);
        Task<ApiResult> JoinRoomCode(JoinRoomCodeModel model);
    }
}
