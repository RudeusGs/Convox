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

        /// <summary>
        /// Gửi tin nhắn vào Room với hỗ trợ Reply
        /// </summary>
        /// <param name="roomId">ID phòng</param>
        /// <param name="messageContent">Nội dung tin nhắn</param>
        /// <param name="imageUrls">Danh sách URL ảnh (optional)</param>
        /// <param name="replyToMessageId">ID tin nhắn được reply (optional)</param>
        public async Task SendMessageToRoom(int roomId, string messageContent, List<string>? imageUrls = null, int? replyToMessageId = null)
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
                ImageUrls = imageUrls ?? new List<string>(),
                ReplyToMessageId = replyToMessageId
            };

            var result = await _roomChatService.SendMessageWithImagesToRoom(model);

            if (result.IsSuccess)
                await Clients.Group($"room_{roomId}").SendAsync("ReceiveMessage", result.Data);
            else
                await Clients.Caller.SendAsync("Error", result.Message);
        }

        public async Task EditMessageInRoom(int messageId, int roomId, string newMessage, List<string>? imageUrls = null)
        {
            var userId = GetUserId();
            if (!userId.HasValue)
            {
                await Clients.Caller.SendAsync("Error", "Unauthorized");
                return;
            }

            var result = await _roomChatService.EditMessageInRoom(messageId, userId.Value, newMessage, imageUrls);

            if (result.IsSuccess)
                await Clients.Group($"room_{roomId}").SendAsync("MessageEdited", result.Data);
            else
                await Clients.Caller.SendAsync("Error", result.Message);
        }

        public async Task DeleteMessageInRoom(int messageId, int roomId)
        {
            var userId = GetUserId();
            if (!userId.HasValue)
            {
                await Clients.Caller.SendAsync("Error", "Unauthorized");
                return;
            }

            var result = await _roomChatService.DeleteMessageInRoom(messageId, userId.Value);

            if (result.IsSuccess)
                await Clients.Group($"room_{roomId}").SendAsync("MessageDeleted", new { MessageId = messageId });
            else
                await Clients.Caller.SendAsync("Error", result.Message);
        }
    }
}
