using Microsoft.AspNetCore.SignalR;

namespace server.Hubs
{
    public partial class ChatHub
    {
        #region Room Pinned Messages

        public async Task PinMessageInRoom(int roomId, int messageId)
        {
            var userId = GetUserId();
            if (!userId.HasValue)
            {
                await Clients.Caller.SendAsync("Error", "Unauthorized");
                return;
            }

            var result = await _pinnedMessageService.PinMessageInRoom(roomId, messageId, userId.Value);

            if (result.IsSuccess)
                await Clients.Group($"room_{roomId}").SendAsync("MessagePinned", result.Data);
            else
                await Clients.Caller.SendAsync("Error", result.Message);
        }

        public async Task UnpinMessageInRoom(int roomId, int messageId)
        {
            var userId = GetUserId();
            if (!userId.HasValue)
            {
                await Clients.Caller.SendAsync("Error", "Unauthorized");
                return;
            }

            var result = await _pinnedMessageService.UnpinMessageInRoom(roomId, messageId, userId.Value);

            if (result.IsSuccess)
                await Clients.Group($"room_{roomId}").SendAsync("MessageUnpinned", result.Data);
            else
                await Clients.Caller.SendAsync("Error", result.Message);
        }

        public async Task GetPinnedMessagesInRoom(int roomId)
        {
            var userId = GetUserId();
            if (!userId.HasValue)
            {
                await Clients.Caller.SendAsync("Error", "Unauthorized");
                return;
            }

            var result = await _pinnedMessageService.GetPinnedMessagesInRoom(roomId);

            if (result.IsSuccess)
                await Clients.Caller.SendAsync("PinnedMessagesLoaded", result.Data);
            else
                await Clients.Caller.SendAsync("Error", result.Message);
        }

        #endregion

        #region Breakroom Pinned Messages

        public async Task PinMessageInBreakroom(int breakroomId, int messageId)
        {
            var userId = GetUserId();
            if (!userId.HasValue)
            {
                await Clients.Caller.SendAsync("Error", "Unauthorized");
                return;
            }

            var result = await _pinnedMessageService.PinMessageInBreakroom(breakroomId, messageId, userId.Value);

            if (result.IsSuccess)
                await Clients.Group($"breakroom_{breakroomId}").SendAsync("MessagePinned", result.Data);
            else
                await Clients.Caller.SendAsync("Error", result.Message);
        }

        public async Task UnpinMessageInBreakroom(int breakroomId, int messageId)
        {
            var userId = GetUserId();
            if (!userId.HasValue)
            {
                await Clients.Caller.SendAsync("Error", "Unauthorized");
                return;
            }

            var result = await _pinnedMessageService.UnpinMessageInBreakroom(breakroomId, messageId, userId.Value);

            if (result.IsSuccess)
                await Clients.Group($"breakroom_{breakroomId}").SendAsync("MessageUnpinned", result.Data);
            else
                await Clients.Caller.SendAsync("Error", result.Message);
        }

        public async Task GetPinnedMessagesInBreakroom(int breakroomId)
        {
            var userId = GetUserId();
            if (!userId.HasValue)
            {
                await Clients.Caller.SendAsync("Error", "Unauthorized");
                return;
            }

            var result = await _pinnedMessageService.GetPinnedMessagesInBreakroom(breakroomId);

            if (result.IsSuccess)
                await Clients.Caller.SendAsync("PinnedMessagesLoaded", result.Data);
            else
                await Clients.Caller.SendAsync("Error", result.Message);
        }

        #endregion
    }
}
