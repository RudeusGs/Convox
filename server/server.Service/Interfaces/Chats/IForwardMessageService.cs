using server.Service.Models;
using server.Service.Models.Chats;

namespace server.Service.Interfaces
{
    public interface IForwardMessageService
    {
        // Forward ??n Room
        Task<ApiResult> ForwardToRoom(ForwardToRoomModel model);

        // Forward ??n P2P
        Task<ApiResult> ForwardToP2P(ForwardToP2PModel model);

        // Forward ??n Breakroom
        Task<ApiResult> ForwardToBreakroom(ForwardToBreakroomModel model);

        // L?y tin nh?n g?c ?? forward
        Task<ApiResult> GetOriginalMessage(string sourceType, int sourceId, int messageId);
    }
}
