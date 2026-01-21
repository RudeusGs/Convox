using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using server.Hubs;
using server.Service.Interfaces;

namespace server.Controllers.Chats
{
    [Authorize]
    [ApiController]
    [Route("api/chat")]
    public class ReactionController : ChatBaseController
    {
        private readonly IReactionService _reactionService;
        private readonly IHubContext<ChatHub> _hub;

        public ReactionController(IReactionService reactionService, IHubContext<ChatHub> hub)
        {
            _reactionService = reactionService;
            _hub = hub;
        }

       

        [HttpPost("rooms/{roomId:int}/messages/{messageId:int}/reactions")]
        public async Task<IActionResult> ToggleReactionInRoom(
            [FromRoute] int roomId,
            [FromRoute] int messageId,
            [FromBody] ToggleReactionRequest request)
        {
            var userId = GetUserId();
            if (!userId.HasValue)
                return FailResult("Unauthorized", 401, "UNAUTHORIZED");

            var result = await _reactionService.ToggleReactionInRoom(messageId, userId.Value, request.Emoji);
            if (!result.IsSuccess) return FromApiResult(result);

            await _hub.Clients.Group($"room_{roomId}")
                .SendAsync("ReactionUpdated", result.Data);

            return FromApiResult(result);
        }

        [HttpGet("rooms/{roomId:int}/messages/{messageId:int}/reactions")]
        public async Task<IActionResult> GetReactionsInRoom(
            [FromRoute] int roomId,
            [FromRoute] int messageId)
        {
            var result = await _reactionService.GetReactionsForRoomMessage(messageId);
            return FromApiResult(result);
        }

        

   
        [HttpPost("breakrooms/{breakroomId:int}/messages/{messageId:int}/reactions")]
        public async Task<IActionResult> ToggleReactionInBreakroom(
            [FromRoute] int breakroomId,
            [FromRoute] int messageId,
            [FromBody] ToggleReactionRequest request)
        {
            var userId = GetUserId();
            if (!userId.HasValue)
                return FailResult("Unauthorized", 401, "UNAUTHORIZED");

            var result = await _reactionService.ToggleReactionInBreakroom(messageId, userId.Value, request.Emoji);
            if (!result.IsSuccess) return FromApiResult(result);

            await _hub.Clients.Group($"breakroom_{breakroomId}")
                .SendAsync("ReactionUpdated", result.Data);

            return FromApiResult(result);
        }

        /// <summary>
        /// L?y danh sách reactions c?a tin nh?n trong Breakroom
        /// </summary>
        [HttpGet("breakrooms/{breakroomId:int}/messages/{messageId:int}/reactions")]
        public async Task<IActionResult> GetReactionsInBreakroom(
            [FromRoute] int breakroomId,
            [FromRoute] int messageId)
        {
            var result = await _reactionService.GetReactionsForBreakroomMessage(messageId);
            return FromApiResult(result);
        }

       

   
        [HttpPost("p2p/messages/{messageId:int}/reactions")]
        public async Task<IActionResult> ToggleReactionP2P(
            [FromRoute] int messageId,
            [FromBody] ToggleReactionRequest request)
        {
            var userId = GetUserId();
            if (!userId.HasValue)
                return FailResult("Unauthorized", 401, "UNAUTHORIZED");

            var result = await _reactionService.ToggleReactionP2P(messageId, userId.Value, request.Emoji);
            if (!result.IsSuccess) return FromApiResult(result);

           
            await _hub.Clients.Group($"user_{userId.Value}")
                .SendAsync("ReactionUpdated", result.Data);

            return FromApiResult(result);
        }


        [HttpGet("p2p/messages/{messageId:int}/reactions")]
        public async Task<IActionResult> GetReactionsP2P([FromRoute] int messageId)
        {
            var result = await _reactionService.GetReactionsForP2PMessage(messageId);
            return FromApiResult(result);
        }

       
    }

    public class ToggleReactionRequest
    {
        public string Emoji { get; set; } = string.Empty;
    }
}
