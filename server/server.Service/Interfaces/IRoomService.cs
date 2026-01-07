
using Microsoft.AspNetCore.Http;
using server.Service.Models;
using server.Service.Models.Rooms;

namespace server.Service.Interfaces
{
    public interface IRoomService
    {
        Task<ApiResult> Add(AddRoomModel model);
        Task<ApiResult> Update(UpdateRoomModel model);
        Task<ApiResult> Delete(int id);
        Task<ApiResult> GetAll();
        Task<ApiResult> GetById(int id);
        Task<ApiResult> GetByUserId(int userId);
        Task<ApiResult> UploadAvatarAsync(int roomId, IFormFile file, CancellationToken cancellationToken = default);
    }
}

