using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using server.Service.Interfaces;

namespace server.Controllers
{
    /// <summary>
    /// UserBadgeController: API cho user tự bấm nhận badge.
    /// </summary>
    public class UserBadgeController : BaseController
    {
        private readonly IUserBadgeService _userBadgeService;

        public UserBadgeController(IUserBadgeService userBadgeService)
        {
            _userBadgeService = userBadgeService;
        }

        /// <summary>
        /// Lấy danh sách badge của user (profile).
        /// GET: /api/userbadge/my-badges
        /// </summary>
        [Authorize]
        [HttpGet("my-badges")]
        public async Task<IActionResult> GetMyBadges([FromQuery] int userId)
            => FromApiResult(await _userBadgeService.GetByUserId(userId));

        /// <summary>
        /// User nhận badge (bấm nút nhận huy hiệu).
        /// POST: /api/userbadge/claim
        /// Body: { "badgeId": 2 }
        /// </summary>
        [Authorize]
        [HttpPost("claim")]
        public async Task<IActionResult> ClaimBadge([FromQuery] int userId, [FromQuery] int badgeId)
        {
            if (userId <= 0 || badgeId <= 0)
                return FailResult("UserId và BadgeId không hợp lệ", StatusCodes.Status400BadRequest);

            var result = await _userBadgeService.Add(userId, badgeId);
            return FromApiResult(result, StatusCodes.Status201Created);
        }

        /// <summary>
        /// Cộng điểm vào badge (admin hoặc system gọi).
        /// POST: /api/userbadge/add-points
        /// Body: { "userId": 1, "badgeId": 2, "points": 50 }
        /// </summary>
        [Authorize]
        [HttpPost("add-points")]
        public async Task<IActionResult> AddPoints([FromQuery] int userId, [FromQuery] int badgeId, [FromQuery] int points)
        {
            if (points < 0)
                return FailResult("Điểm không thể âm", StatusCodes.Status400BadRequest);

            var result = await _userBadgeService.AddPoints(userId, badgeId, points);
            return FromApiResult(result);
        }

        /// <summary>
        /// Gỡ badge khỏi user.
        /// DELETE: /api/userbadge/remove?userId=1&badgeId=2
        /// </summary>
        [Authorize]
        [HttpDelete("remove")]
        public async Task<IActionResult> RemoveBadge([FromQuery] int userId, [FromQuery] int badgeId)
            => FromApiResult(await _userBadgeService.Remove(userId, badgeId));
    }
}