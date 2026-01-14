using Microsoft.AspNetCore.SignalR;
using server.Service.Models.Chats;

namespace server.Hubs
{
    public partial class ChatHub
    {
        public async Task JoinRoom(int roomId)
        {
            var userId = GetUserId();
            if (!userId.HasValue)
            {
                await Clients.Caller.SendAsync("Error", "Unauthorized");
                return;
            }

            await Groups.AddToGroupAsync(Context.ConnectionId, $"room_{roomId}");
            await Clients.Group($"room_{roomId}").SendAsync("UserJoined", new { UserId = userId.Value, RoomId = roomId });
        }

        public async Task LeaveRoom(int roomId)
        {
            var userId = GetUserId();
            if (!userId.HasValue)
            {
                await Clients.Caller.SendAsync("Error", "Unauthorized");
                return;
            }

            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"room_{roomId}");
            await Clients.Group($"room_{roomId}").SendAsync("UserLeft", new { UserId = userId.Value, RoomId = roomId });
        }

        public async Task SendMessageToRoom(int roomId, string messageContent, List<string>? imageUrls = null)
        {
            var userId = GetUserId();
            if (!userId.HasValue)
            {
                await Clients.Caller.SendAsync("Error", "Unauthorized");
                return;
            }

            var model = new SendRoomMessageWithImagesModel
            {
                RoomId = roomId,
                SenderId = userId.Value,
                MessageContent = messageContent ?? string.Empty,
                ImageUrls = imageUrls ?? new List<string>()
            };

            var result = await _roomChatService.SendMessageWithImagesToRoom(model);

            if (result.IsSuccess)
                await Clients.Group($"room_{roomId}").SendAsync("ReceiveMessage", result.Data);
            else
                await Clients.Caller.SendAsync("Error", result.Message);
        }
    }
}
