using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using server.Domain.Entities;
using server.Infrastructure.Persistence;
using server.Service.Common.IServices;
using server.Service.Interfaces;
using server.Service.Models;
using server.Service.Models.Badges;
using server.Service.Utilities;

namespace server.Service.Services.Badges
{
    public class BadgeService : BaseService, IBadgeService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<BadgeService> _logger;

        public BadgeService(
            DataContext dataContext,
            IUserService userService,
            IConfiguration configuration,
            ILogger<BadgeService> logger)
            : base(dataContext, userService)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<ApiResult> Add(AddBadgeModel modelBadge, CancellationToken cancellationToken = default)
        {
            if (modelBadge == null)
                return ApiResult.Fail("Dữ liệu không hợp lệ", "VALIDATION_ERROR");

            if (string.IsNullOrWhiteSpace(modelBadge.Name))
                return ApiResult.Fail("Tên badge không được để trống", "VALIDATION_ERROR");

            if (modelBadge.IconFile == null)
                return ApiResult.Fail("Icon badge không được để trống", "VALIDATION_ERROR");

            if (modelBadge.ExperiencePoints < 0)
                return ApiResult.Fail("Điểm kinh nghiệm không được âm", "VALIDATION_ERROR");

            var userId = _userService.UserId;
            if (userId <= 0)
                return ApiResult.Fail("Không xác định được người thực hiện", "UNAUTHORIZED");

            try
            {
                var existed = await _dataContext.Set<Badge>()
                    .AsNoTracking()
                    .AnyAsync(x => x.Name == modelBadge.Name.Trim(), cancellationToken);

                if (existed)
                    return ApiResult.Fail("Tên badge đã tồn tại", "BADGE_ALREADY_EXISTS");
                string iconUrl;
                try
                {
                    iconUrl = await ImgBBUploadHelper.UploadImageAsync(
                        modelBadge.IconFile,
                        _configuration,
                        cancellationToken);

                    _logger.LogInformation($"Icon uploaded successfully: {iconUrl}");
                }
                catch (Exception uploadEx)
                {
                    _logger.LogError($"Upload error: {uploadEx.Message}");
                    return ApiResult.Fail($"Upload icon thất bại: {uploadEx.Message}", "UPLOAD_ERROR");
                }

                var badge = new Badge
                {
                    Name = modelBadge.Name.Trim(),
                    Icon = iconUrl,
                    Description = string.IsNullOrWhiteSpace(modelBadge.Description)
                        ? null
                        : modelBadge.Description.Trim(),
                    ExperiencePoints = modelBadge.ExperiencePoints,
                    CreatedDate = Now,
                };

                _logger.LogInformation(
                    $"Creating badge: Name={badge.Name}, ExperiencePoints={badge.ExperiencePoints}, Icon={badge.Icon}");

                await _dataContext.Set<Badge>().AddAsync(badge, cancellationToken);
                await SaveChangesAsync(cancellationToken);

                _logger.LogInformation($"Badge created successfully with ID: {badge.Id}");
                return ApiResult.Success(badge, "Tạo badge thành công");
            }
            catch (DbUpdateException dbEx)
            {
                _logger.LogError($"Database error: {dbEx.InnerException?.Message}");
                return ApiResult.Fail(
                    "Lỗi lưu vào database",
                    "DATABASE_ERROR",
                    new[] { dbEx.InnerException?.Message ?? dbEx.Message }
                );
            }
            catch (Exception ex)
            {
                _logger.LogError($"Unexpected error: {ex.Message}\n{ex.StackTrace}");
                return ApiResult.Fail(
                    "Tạo badge thất bại",
                    "INTERNAL_ERROR",
                    new[] { ex.Message }
                );
            }
        }

        public async Task<ApiResult> Update(UpdateBadgeModel modelBadge, CancellationToken cancellationToken = default)
        {
            if (modelBadge == null || modelBadge.Id <= 0)
                return ApiResult.Fail("Dữ liệu không hợp lệ", "VALIDATION_ERROR");

            if (string.IsNullOrWhiteSpace(modelBadge.Name))
                return ApiResult.Fail("Tên badge không được để trống", "VALIDATION_ERROR");

            if (modelBadge.ExperiencePoints < 0)
                return ApiResult.Fail("Điểm kinh nghiệm không được âm", "VALIDATION_ERROR");

            var userId = _userService.UserId;
            if (userId <= 0)
                return ApiResult.Fail("Không xác định được người thực hiện", "UNAUTHORIZED");

            await using var tran = await _dataContext.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                var badge = await _dataContext.Set<Badge>()
                    .FirstOrDefaultAsync(x => x.Id == modelBadge.Id, cancellationToken);

                if (badge == null)
                    return ApiResult.Fail("Không tìm thấy badge", "BADGE_NOT_FOUND");

                var existed = await _dataContext.Set<Badge>()
                    .AsNoTracking()
                    .AnyAsync(
                        x => x.Id != modelBadge.Id && x.Name == modelBadge.Name.Trim(),
                        cancellationToken);

                if (existed)
                    return ApiResult.Fail("Tên badge đã tồn tại", "BADGE_ALREADY_EXISTS");

                badge.Name = modelBadge.Name.Trim();
                badge.Description = modelBadge.Description?.Trim();
                badge.ExperiencePoints = modelBadge.ExperiencePoints;
                if (modelBadge.IconFile != null)
                {
                    try
                    {
                        var iconUrl = await ImgBBUploadHelper.UploadImageAsync(
                            modelBadge.IconFile,
                            _configuration,
                            cancellationToken);

                        badge.Icon = iconUrl;
                        _logger.LogInformation($"Icon updated: {iconUrl}");
                    }
                    catch (Exception uploadEx)
                    {
                        await tran.RollbackAsync(cancellationToken);
                        _logger.LogError($"Upload error during update: {uploadEx.Message}");
                        return ApiResult.Fail($"Upload icon thất bại: {uploadEx.Message}", "UPLOAD_ERROR");
                    }
                }

                badge.UpdatedDate = Now;

                await SaveChangesAsync(cancellationToken);
                await tran.CommitAsync(cancellationToken);

                _logger.LogInformation($"Badge updated successfully with ID: {badge.Id}");
                return ApiResult.Success(badge, "Cập nhật badge thành công");
            }
            catch (DbUpdateException dbEx)
            {
                await tran.RollbackAsync(cancellationToken);
                _logger.LogError($"Database error: {dbEx.InnerException?.Message}");
                return ApiResult.Fail(
                    "Lỗi lưu vào database",
                    "DATABASE_ERROR",
                    new[] { dbEx.InnerException?.Message ?? dbEx.Message }
                );
            }
            catch (Exception ex)
            {
                await tran.RollbackAsync(cancellationToken);
                _logger.LogError($"Unexpected error: {ex.Message}\n{ex.StackTrace}");
                return ApiResult.Fail(
                    "Cập nhật badge thất bại",
                    "INTERNAL_ERROR",
                    new[] { ex.Message }
                );
            }
        }

        public async Task<ApiResult> GetAll()
        {
            try
            {
                var result = await _dataContext.Set<Badge>()
                    .AsNoTracking()
                    .OrderByDescending(x => x.CreatedDate)
                    .ToListAsync();

                return ApiResult.Success(result, "Lấy tất cả badge thành công");
            }
            catch (Exception ex)
            {
                _logger.LogError($"GetAll error: {ex.Message}");
                return ApiResult.Fail(
                    message: "Lấy danh sách badge thất bại",
                    errorCode: "SYSTEM_ERROR",
                    errors: new[] { ex.Message }
                );
            }
        }

        public async Task<ApiResult> GetById(int id)
        {
            if (id <= 0)
                return ApiResult.Fail("Id không hợp lệ", "VALIDATION_ERROR");

            try
            {
                var badge = await _dataContext.Set<Badge>()
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.Id == id);

                if (badge == null)
                    return ApiResult.Fail("Không tìm thấy badge", "BADGE_NOT_FOUND");

                return ApiResult.Success(badge, "Lấy badge thành công");
            }
            catch (Exception ex)
            {
                _logger.LogError($"GetById error: {ex.Message}");
                return ApiResult.Fail("Lấy badge thất bại", "SYSTEM_ERROR", new[] { ex.Message });
            }
        }

        public async Task<ApiResult> Delete(int id)
        {
            if (id <= 0)
                return ApiResult.Fail("Id không hợp lệ", "VALIDATION_ERROR");

            await using var tran = await _dataContext.Database.BeginTransactionAsync();
            try
            {
                var badge = await _dataContext.Set<Badge>()
                    .FirstOrDefaultAsync(x => x.Id == id);

                if (badge == null)
                    return ApiResult.Fail("Không tìm thấy badge", "BADGE_NOT_FOUND");

                _dataContext.Set<Badge>().Remove(badge);
                await SaveChangesAsync();

                await tran.CommitAsync();

                _logger.LogInformation($"Badge deleted successfully with ID: {id}");
                return ApiResult.Success(null, "Xóa badge thành công");
            }
            catch (Exception ex)
            {
                await tran.RollbackAsync();
                _logger.LogError($"Delete error: {ex.Message}");
                return ApiResult.Fail(
                    "Xóa badge thất bại",
                    "INTERNAL_ERROR",
                    new[] { ex.Message }
                );
            }
        }
    }
}