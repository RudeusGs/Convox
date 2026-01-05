using Microsoft.EntityFrameworkCore;
using server.Domain.Entities;
using server.Infrastructure.Persistence;
using server.Service.Common.IServices;
using server.Service.Interfaces;
using server.Service.Models;

namespace server.Service.Services
{
    /// <summary>
    /// UserBadgeService: Quản lý badge của user với hệ thống điểm và tier.
    /// 
    /// Hệ thống Tier:
    /// - Newbie (1)    : 0 - 99 điểm
    /// - Bronze (2)    : 100 - 249 điểm
    /// - Silver (3)    : 250 - 449 điểm
    /// - Gold (4)      : 450 - 699 điểm
    /// - Platinum (5)  : 700 - 999 điểm
    /// - Diamond (6)   : 1000 - 1499 điểm
    /// - Legend (7)    : 1500+ điểm
    /// </summary>
    public class UserBadgeService : BaseService, IUserBadgeService
    {
        /// <summary>
        /// Bảng chuyển đổi điểm để nâng tier.
        /// Key: Tier (1-7), Value: Điểm cần thiết để đạt tier đó
        /// </summary>
        private static readonly Dictionary<int, int> TierThresholds = new()
        {
            { 1, 0 },       // Newbie
            { 2, 100 },     // Bronze
            { 3, 250 },     // Silver
            { 4, 450 },     // Gold
            { 5, 700 },     // Platinum
            { 6, 1000 },    // Diamond
            { 7, 1500 }     // Legend
        };

        public UserBadgeService(DataContext dataContext, IUserService userService)
            : base(dataContext, userService)
        {
        }

        /// <summary>
        /// Lấy danh sách badge của user kèm tier hiện tại.
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

                return ApiResult.Success(result, "Lấy danh sách badge của user thành công");
            }
            catch (Exception ex)
            {
                return ApiResult.Fail(
                    "Lấy danh sách badge của user thất bại",
                    "SYSTEM_ERROR",
                    new[] { ex.Message }
                );
            }
        }

        /// <summary>
        /// Lấy danh sách user có badge cụ thể.
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

                return ApiResult.Success(result, "Lấy danh sách user theo badge thành công");
            }
            catch (Exception ex)
            {
                return ApiResult.Fail(
                    "Lấy danh sách user theo badge thất bại",
                    "SYSTEM_ERROR",
                    new[] { ex.Message }
                );
            }
        }

        /// <summary>
        /// Gán badge mới cho user (khởi tạo với tier Newbie).
        /// </summary>
        public async Task<ApiResult> Add(int userId, int badgeId)
        {
            if (userId <= 0)
                return ApiResult.Fail("UserId không hợp lệ", "VALIDATION_ERROR");

            if (badgeId <= 0)
                return ApiResult.Fail("BadgeId không hợp lệ", "VALIDATION_ERROR");

            await using var tran = await _dataContext.Database.BeginTransactionAsync();
            try
            {
                var badgeExists = await _dataContext.Set<Badge>()
                    .AsNoTracking()
                    .AnyAsync(x => x.Id == badgeId);

                if (!badgeExists)
                    return ApiResult.Fail("Không tìm thấy badge", "BADGE_NOT_FOUND");

                var existed = await _dataContext.Set<UserBadge>()
                    .AnyAsync(x => x.UserId == userId && x.BadgeId == badgeId);

                if (existed)
                    return ApiResult.Fail("User đã có badge này", "USER_BADGE_ALREADY_EXISTS");

                var userBadge = new UserBadge
                {
                    UserId = userId,
                    BadgeId = badgeId,
                    Points = 0,
                    CurrentTier = 1,
                    CreatedDate = Now
                };

                await _dataContext.Set<UserBadge>().AddAsync(userBadge);
                await SaveChangesAsync();

                await tran.CommitAsync();
                return ApiResult.Success(userBadge, "Gán badge cho user thành công");
            }
            catch (Exception ex)
            {
                await tran.RollbackAsync();
                return ApiResult.Fail(
                    "Gán badge cho user thất bại",
                    "INTERNAL_ERROR",
                    new[] { ex.Message }
                );
            }
        }

        /// <summary>
        /// Cộng điểm vào badge của user và tự động nâng tier nếu đạt ngưỡng.
        /// </summary>
        public async Task<ApiResult> AddPoints(int userId, int badgeId, int points)
        {
            if (userId <= 0)
                return ApiResult.Fail("UserId không hợp lệ", "VALIDATION_ERROR");

            if (badgeId <= 0)
                return ApiResult.Fail("BadgeId không hợp lệ", "VALIDATION_ERROR");

            if (points < 0)
                return ApiResult.Fail("Điểm không thể âm", "VALIDATION_ERROR");

            await using var tran = await _dataContext.Database.BeginTransactionAsync();
            try
            {
                var userBadge = await _dataContext.Set<UserBadge>()
                    .FirstOrDefaultAsync(x => x.UserId == userId && x.BadgeId == badgeId);

                if (userBadge == null)
                    return ApiResult.Fail("User chưa có badge này", "USER_BADGE_NOT_FOUND");

                userBadge.Points += points;

                var newTier = GetTierByPoints(userBadge.Points);
                if (newTier > userBadge.CurrentTier)
                {
                    userBadge.CurrentTier = newTier;
                }

                userBadge.UpdatedDate = Now;

                await SaveChangesAsync();
                await tran.CommitAsync();

                return ApiResult.Success(
                    userBadge,
                    $"Cộng {points} điểm thành công. Tier hiện tại: {GetTierName(userBadge.CurrentTier)}"
                );
            }
            catch (Exception ex)
            {
                await tran.RollbackAsync();
                return ApiResult.Fail(
                    "Cộng điểm thất bại",
                    "INTERNAL_ERROR",
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
                    return ApiResult.Fail("User chưa có badge này", "USER_BADGE_NOT_FOUND");

                _dataContext.Set<UserBadge>().Remove(userBadge);
                await SaveChangesAsync();

                await tran.CommitAsync();
                return ApiResult.Success(null, "Gỡ badge khỏi user thành công");
            }
            catch (Exception ex)
            {
                await tran.RollbackAsync();
                return ApiResult.Fail(
                    "Gỡ badge khỏi user thất bại",
                    "INTERNAL_ERROR",
                    new[] { ex.Message }
                );
            }
        }

        /// <summary>
        /// Xác định tier dựa trên tổng điểm.
        /// </summary>
        private int GetTierByPoints(int points)
        {
            foreach (var (tier, threshold) in TierThresholds.OrderByDescending(x => x.Key))
            {
                if (points >= threshold)
                    return tier;
            }

            return 1; // Mặc định Newbie
        }

        /// <summary>
        /// Lấy tên tier từ số hiệu.
        /// </summary>
        private string GetTierName(int tier) => tier switch
        {
            1 => "Newbie",
            2 => "Bronze",
            3 => "Silver",
            4 => "Gold",
            5 => "Platinum",
            6 => "Diamond",
            7 => "Legend",
            _ => "Unknown"
        };
    }
}