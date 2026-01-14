using Microsoft.AspNetCore.SignalR;

namespace server.Hubs
{
    public partial class ChatHub
    {
        public async Task StartTypingInRoom(int roomId)
        {
            var userId = GetUserId();
            if (!userId.HasValue) return;

            await Clients.OthersInGroup($"room_{roomId}").SendAsync("UserTyping", new
            {
                UserId = userId.Value,
                RoomId = roomId,
                IsTyping = true
            });
        }

        public async Task StopTypingInRoom(int roomId)
        {
            var userId = GetUserId();
            if (!userId.HasValue) return;

            await Clients.OthersInGroup($"room_{roomId}").SendAsync("UserTyping", new
            {
                UserId = userId.Value,
                RoomId = roomId,
                IsTyping = false
            });
        }

        public async Task StartTypingP2P(int receiverId)
        {
            var userId = GetUserId();
            if (!userId.HasValue) return;

            await Clients.Group($"user_{receiverId}").SendAsync("UserTypingP2P", new
            {
                SenderId = userId.Value,
                IsTyping = true
            });
        }

        public async Task StopTypingP2P(int receiverId)
        {
            var userId = GetUserId();
            if (!userId.HasValue) return;

            await Clients.Group($"user_{receiverId}").SendAsync("UserTypingP2P", new
            {
                SenderId = userId.Value,
                IsTyping = false
            });
        }
    }
}
