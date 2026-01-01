using server.Service.Models;
using server.Service.Models.Room;

namespace server.Service.Interfaces
{
    public interface IRoomService
    {
        Task<ApiResult> AddRoom(AddRoomModel model);
        Task<ApiResult> Update(UpdateRoomModel model);
        Task<ApiResult> Delete(int id);
        Task<ApiResult> GetAll();
        Task<ApiResult> GetById(int id);
        Task<ApiResult> GetByUserId(int userId);
    }
}
