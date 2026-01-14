using Microsoft.AspNetCore.SignalR;
using server.Service.Models.Chats;

namespace server.Hubs
{
    public partial class ChatHub
    {
        public async Task SendMessageP2P(int receiverId, string messageContent, List<string>? imageUrls = null)
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
                ImageUrls = imageUrls ?? new List<string>()
            };

            var result = await _p2pChatService.SendMessageWithImagesToP2P(model);

            if (result.IsSuccess)
            {
                await Clients.Group($"user_{receiverId}").SendAsync("ReceiveP2PMessage", result.Data);
                await Clients.Caller.SendAsync("ReceiveP2PMessage", result.Data);
            }
            else
            {
                await Clients.Caller.SendAsync("Error", result.Message);
            }
        }
    }
}
