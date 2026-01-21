using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using server.Infrastructure.Persistence;
using server.Service.Models.Chats;

namespace server.Hubs
{
    public partial class ChatHub
    {
       
        public async Task SendMessageP2P(int receiverId, string messageContent, List<string>? imageUrls = null, int? replyToMessageId = null)
        {
            var userId = GetUserId();
            if (!userId.HasValue)
            {
                await Clients.Caller.SendAsync("Error", "Unauthorized");
                return;
            }

            var model = new SendP2PMessageWithImagesModel
            {
                SenderId = userId.Value,
                ReceiverId = receiverId,
                MessageContent = messageContent ?? string.Empty,
                ImageUrls = imageUrls ?? new List<string>(),
                ReplyToMessageId = replyToMessageId
            };

            var result = await _p2pChatService.SendMessageWithImagesToP2P(model);

            if (result.IsSuccess)
            {
                await Clients.Group($"user_{receiverId}").SendAsync("ReceiveP2PMessage", result.Data);
                await Clients.Group($"user_{userId.Value}").SendAsync("ReceiveP2PMessage", result.Data);
            }
            else
            {
                await Clients.Caller.SendAsync("Error", result.Message);
            }
        }

        public async Task EditMessageP2P(int messageId, string newMessage, List<string>? imageUrls = null)
        {
            var userId = GetUserId();
            if (!userId.HasValue)
            {
                await Clients.Caller.SendAsync("Error", "Unauthorized");
                return;
            }

            var receiverId = await GetDataContext().ChatP2Ps
                .Where(x => x.Id == messageId && x.DeletedDate == null)
                .Select(x => x.ReceiverId)
                .FirstOrDefaultAsync();

            if (receiverId == 0)
            {
                await Clients.Caller.SendAsync("Error", "Tin nhắn không tồn tại");
                return;
            }

            var result = await _p2pChatService.EditMessageP2P(messageId, userId.Value, newMessage, imageUrls);

            if (result.IsSuccess)
            {
                await Clients.Group($"user_{receiverId}").SendAsync("MessageEdited", result.Data);
                await Clients.Group($"user_{userId.Value}").SendAsync("MessageEdited", result.Data);
            }
            else
            {
                await Clients.Caller.SendAsync("Error", result.Message);
            }
        }

        public async Task DeleteMessageP2P(int messageId, int receiverId)
        {
            var userId = GetUserId();
            if (!userId.HasValue)
            {
                await Clients.Caller.SendAsync("Error", "Unauthorized");
                return;
            }

            if (receiverId <= 0)
            {
                receiverId = await GetDataContext().ChatP2Ps
                    .Where(x => x.Id == messageId && x.DeletedDate == null)
                    .Select(x => x.ReceiverId)
                    .FirstOrDefaultAsync();
            }

            if (receiverId <= 0)
            {
                await Clients.Caller.SendAsync("Error", "Tin nhắn không tồn tại");
                return;
            }

            var result = await _p2pChatService.DeleteMessageP2P(messageId, userId.Value);

            if (result.IsSuccess)
            {
                await Clients.Group($"user_{receiverId}").SendAsync("MessageDeleted", new { MessageId = messageId });
                await Clients.Group($"user_{userId.Value}").SendAsync("MessageDeleted", new { MessageId = messageId });
            }
            else
            {
                await Clients.Caller.SendAsync("Error", result.Message);
            }
        }
    }
}
