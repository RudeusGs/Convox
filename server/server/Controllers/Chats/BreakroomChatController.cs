using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using server.Models.Chats;
using server.Service.Interfaces;
using server.Service.Models.Chats;

namespace server.Controllers.Chats
{
    [Authorize]
    [Route("api/chat")]
    public class BreakroomChatController : ChatBaseController
    {
        private readonly IBreakroomChatService _breakroomChatService;

        public BreakroomChatController(IBreakroomChatService breakroomChatService)
        {
            _breakroomChatService = breakroomChatService;
        }

        [HttpGet("get-breakroom-chat-history")]
        public async Task<IActionResult> GetBreakroomChatHistory(int breakroomId, [FromQuery] int page = 1, [FromQuery] int pageSize = 50)
        {
            var result = await _breakroomChatService.GetHistoryOfBreakroomChat(breakroomId, page, pageSize);
            return FromApiResult(result);
        }

        [HttpPost("send-message-to-break-room")]
        public async Task<IActionResult> SendMessageToBreakroom(int breakroomId, [FromBody] SendMessageRequest request)
        {
            var userId = GetUserId();
            if (!userId.HasValue)
                return FailResult("Unauthorized", 401, "UNAUTHORIZED");

            var model = new SendBreakroomMessageWithImagesModel
            {
                BreakroomId = breakroomId,
                SenderId = userId.Value,
                MessageContent = request.MessageContent ?? string.Empty,
                ImageUrls = request.ImageUrls ?? new List<string>()
            };

            var result = await _breakroomChatService.SendMessageWithImagesToBreakroom(model);
            return FromApiResult(result);
        }
    }
}
