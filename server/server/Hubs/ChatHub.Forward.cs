using Microsoft.AspNetCore.SignalR;
using server.Service.Models.Chats;

namespace server.Hubs
{
    public partial class ChatHub
    {
        #region Forward to Room

        public async Task ForwardToRoom(int sourceMessageId, string sourceType, int sourceId, int targetRoomId)
        {
            var userId = GetUserId();
            if (!userId.HasValue)
            {
                await Clients.Caller.SendAsync("Error", "Unauthorized");
                return;
            }

            var model = new ForwardToRoomModel
            {
                SourceMessageId = sourceMessageId,
                SourceType = sourceType,
                SourceId = sourceId,
                ForwarderId = userId.Value,
                TargetRoomId = targetRoomId
            };

            var result = await _forwardMessageService.ForwardToRoom(model);

            if (result.IsSuccess)
                await Clients.Group($"room_{targetRoomId}").SendAsync("ReceiveMessage", result.Data);
            else
                await Clients.Caller.SendAsync("Error", result.Message);
        }

        #endregion

        #region Forward to P2P

        public async Task ForwardToP2P(int sourceMessageId, string sourceType, int sourceId, int targetReceiverId)
        {
            var userId = GetUserId();
            if (!userId.HasValue)
            {
                await Clients.Caller.SendAsync("Error", "Unauthorized");
                return;
            }

            var model = new ForwardToP2PModel
            {
                SourceMessageId = sourceMessageId,
                SourceType = sourceType,
                SourceId = sourceId,
                ForwarderId = userId.Value,
                TargetReceiverId = targetReceiverId
            };

            var result = await _forwardMessageService.ForwardToP2P(model);

            if (result.IsSuccess)
            {
                await Clients.Group($"user_{targetReceiverId}").SendAsync("ReceiveP2PMessage", result.Data);
                await Clients.Group($"user_{userId.Value}").SendAsync("ReceiveP2PMessage", result.Data);
            }
            else
            {
                await Clients.Caller.SendAsync("Error", result.Message);
            }
        }

        #endregion

        #region Forward to Breakroom

        public async Task ForwardToBreakroom(int sourceMessageId, string sourceType, int sourceId, int targetBreakroomId)
        {
            var userId = GetUserId();
            if (!userId.HasValue)
            {
                await Clients.Caller.SendAsync("Error", "Unauthorized");
                return;
            }

            var model = new ForwardToBreakroomModel
            {
                SourceMessageId = sourceMessageId,
                SourceType = sourceType,
                SourceId = sourceId,
                ForwarderId = userId.Value,
                TargetBreakroomId = targetBreakroomId
            };

            var result = await _forwardMessageService.ForwardToBreakroom(model);

            if (result.IsSuccess)
                await Clients.Group($"breakroom_{targetBreakroomId}").SendAsync("ReceiveMessage", result.Data);
            else
                await Clients.Caller.SendAsync("Error", result.Message);
        }

        #endregion
    }
}
