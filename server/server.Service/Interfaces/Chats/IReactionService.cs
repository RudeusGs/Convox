using server.Service.Models;

namespace server.Service.Interfaces
{
    public interface IReactionService
    {
        // Room
        Task<ApiResult> ToggleReactionInRoom(int messageId, int userId, string emoji);
        Task<ApiResult> GetReactionsForRoomMessage(int messageId);

        // Breakroom
        Task<ApiResult> ToggleReactionInBreakroom(int messageId, int userId, string emoji);
        Task<ApiResult> GetReactionsForBreakroomMessage(int messageId);

        // P2P
        Task<ApiResult> ToggleReactionP2P(int messageId, int userId, string emoji);
        Task<ApiResult> GetReactionsForP2PMessage(int messageId);
    }
}
