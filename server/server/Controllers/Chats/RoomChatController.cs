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
    public class RoomChatController : ChatBaseController
    {
        private readonly IRoomChatService _roomChatService;
        private readonly IHubContext<ChatHub> _hub;

        public RoomChatController(IRoomChatService roomChatService, IHubContext<ChatHub> hub)
        {
            _roomChatService = roomChatService;
            _hub = hub;
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
    }
}
