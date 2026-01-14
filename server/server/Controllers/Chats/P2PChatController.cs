using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using server.Models.Chats;
using server.Service.Interfaces;
using server.Service.Models.Chats;

namespace server.Controllers.Chats
{
    [Authorize]
    [Route("api/chat")]
    public class P2PChatController : ChatBaseController
    {
        private readonly IP2PChatService _p2pChatService;

        public P2PChatController(IP2PChatService p2pChatService)
        {
            _p2pChatService = p2pChatService;
        }

        [HttpGet("get-p2p-chat-history")]
        public async Task<IActionResult> GetP2PChatHistory(int otherUserId, [FromQuery] int page = 1, [FromQuery] int pageSize = 50)
        {
            var userId = GetUserId();
            if (!userId.HasValue)
                return FailResult("Unauthorized", 401, "UNAUTHORIZED");

            var result = await _p2pChatService.GetHistoryOfChatP2P(userId.Value, otherUserId, page, pageSize);
            return FromApiResult(result);
        }

        [HttpPost("send-message-p2p")]
        public async Task<IActionResult> SendMessageP2P(int receiverId, [FromBody] SendMessageRequest request)
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
            return FromApiResult(result);
        }
    }
}
