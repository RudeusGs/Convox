using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using server.Hubs;
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

        public P2PChatController(IP2PChatService p2pChatService, IHubContext<ChatHub> hub)
        {
            _p2pChatService = p2pChatService;
            _hub = hub;
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
    }
}
