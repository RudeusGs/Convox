using server.Service.Models;
using server.Service.Models.Chats;

namespace server.Service.Interfaces
{
    public interface IP2PChatService
    {
        Task<ApiResult> GetHistoryOfChatP2P(int userId1, int userId2, int page = 1, int pageSize = 50);
        Task<ApiResult> SendMessageWithImagesToP2P(SendP2PMessageWithImagesModel model);
    }
}
