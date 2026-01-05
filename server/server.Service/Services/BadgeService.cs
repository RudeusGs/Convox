using Microsoft.EntityFrameworkCore;
using server.Domain.Entities;
using server.Infrastructure.Persistence;
using server.Service.Common.IServices;
using server.Service.Interfaces;
using server.Service.Models;
using server.Service.Models.Badge;

namespace server.Service.Services
{
    public class BadgeService : BaseService, IBadgeService
    {
        public BadgeService(DataContext dataContext, IUserService userService)
            : base(dataContext, userService)
        {
        }

        public async Task<ApiResult> Add(AddBadgeModel modelBadge)
        {
            if (modelBadge == null)
                return ApiResult.Fail("Dữ liệu không hợp lệ", "VALIDATION_ERROR");

            if (string.IsNullOrWhiteSpace(modelBadge.Name))
                return ApiResult.Fail("Tên badge không được để trống", "VALIDATION_ERROR");

            if (string.IsNullOrWhiteSpace(modelBadge.Icon))
                return ApiResult.Fail("Icon badge không được để trống", "VALIDATION_ERROR");

            var userId = _userService.UserId;
            if (userId <= 0)
                return ApiResult.Fail("Không xác định được người thực hiện", "UNAUTHORIZED");

            try
            {
                var existed = await _dataContext.Set<Badge>()
                    .AsNoTracking()
                    .AnyAsync(x => x.Name == modelBadge.Name.Trim());

                if (existed)
                    return ApiResult.Fail("Tên badge đã tồn tại", "BADGE_ALREADY_EXISTS");

                var badge = new Badge
                {
                    Name = modelBadge.Name.Trim(),
                    Icon = modelBadge.Icon.Trim(),
                    Description = string.IsNullOrWhiteSpace(modelBadge.Description)
                        ? null
                        : modelBadge.Description.Trim(),
                    CreatedDate = Now,
                };

                await _dataContext.Set<Badge>().AddAsync(badge);
                await SaveChangesAsync();

                return ApiResult.Success(badge, "Tạo badge thành công");
            }
            catch
            {
                return ApiResult.Fail("Tạo badge thất bại", "INTERNAL_ERROR");
            }
        }

        public async Task<ApiResult> Update(UpdateBadgeModel modelBadge)
        {
            if (modelBadge == null || modelBadge.Id <= 0)
                return ApiResult.Fail("Dữ liệu không hợp lệ", "VALIDATION_ERROR");

            if (string.IsNullOrWhiteSpace(modelBadge.Name))
                return ApiResult.Fail("Tên badge không được để trống", "VALIDATION_ERROR");

            if (string.IsNullOrWhiteSpace(modelBadge.Icon))
                return ApiResult.Fail("Icon badge không được để trống", "VALIDATION_ERROR");

            var userId = _userService.UserId;
            if (userId <= 0)
                return ApiResult.Fail("Không xác định được người thực hiện", "UNAUTHORIZED");

            await using var tran = await _dataContext.Database.BeginTransactionAsync();
            try
            {
                var badge = await _dataContext.Set<Badge>()
                    .FirstOrDefaultAsync(x => x.Id == modelBadge.Id);

                if (badge == null)
                    return ApiResult.Fail("Không tìm thấy badge", "BADGE_NOT_FOUND");

                var existed = await _dataContext.Set<Badge>()
                    .AsNoTracking()
                    .AnyAsync(x => x.Id != modelBadge.Id && x.Name == modelBadge.Name.Trim());

                if (existed)
                    return ApiResult.Fail("Tên badge đã tồn tại", "BADGE_ALREADY_EXISTS");

                badge.Name = modelBadge.Name.Trim();
                badge.Icon = modelBadge.Icon.Trim();
                badge.Description = modelBadge.Description?.Trim();
                badge.UpdatedDate = Now;

                await SaveChangesAsync();
                await tran.CommitAsync();

                return ApiResult.Success(badge, "Cập nhật badge thành công");
            }
            catch (Exception ex)
            {
                await tran.RollbackAsync();
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
                return ApiResult.Success(null, "Xóa badge thành công");
            }
            catch (Exception ex)
            {
                await tran.RollbackAsync();
                return ApiResult.Fail(
                    "Xóa badge thất bại",
                    "INTERNAL_ERROR",
                    new[] { ex.Message }
                );
            }
        }
    }
}
