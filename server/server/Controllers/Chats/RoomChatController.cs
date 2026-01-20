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
    public class RoomChatController : ChatBaseController
    {
        private readonly IRoomChatService _roomChatService;
        private readonly IHubContext<ChatHub> _hub;
        private readonly DataContext _dataContext;

        public RoomChatController(IRoomChatService roomChatService, IHubContext<ChatHub> hub, DataContext dataContext)
        {
            _roomChatService = roomChatService;
            _hub = hub;
            _dataContext = dataContext;
        }

        [HttpGet("rooms/{roomId:int}/history")]
        public async Task<IActionResult> GetRoomChatHistory(
            [FromRoute] int roomId,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 50)
        {
            var result = await _roomChatService.GetHistoryOfRoomChat(roomId, page, pageSize);
            return FromApiResult(result);
        }

        [HttpPost("rooms/{roomId:int}/messages")]
        public async Task<IActionResult> SendMessageToRoom(
            [FromRoute] int roomId,
            [FromBody] SendMessageRequest request)
        {
            var userId = GetUserId();
            if (!userId.HasValue)
                return FailResult("Unauthorized", 401, "UNAUTHORIZED");

            var model = new SendRoomMessageWithImagesModel
            {
                RoomId = roomId,
                SenderId = userId.Value,
                MessageContent = request.MessageContent ?? string.Empty,
                ImageUrls = request.ImageUrls ?? new List<string>()
            };

            var result = await _roomChatService.SendMessageWithImagesToRoom(model);
            if (!result.IsSuccess) return FromApiResult(result);

            await _hub.Clients.Group($"room_{roomId}")
                .SendAsync("ReceiveMessage", result.Data);

            return FromApiResult(result);
        }

        [HttpPut("rooms/{roomId:int}/messages/{messageId:int}")]
        public async Task<IActionResult> EditMessageInRoom(
            [FromRoute] int roomId,
            [FromRoute] int messageId,
            [FromBody] UpdateRoomMessageRequest request)
        {
            var userId = GetUserId();
            if (!userId.HasValue)
                return FailResult("Unauthorized", 401, "UNAUTHORIZED");

            var messageRoomId = await _dataContext.ChatMessages
                .Where(x => x.Id == messageId && x.DeletedDate == null)
                .Select(x => x.RoomId)
                .FirstOrDefaultAsync();

            if (messageRoomId == 0)
                return FailResult("Tin nhắn không tồn tại", 404, "MESSAGE_NOT_FOUND");

            if (messageRoomId != roomId)
                return FailResult("Tin nhắn không thuộc phòng này", 400, "INVALID_ROOM");

            var result = await _roomChatService.EditMessageInRoom(
                messageId,
                userId.Value,
                request.MessageContent ?? string.Empty,
                request.ImageUrls);

            if (!result.IsSuccess) return FromApiResult(result);

            await _hub.Clients.Group($"room_{roomId}")
                .SendAsync("MessageEdited", result.Data);

            return FromApiResult(result);
        }

        [HttpDelete("rooms/{roomId:int}/messages/{messageId:int}")]
        public async Task<IActionResult> DeleteMessageInRoom(
            [FromRoute] int roomId,
            [FromRoute] int messageId)
        {
            var userId = GetUserId();
            if (!userId.HasValue)
                return FailResult("Unauthorized", 401, "UNAUTHORIZED");

            var messageRoomId = await _dataContext.ChatMessages
                .Where(x => x.Id == messageId && x.DeletedDate == null)
                .Select(x => x.RoomId)
                .FirstOrDefaultAsync();

            if (messageRoomId == 0)
                return FailResult("Tin nhắn không tồn tại", 404, "MESSAGE_NOT_FOUND");

            if (messageRoomId != roomId)
                return FailResult("Tin nhắn không thuộc phòng này", 400, "INVALID_ROOM");

            var result = await _roomChatService.DeleteMessageInRoom(messageId, userId.Value);
            if (!result.IsSuccess) return FromApiResult(result);

            await _hub.Clients.Group($"room_{roomId}")
                .SendAsync("MessageDeleted", new { MessageId = messageId });

            return FromApiResult(result);
        }
    }
}
