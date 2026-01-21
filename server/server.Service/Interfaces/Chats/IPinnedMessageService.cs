using server.Service.Models;

namespace server.Service.Interfaces
{
    public interface IPinnedMessageService
    {
        // Room
        Task<ApiResult> PinMessageInRoom(int roomId, int messageId, int userId);
        Task<ApiResult> UnpinMessageInRoom(int roomId, int messageId, int userId);
        Task<ApiResult> GetPinnedMessagesInRoom(int roomId);

        // Breakroom
        Task<ApiResult> PinMessageInBreakroom(int breakroomId, int messageId, int userId);
        Task<ApiResult> UnpinMessageInBreakroom(int breakroomId, int messageId, int userId);
        Task<ApiResult> GetPinnedMessagesInBreakroom(int breakroomId);
    }
}
