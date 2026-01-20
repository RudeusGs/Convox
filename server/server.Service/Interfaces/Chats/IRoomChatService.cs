using server.Service.Models;
using server.Service.Models.Chats;

namespace server.Service.Interfaces
{
    public interface IRoomChatService
    {
        Task<ApiResult> GetHistoryOfRoomChat(int roomId, int page = 1, int pageSize = 50);
        Task<ApiResult> SendMessageWithImagesToRoom(SendRoomMessageWithImagesModel model);
        Task<ApiResult> EditMessageInRoom(int messageId, int userId, string newMessage, List<string>? imageUrls = null);
        Task<ApiResult> DeleteMessageInRoom(int messageId, int userId);
    }
}
