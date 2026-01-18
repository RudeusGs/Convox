using server.Service.Models;
using server.Service.Models.BreakoutRooms;

namespace server.Service.Interfaces
{
    public interface IBreakoutRoomService
    {
        public Task<ApiResult> GetAllBreakoutRooms();
        public Task<ApiResult> GetBreakoutRoomById(int Id);
        public Task<ApiResult> AddBreakoutRoom(AddBreakoutRoomModel model);
        public Task<ApiResult> GetBreakoutRoomsByParentRoomId(int parentRoomId);
        public Task<ApiResult> DeleteBreakoutRoom(int breakoutRoomId);
        public Task<ApiResult> UpdateBreakoutRoom(UpdateBreakoutRoomModel model);
    }
}
