using Microsoft.AspNetCore.SignalR;
using server.Service.Models.Chats;

namespace server.Hubs
{
    public partial class ChatHub
    {
        public async Task JoinBreakroom(int breakroomId)
        {
            var userId = GetUserId();
            if (!userId.HasValue)
            {
                await Clients.Caller.SendAsync("Error", "Unauthorized");
                return;
            }

            await Groups.AddToGroupAsync(Context.ConnectionId, $"breakroom_{breakroomId}");
            await Clients.Group($"breakroom_{breakroomId}").SendAsync("UserJoined", new { UserId = userId.Value, BreakroomId = breakroomId });
        }

        public async Task LeaveBreakroom(int breakroomId)
        {
            var userId = GetUserId();
            if (!userId.HasValue)
            {
                await Clients.Caller.SendAsync("Error", "Unauthorized");
                return;
            }

            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"breakroom_{breakroomId}");
            await Clients.Group($"breakroom_{breakroomId}").SendAsync("UserLeft", new { UserId = userId.Value, BreakroomId = breakroomId });
        }

        public async Task SendMessageToBreakroom(int breakroomId, string messageContent, List<string>? imageUrls = null)
        {
            var userId = GetUserId();
            if (!userId.HasValue)
            {
                await Clients.Caller.SendAsync("Error", "Unauthorized");
                return;
            }

            var model = new SendBreakroomMessageWithImagesModel
            {
                BreakroomId = breakroomId,
                SenderId = userId.Value,
                MessageContent = messageContent ?? string.Empty,
                ImageUrls = imageUrls ?? new List<string>()
            };

            var result = await _breakroomChatService.SendMessageWithImagesToBreakroom(model);

            if (result.IsSuccess)
                await Clients.Group($"breakroom_{breakroomId}").SendAsync("ReceiveMessage", result.Data);
            else
                await Clients.Caller.SendAsync("Error", result.Message);
        }

        public async Task EditMessageInBreakroom(int messageId, int breakroomId, string newMessage, List<string>? imageUrls = null)
        {
            var userId = GetUserId();
            if (!userId.HasValue)
            {
                await Clients.Caller.SendAsync("Error", "Unauthorized");
                return;
            }

            var result = await _breakroomChatService.EditMessageInBreakroom(messageId, userId.Value, newMessage, imageUrls);

            if (result.IsSuccess)
                await Clients.Group($"breakroom_{breakroomId}").SendAsync("MessageEdited", result.Data);
            else
                await Clients.Caller.SendAsync("Error", result.Message);
        }

        public async Task DeleteMessageInBreakroom(int messageId, int breakroomId)
        {
            var userId = GetUserId();
            if (!userId.HasValue)
            {
                await Clients.Caller.SendAsync("Error", "Unauthorized");
                return;
            }

            var result = await _breakroomChatService.DeleteMessageInBreakroom(messageId, userId.Value);

            if (result.IsSuccess)
                await Clients.Group($"breakroom_{breakroomId}").SendAsync("MessageDeleted", new { MessageId = messageId });
            else
                await Clients.Caller.SendAsync("Error", result.Message);
        }
    }
}
