using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using server.Hubs;
using server.Infrastructure.Persistence;
using server.Models.Chats;
using server.Service.Interfaces;
using server.Service.Models.Chats;

namespace server.Controllers.Chats
{
    [Authorize]
    [ApiController]
    [Route("api/chat")]
    public class P2PChatController : ChatBaseController
    {
        private readonly IP2PChatService _p2pChatService;
        private readonly IHubContext<ChatHub> _hub;
        private readonly DataContext _dataContext;

        public P2PChatController(IP2PChatService p2pChatService, IHubContext<ChatHub> hub, DataContext dataContext)
        {
            _p2pChatService = p2pChatService;
            _hub = hub;
            _dataContext = dataContext;
        }

        [HttpGet("p2p/{otherUserId:int}/history")]
        public async Task<IActionResult> GetP2PChatHistory(
            [FromRoute] int otherUserId,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 50)
        {
            var userId = GetUserId();
            if (!userId.HasValue)
                return FailResult("Unauthorized", 401, "UNAUTHORIZED");

            var result = await _p2pChatService.GetHistoryOfChatP2P(userId.Value, otherUserId, page, pageSize);
            return FromApiResult(result);
        }

        [HttpPost("p2p/{receiverId:int}/messages")]
        public async Task<IActionResult> SendMessageP2P(
            [FromRoute] int receiverId,
            [FromBody] SendMessageRequest request)
        {
            var userId = GetUserId();
            if (!userId.HasValue)
                return FailResult("Unauthorized", 401, "UNAUTHORIZED");

            var model = new SendP2PMessageWithImagesModel
            {
                SenderId = userId.Value,
                ReceiverId = receiverId,
                MessageContent = request.MessageContent ?? string.Empty,
                ImageUrls = request.ImageUrls ?? new List<string>()
            };

            var result = await _p2pChatService.SendMessageWithImagesToP2P(model);
            if (!result.IsSuccess) return FromApiResult(result);

            await _hub.Clients.Group($"user_{receiverId}")
                .SendAsync("ReceiveP2PMessage", result.Data);

            await _hub.Clients.Group($"user_{userId.Value}")
                .SendAsync("ReceiveP2PMessage", result.Data);

            return FromApiResult(result);
        }

        [HttpPut("p2p/messages/{messageId:int}")]
        public async Task<IActionResult> EditP2PMessage(
            [FromRoute] int messageId,
            [FromBody] UpdateP2PMessageRequest request)
        {
            var userId = GetUserId();
            if (!userId.HasValue)
                return FailResult("Unauthorized", 401, "UNAUTHORIZED");

            var receiverId = await _dataContext.ChatP2Ps
                .Where(x => x.Id == messageId && x.DeletedDate == null)
                .Select(x => x.ReceiverId)
                .FirstOrDefaultAsync();

            if (receiverId == 0)
                return FailResult("Tin nhắn không tồn tại", 404, "MESSAGE_NOT_FOUND");

            var result = await _p2pChatService.EditMessageP2P(
                messageId,
                userId.Value,
                request.MessageContent ?? string.Empty,
                request.ImageUrls);

            if (!result.IsSuccess) return FromApiResult(result);
            await _hub.Clients.Group($"user_{receiverId}")
                .SendAsync("MessageEdited", result.Data);

            await _hub.Clients.Group($"user_{userId.Value}")
                .SendAsync("MessageEdited", result.Data);

            return FromApiResult(result);
        }

        [HttpDelete("p2p/messages/{messageId:int}")]
        public async Task<IActionResult> DeleteP2PMessage(
            [FromRoute] int messageId,
            [FromQuery] int receiverId)
        {
            var userId = GetUserId();
            if (!userId.HasValue)
                return FailResult("Unauthorized", 401, "UNAUTHORIZED");

            if (receiverId <= 0)
            {
                receiverId = await _dataContext.ChatP2Ps
                    .Where(x => x.Id == messageId && x.DeletedDate == null)
                    .Select(x => x.ReceiverId)
                    .FirstOrDefaultAsync();
            }

            var result = await _p2pChatService.DeleteMessageP2P(messageId, userId.Value);
            if (!result.IsSuccess) return FromApiResult(result);

            await _hub.Clients.Group($"user_{receiverId}")
                .SendAsync("MessageDeleted", new { MessageId = messageId });

            await _hub.Clients.Group($"user_{userId.Value}")
                .SendAsync("MessageDeleted", new { MessageId = messageId });

            return FromApiResult(result);
        }
    }
}
