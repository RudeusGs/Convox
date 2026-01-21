using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using server.Infrastructure.Persistence;
using server.Service.Interfaces;

namespace server.Hubs
{
    [Authorize]
    public partial class ChatHub : Hub
    {
        private readonly IRoomChatService _roomChatService;
        private readonly IBreakroomChatService _breakroomChatService;
        private readonly IP2PChatService _p2pChatService;
        private readonly IReactionService _reactionService;
        private readonly IPinnedMessageService _pinnedMessageService;
        private readonly IForwardMessageService _forwardMessageService;

        public ChatHub(
            IRoomChatService roomChatService,
            IBreakroomChatService breakroomChatService,
            IP2PChatService p2pChatService,
            IReactionService reactionService,
            IPinnedMessageService pinnedMessageService,
            IForwardMessageService forwardMessageService)
        {
            _roomChatService = roomChatService;
            _breakroomChatService = breakroomChatService;
            _p2pChatService = p2pChatService;
            _reactionService = reactionService;
            _pinnedMessageService = pinnedMessageService;
            _forwardMessageService = forwardMessageService;
        }

        public override async Task OnConnectedAsync()
        {
            var userId = GetUserId();
            if (userId.HasValue)
                await Groups.AddToGroupAsync(Context.ConnectionId, $"user_{userId.Value}");

            await base.OnConnectedAsync();
        }
            
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var userId = GetUserId();
            if (userId.HasValue)
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"user_{userId.Value}");

            await base.OnDisconnectedAsync(exception);
        }

        private int? GetUserId()
        {
            var userIdClaim = Context.User?.FindFirst("UserId")?.Value
                ?? Context.User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            return int.TryParse(userIdClaim, out var userId) ? userId : null;
        }

        private DataContext GetDataContext()
        {
            return (DataContext)Context.GetHttpContext()!.RequestServices.GetService(typeof(DataContext))!;
        }
    }
}
