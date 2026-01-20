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
    public class BreakroomChatController : ChatBaseController
    {
        private readonly IBreakroomChatService _breakroomChatService;
        private readonly IHubContext<ChatHub> _hub;
        private readonly DataContext _dataContext;

        public BreakroomChatController(
            IBreakroomChatService breakroomChatService,
            IHubContext<ChatHub> hub,
            DataContext dataContext)
        {
            _breakroomChatService = breakroomChatService;
            _hub = hub;
            _dataContext = dataContext;
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

        [HttpPut("breakrooms/{breakroomId:int}/messages/{messageId:int}")]
        public async Task<IActionResult> EditMessageInBreakroom(
            [FromRoute] int breakroomId,
            [FromRoute] int messageId,
            [FromBody] UpdateBreakroomMessageRequest request)
        {
            var userId = GetUserId();
            if (!userId.HasValue)
                return FailResult("Unauthorized", 401, "UNAUTHORIZED");

            var messageBreakroomId = await _dataContext.ChatMessageBreakoutRooms
                .Where(x => x.Id == messageId && x.DeletedDate == null)
                .Select(x => x.BreakoutRoomId)
                .FirstOrDefaultAsync();

            if (messageBreakroomId == 0)
                return FailResult("Tin nhắn không tồn tại", 404, "MESSAGE_NOT_FOUND");

            if (messageBreakroomId != breakroomId)
                return FailResult("Tin nhắn không thuộc phòng con này", 400, "INVALID_BREAKROOM");

            var result = await _breakroomChatService.EditMessageInBreakroom(
                messageId,
                userId.Value,
                request.MessageContent ?? string.Empty,
                request.ImageUrls);

            if (!result.IsSuccess) return FromApiResult(result);

            await _hub.Clients.Group($"breakroom_{breakroomId}")
                .SendAsync("MessageEdited", result.Data);

            return FromApiResult(result);
        }

        [HttpDelete("breakrooms/{breakroomId:int}/messages/{messageId:int}")]
        public async Task<IActionResult> DeleteMessageInBreakroom(
            [FromRoute] int breakroomId,
            [FromRoute] int messageId)
        {
            var userId = GetUserId();
            if (!userId.HasValue)
                return FailResult("Unauthorized", 401, "UNAUTHORIZED");

            var messageBreakroomId = await _dataContext.ChatMessageBreakoutRooms
                .Where(x => x.Id == messageId && x.DeletedDate == null)
                .Select(x => x.BreakoutRoomId)
                .FirstOrDefaultAsync();

            if (messageBreakroomId == 0)
                return FailResult("Tin nhắn không tồn tại", 404, "MESSAGE_NOT_FOUND");

            if (messageBreakroomId != breakroomId)
                return FailResult("Tin nhắn không thuộc phòng con này", 400, "INVALID_BREAKROOM");

            var result = await _breakroomChatService.DeleteMessageInBreakroom(messageId, userId.Value);
            if (!result.IsSuccess) return FromApiResult(result);

            await _hub.Clients.Group($"breakroom_{breakroomId}")
                .SendAsync("MessageDeleted", new { MessageId = messageId });

            return FromApiResult(result);
        }
    }
}
