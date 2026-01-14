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
    public class BreakroomChatController : ChatBaseController
    {
        private readonly IBreakroomChatService _breakroomChatService;
        private readonly IHubContext<ChatHub> _hub;

        public BreakroomChatController(
            IBreakroomChatService breakroomChatService,
            IHubContext<ChatHub> hub)
        {
            _breakroomChatService = breakroomChatService;
            _hub = hub;
        }

        [HttpGet("breakrooms/{breakroomId:int}/history")]
        public async Task<IActionResult> GetBreakroomChatHistory(
            [FromRoute] int breakroomId,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 50)
        {
            var result = await _breakroomChatService.GetHistoryOfBreakroomChat(breakroomId, page, pageSize);
            return FromApiResult(result);
        }

        [HttpPost("breakrooms/{breakroomId:int}/messages")]
        public async Task<IActionResult> SendMessageToBreakroom(
            [FromRoute] int breakroomId,
            [FromBody] SendMessageRequest request)
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
            if (!result.IsSuccess) return FromApiResult(result);

            await _hub.Clients.Group($"breakroom_{breakroomId}")
                .SendAsync("ReceiveMessage", result.Data);

            return FromApiResult(result);
        }
    }
}
