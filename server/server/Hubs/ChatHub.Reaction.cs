using Microsoft.AspNetCore.SignalR;

namespace server.Hubs
{
    public partial class ChatHub
    {
        #region Room Reactions

        public async Task ToggleReactionInRoom(int roomId, int messageId, string emoji)
        {
            var userId = GetUserId();
            if (!userId.HasValue)
            {
                await Clients.Caller.SendAsync("Error", "Unauthorized");
                return;
            }

            var result = await _reactionService.ToggleReactionInRoom(messageId, userId.Value, emoji);

            if (result.IsSuccess)
                await Clients.Group($"room_{roomId}").SendAsync("ReactionUpdated", result.Data);
            else
                await Clients.Caller.SendAsync("Error", result.Message);
        }

        public async Task GetReactionsInRoom(int messageId)
        {
            var userId = GetUserId();
            if (!userId.HasValue)
            {
                await Clients.Caller.SendAsync("Error", "Unauthorized");
                return;
            }

            var result = await _reactionService.GetReactionsForRoomMessage(messageId);

            if (result.IsSuccess)
                await Clients.Caller.SendAsync("ReactionsLoaded", result.Data);
            else
                await Clients.Caller.SendAsync("Error", result.Message);
        }

        #endregion

        #region Breakroom Reactions

        public async Task ToggleReactionInBreakroom(int breakroomId, int messageId, string emoji)
        {
            var userId = GetUserId();
            if (!userId.HasValue)
            {
                await Clients.Caller.SendAsync("Error", "Unauthorized");
                return;
            }

            var result = await _reactionService.ToggleReactionInBreakroom(messageId, userId.Value, emoji);

            if (result.IsSuccess)
                await Clients.Group($"breakroom_{breakroomId}").SendAsync("ReactionUpdated", result.Data);
            else
                await Clients.Caller.SendAsync("Error", result.Message);
        }

        public async Task GetReactionsInBreakroom(int messageId)
        {
            var userId = GetUserId();
            if (!userId.HasValue)
            {
                await Clients.Caller.SendAsync("Error", "Unauthorized");
                return;
            }

            var result = await _reactionService.GetReactionsForBreakroomMessage(messageId);

            if (result.IsSuccess)
                await Clients.Caller.SendAsync("ReactionsLoaded", result.Data);
            else
                await Clients.Caller.SendAsync("Error", result.Message);
        }

        #endregion

        #region P2P Reactions

        public async Task ToggleReactionP2P(int messageId, string emoji)
        {
            var userId = GetUserId();
            if (!userId.HasValue)
            {
                await Clients.Caller.SendAsync("Error", "Unauthorized");
                return;
            }

            var result = await _reactionService.ToggleReactionP2P(messageId, userId.Value, emoji);

            if (result.IsSuccess)
            {
                // Get receiver from message
                var receiverId = await GetP2PMessageReceiverId(messageId);
                if (receiverId > 0)
                {
                    await Clients.Group($"user_{receiverId}").SendAsync("ReactionUpdated", result.Data);
                    await Clients.Group($"user_{userId.Value}").SendAsync("ReactionUpdated", result.Data);
                }
            }
            else
            {
                await Clients.Caller.SendAsync("Error", result.Message);
            }
        }

        public async Task GetReactionsP2P(int messageId)
        {
            var userId = GetUserId();
            if (!userId.HasValue)
            {
                await Clients.Caller.SendAsync("Error", "Unauthorized");
                return;
            }

            var result = await _reactionService.GetReactionsForP2PMessage(messageId);

            if (result.IsSuccess)
                await Clients.Caller.SendAsync("ReactionsLoaded", result.Data);
            else
                await Clients.Caller.SendAsync("Error", result.Message);
        }

        private async Task<int> GetP2PMessageReceiverId(int messageId)
        {
            var dataContext = GetDataContext();
            var message = await dataContext.ChatP2Ps.FindAsync(messageId);
            return message?.ReceiverId ?? 0;
        }

        #endregion
    }
}
