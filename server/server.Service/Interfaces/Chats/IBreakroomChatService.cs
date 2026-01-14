using server.Service.Models;
using server.Service.Models.Chats;

namespace server.Service.Interfaces
{
    public interface IBreakroomChatService
    {
        Task<ApiResult> GetHistoryOfBreakroomChat(int breakroomId, int page = 1, int pageSize = 50);
        Task<ApiResult> SendMessageWithImagesToBreakroom(SendBreakroomMessageWithImagesModel model);
    }
}
