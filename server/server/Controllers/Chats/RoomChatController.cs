using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using server.Models.Chats;
using server.Service.Interfaces;
using server.Service.Models.Chats;

namespace server.Controllers.Chats
{
    [Authorize]
    [Route("api/chat")]
    public class RoomChatController : ChatBaseController
    {
        private readonly IRoomChatService _roomChatService;

        public RoomChatController(IRoomChatService roomChatService)
        {
            _roomChatService = roomChatService;
        }

        [HttpGet("get-room-chat-history")]
        public async Task<IActionResult> GetRoomChatHistory(int roomId, [FromQuery] int page = 1, [FromQuery] int pageSize = 50)
        {
            var result = await _roomChatService.GetHistoryOfRoomChat(roomId, page, pageSize);
            return FromApiResult(result);
        }

        [HttpPost("send-message-to-room")]
        public async Task<IActionResult> SendMessageToRoom(int roomId, [FromBody] SendMessageRequest request)
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
            return FromApiResult(result);
        }
    }
}
