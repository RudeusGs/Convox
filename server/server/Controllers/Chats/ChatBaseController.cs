using System.Security.Claims;

namespace server.Controllers.Chats
{
    public abstract class ChatBaseController : BaseController
    {
        protected int? GetUserId()
        {
            var userIdClaim = User.FindFirst("UserId")?.Value
                ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            return int.TryParse(userIdClaim, out var userId) ? userId : null;
        }
    }
}
