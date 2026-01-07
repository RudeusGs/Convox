using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using server.Service.Interfaces;
using server.Service.Models.UserBadge;

namespace server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UserBadgeController : BaseController
    {
        private readonly IUserBadgeService _userBadgeService;

        public UserBadgeController(IUserBadgeService userBadgeService)
        {
            _userBadgeService = userBadgeService;
        }

        [HttpGet("get-my-badges")]
        public async Task<IActionResult> GetMyBadges([FromQuery] int userId)
        {
            var result = await _userBadgeService.GetByUserId(userId);
            return FromApiResult(result);
        }

        [HttpGet("get-by-badge")]
        public async Task<IActionResult> GetByBadgeId([FromQuery] int badgeId)
        {
            var result = await _userBadgeService.GetByBadgeId(badgeId);
            return FromApiResult(result);
        }

        [HttpGet("get-total-points")]
        public async Task<IActionResult> GetTotalPoints([FromQuery] int userId)
        {
            var result = await _userBadgeService.GetTotalPoints(userId);
            return FromApiResult(result);
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddPoints([FromBody] AddPointsRequestModel request)
        {
            var result = await _userBadgeService.AddPoints(
                request.UserId,
                request.ExperiencePoints
            );

            return FromApiResult(result);
        }

        [HttpDelete("remove")]
        public async Task<IActionResult> RemoveBadge(
            [FromQuery] int userId,
            [FromQuery] int badgeId)
        {
            var result = await _userBadgeService.Remove(userId, badgeId);
            return FromApiResult(result);
        }
    }
}
