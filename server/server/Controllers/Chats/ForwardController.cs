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
    public class ForwardController : ChatBaseController
    {
        private readonly IForwardMessageService _forwardMessageService;
        private readonly IHubContext<ChatHub> _hub;

        public ForwardController(IForwardMessageService forwardMessageService, IHubContext<ChatHub> hub)
        {
            _forwardMessageService = forwardMessageService;
            _hub = hub;
        }

   
        [HttpPost("forward/room")]
        public async Task<IActionResult> ForwardToRoom([FromBody] ForwardToRoomRequest request)
        {
            var userId = GetUserId();
            if (!userId.HasValue)
                return FailResult("Unauthorized", 401, "UNAUTHORIZED");

            var model = new ForwardToRoomModel
            {
                SourceMessageId = request.SourceMessageId,
                SourceType = request.SourceType,
                SourceId = request.SourceId,
                ForwarderId = userId.Value,
                TargetRoomId = request.TargetRoomId
            };

            var result = await _forwardMessageService.ForwardToRoom(model);
            if (!result.IsSuccess) return FromApiResult(result);

            await _hub.Clients.Group($"room_{request.TargetRoomId}")
                .SendAsync("ReceiveMessage", result.Data);

            return FromApiResult(result);
        }

       
        [HttpPost("forward/p2p")]
        public async Task<IActionResult> ForwardToP2P([FromBody] ForwardToP2PRequest request)
        {
            var userId = GetUserId();
            if (!userId.HasValue)
                return FailResult("Unauthorized", 401, "UNAUTHORIZED");

            var model = new ForwardToP2PModel
            {
                SourceMessageId = request.SourceMessageId,
                SourceType = request.SourceType,
                SourceId = request.SourceId,
                ForwarderId = userId.Value,
                TargetReceiverId = request.TargetReceiverId
            };

            var result = await _forwardMessageService.ForwardToP2P(model);
            if (!result.IsSuccess) return FromApiResult(result);

            await _hub.Clients.Group($"user_{request.TargetReceiverId}")
                .SendAsync("ReceiveP2PMessage", result.Data);

            await _hub.Clients.Group($"user_{userId.Value}")
                .SendAsync("ReceiveP2PMessage", result.Data);

            return FromApiResult(result);
        }

 
        [HttpPost("forward/breakroom")]
        public async Task<IActionResult> ForwardToBreakroom([FromBody] ForwardToBreakroomRequest request)
        {
            var userId = GetUserId();
            if (!userId.HasValue)
                return FailResult("Unauthorized", 401, "UNAUTHORIZED");

            var model = new ForwardToBreakroomModel
            {
                SourceMessageId = request.SourceMessageId,
                SourceType = request.SourceType,
                SourceId = request.SourceId,
                ForwarderId = userId.Value,
                TargetBreakroomId = request.TargetBreakroomId
            };

            var result = await _forwardMessageService.ForwardToBreakroom(model);
            if (!result.IsSuccess) return FromApiResult(result);

            await _hub.Clients.Group($"breakroom_{request.TargetBreakroomId}")
                .SendAsync("ReceiveMessage", result.Data);

            return FromApiResult(result);
        }
    }
}
