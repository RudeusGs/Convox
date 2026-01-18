using server.Service.Models;
using server.Service.Models.BreakoutRooms;

namespace server.Service.Interfaces
{
    public interface IBreakoutRoomMemberService
    {
        public Task<ApiResult> GetByBreakoutRoomMemberId(int Id);
        public Task<ApiResult> GetByBreakoutRoomId(int breakoutRoomId);
        public Task<ApiResult> Add(AddBreakoutRoomModel breakoutRoomMember);
        public Task<ApiResult> Delete(int id);
        public Task<ApiResult> Update(UpdateBreakoutRoomModel breakoutRoomMember);


    }
}
