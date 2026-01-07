using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using server.Domain.Entities;
using server.Infrastructure.Persistence;
using server.Service.Common.IServices;
using server.Service.Interfaces;
using server.Service.Models;

namespace server.Service.Services.Badges
{
    /// <summary>
    /// UserBadgeService: Quản lý huy hiệu của user với hệ thống mốc kinh nghiệm cố định.
    /// 
    /// Hệ thống mốc kinh nghiệm (Experience Milestones):
    /// - BadgeId 2: 0 điểm        (Bắt đầu)
    /// - BadgeId 3: 100 điểm      (Cấp độ 1)
    /// - BadgeId 4: 300 điểm      (Cấp độ 2)
    /// - BadgeId 5: 700 điểm      (Cấp độ 3)
    /// - BadgeId 6: 1500 điểm     (Cấp độ 4)
    /// - BadgeId 7: 3000 điểm     (Cấp độ 5)
    /// - BadgeId 8: 5000 điểm     (Cấp độ 6 - Max)
    /// 
    /// Logic:
    /// - Lần đầu tiên: Tạo UserBadge mốc đầu tiên (BadgeId 2) với Points = experiencePoints
    /// - Lần sau: Chỉ UPDATE Points (không thêm dòng mới, chỉ cập nhật giá trị)
    /// - Khi user đạt mốc kinh nghiệm, tự động unlock badge tương ứng
    /// </summary>
    public class UserBadgeService : BaseService, IUserBadgeService
    {
        private readonly ILogger<UserBadgeService> _logger;

        /// <summary>
        /// Bảng mốc kinh nghiệm: Điểm → BadgeId (bắt đầu từ BadgeId 2).
        /// Key: Điểm kinh nghiệm
        /// Value: BadgeId tương ứng
        /// </summary>
        private static readonly Dictionary<int, int> ExperienceMilestones = new()
        {
            { 0, 2 },       // BadgeId 2: 0 điểm (Bắt đầu)
            { 100, 3 },     // BadgeId 3: 100 điểm
            { 300, 4 },     // BadgeId 4: 300 điểm
            { 700, 5 },     // BadgeId 5: 700 điểm
            { 1500, 6 },    // BadgeId 6: 1500 điểm
            { 3000, 7 },    // BadgeId 7: 3000 điểm
            { 5000, 8 }     // BadgeId 8: 5000 điểm (Max)
        };

        public UserBadgeService(
            DataContext dataContext,
            IUserService userService,
            ILogger<UserBadgeService> logger)
            : base(dataContext, userService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Lấy danh sách badge của user cùng với thông tin điểm.
        /// </summary>
        public async Task<ApiResult> GetByUserId(int userId)
        {
            if (userId <= 0)
                return ApiResult.Fail("UserId không hợp lệ", "VALIDATION_ERROR");

            try
            {
                var result = await _dataContext.Set<UserBadge>()
                    .AsNoTracking()
                    .Where(x => x.UserId == userId)
                    .OrderByDescending(x => x.CreatedDate)
                    .ToListAsync();

                return ApiResult.Success(result, "Lấy danh sách huy hiệu của user thành công");
            }
            catch (Exception ex)
            {
                _logger.LogError($"GetByUserId error: {ex.Message}");
                return ApiResult.Fail(
                    "Lấy danh sách huy hiệu của user thất bại",
                    "SYSTEM_ERROR",
                    new[] { ex.Message }
                );
            }
        }

        /// <summary>
        /// Lấy danh sách user sở hữu badge cụ thể.
        /// </summary>
        public async Task<ApiResult> GetByBadgeId(int badgeId)
        {
            if (badgeId <= 0)
                return ApiResult.Fail("BadgeId không hợp lệ", "VALIDATION_ERROR");

            try
            {
                var result = await _dataContext.Set<UserBadge>()
                    .AsNoTracking()
                    .Where(x => x.BadgeId == badgeId)
                    .OrderByDescending(x => x.CreatedDate)
                    .ToListAsync();

                return ApiResult.Success(result, "Lấy danh sách user theo huy hiệu thành công");
            }
            catch (Exception ex)
            {
                _logger.LogError($"GetByBadgeId error: {ex.Message}");
                return ApiResult.Fail(
                    "Lấy danh sách user theo huy hiệu thất bại",
                    "SYSTEM_ERROR",
                    new[] { ex.Message }
                );
            }
        }

        /// <summary>
        /// Thêm điểm kinh nghiệm cho user và tự động unlock badge khi đạt mốc.
        /// 
        /// Quy trình:
        /// 1. Kiểm tra user đã có badge mốc đầu tiên (BadgeId 2) chưa
        ///    - Nếu chưa: INSERT 1 dòng mới
        ///    - Nếu rồi: UPDATE điểm (chỉ cập nhật value, không thêm dòng mới)
        /// 2. Kiểm tra mốc mới nào được unlock
        /// 3. Tự động gán badge mới (INSERT) nếu mốc mới được đạt
        /// </summary>
        public async Task<ApiResult> AddPoints(int userId, int experiencePoints)
        {
            if (userId <= 0 || experiencePoints <= 0)
                return ApiResult.Success(null, "Không có điểm được cộng");

            await using var tran = await _dataContext.Database.BeginTransactionAsync();
            try
            {
                var userBadge = await _dataContext.Set<UserBadge>()
                    .FirstOrDefaultAsync(x => x.UserId == userId);

                if (userBadge == null)
                {
                    userBadge = new UserBadge
                    {
                        UserId = userId,
                        BadgeId = 2,
                        Points = 0,
                        CreatedDate = Now
                    };
                    await _dataContext.Set<UserBadge>().AddAsync(userBadge);
                }

                userBadge.Points += experiencePoints;
                userBadge.UpdatedDate = Now;

                var totalPoints = userBadge.Points;

                var newBadgeId = ExperienceMilestones
                    .Where(x => totalPoints >= x.Key)
                    .OrderByDescending(x => x.Key)
                    .Select(x => x.Value)
                    .First();

                var unlocked = new List<int>();
                if (newBadgeId != userBadge.BadgeId)
                {
                    userBadge.BadgeId = newBadgeId;
                    unlocked.Add(newBadgeId);
                }

                await SaveChangesAsync();
                await tran.CommitAsync();

                return ApiResult.Success(
                    new { UserId = userId, TotalPoints = totalPoints, UnlockedBadges = unlocked },
                    unlocked.Count > 0
                        ? $"Cộng {experiencePoints} điểm thành công. Tổng: {totalPoints}. Badge hiện tại: {newBadgeId}"
                        : $"Cộng {experiencePoints} điểm thành công. Tổng: {totalPoints}"
                );
            }
            catch (Exception ex)
            {
                await tran.RollbackAsync();
                _logger.LogError(ex, "AddPoints error");
                return ApiResult.Fail("Cộng điểm thất bại", "INTERNAL_ERROR");
            }
        }



        /// <summary>
        /// Lấy tổng điểm kinh nghiệm của user (từ badge mốc bắt đầu BadgeId 2).
        /// </summary>
        public async Task<ApiResult> GetTotalPoints(int userId)
        {
            if (userId <= 0)
                return ApiResult.Fail("UserId không hợp lệ", "VALIDATION_ERROR");

            try
            {
                var startingBadge = await _dataContext.Set<UserBadge>()
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.UserId == userId && x.BadgeId == 2);

                var totalPoints = startingBadge?.Points ?? 0;
                var currentMilestone = GetCurrentMilestone(totalPoints);

                return ApiResult.Success(
                    new
                    {
                        UserId = userId,
                        TotalPoints = totalPoints,
                        CurrentMilestone = currentMilestone,
                        NextMilestonePoints = GetNextMilestoneThreshold(totalPoints)
                    },
                    "Lấy tổng điểm kinh nghiệm thành công"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError($"GetTotalPoints error: {ex.Message}");
                return ApiResult.Fail(
                    "Lấy tổng điểm thất bại",
                    "SYSTEM_ERROR",
                    new[] { ex.Message }
                );
            }
        }

        /// <summary>
        /// Gỡ badge khỏi user.
        /// </summary>
        public async Task<ApiResult> Remove(int userId, int badgeId)
        {
            if (userId <= 0)
                return ApiResult.Fail("UserId không hợp lệ", "VALIDATION_ERROR");

            if (badgeId <= 0)
                return ApiResult.Fail("BadgeId không hợp lệ", "VALIDATION_ERROR");

            await using var tran = await _dataContext.Database.BeginTransactionAsync();
            try
            {
                var userBadge = await _dataContext.Set<UserBadge>()
                    .FirstOrDefaultAsync(x => x.UserId == userId && x.BadgeId == badgeId);

                if (userBadge == null)
                    return ApiResult.Fail("User chưa có huy hiệu này", "USER_BADGE_NOT_FOUND");

                _dataContext.Set<UserBadge>().Remove(userBadge);
                await SaveChangesAsync();

                await tran.CommitAsync();

                _logger.LogInformation($"User {userId} badge {badgeId} removed");
                return ApiResult.Success(null, "Gỡ huy hiệu khỏi user thành công");
            }
            catch (Exception ex)
            {
                await tran.RollbackAsync();
                _logger.LogError($"Remove error: {ex.Message}");
                return ApiResult.Fail(
                    "Gỡ huy hiệu khỏi user thất bại",
                    "INTERNAL_ERROR",
                    new[] { ex.Message }
                );
            }
        }

        #region Helper Methods

        /// <summary>
        /// Lấy mốc kinh nghiệm hiện tại dựa trên tổng điểm.
        /// </summary>
        private string GetCurrentMilestone(int totalPoints)
        {
            var currentMilestone = ExperienceMilestones
                .Where(x => totalPoints >= x.Key)
                .OrderByDescending(x => x.Key)
                .FirstOrDefault();

            return currentMilestone.Key >= 0
                ? $"Mốc {currentMilestone.Key} điểm (BadgeId: {currentMilestone.Value})"
                : "Chưa đạt mốc nào";
        }

        /// <summary>
        /// Lấy mốc kinh nghiệm tiếp theo cần đạt.
        /// </summary>
        private int GetNextMilestoneThreshold(int totalPoints)
        {
            var nextMilestone = ExperienceMilestones
                .Where(x => x.Key > totalPoints)
                .OrderBy(x => x.Key)
                .FirstOrDefault();

            return nextMilestone.Key > 0 ? nextMilestone.Key : 5000;
        }

        #endregion
    }
}