using Microsoft.EntityFrameworkCore;
using server.Domain.Entities;
using server.Infrastructure.Persistence;
using server.Service.Common.IServices;
using server.Service.Interfaces;
using server.Service.Models;
using server.Service.Models.Room;

namespace server.Service.Services
{
    public class RoomService : BaseService, IRoomService
    {
        public RoomService(DataContext dataContext, IUserService userService) : base(dataContext, userService)
        {
        }

        public async Task<ApiResult> Add(AddRoomModel model)
        {
            if (model == null)
                return ApiResult.Fail("Dữ liệu không hợp lệ", "VALIDATION_ERROR");

            if (string.IsNullOrWhiteSpace(model.Name))
                return ApiResult.Fail("Tên phòng không được để trống", "VALIDATION_ERROR");

            var ownerId = _userService.UserId;
            if (ownerId <= 0)
                return ApiResult.Fail("Không xác định được người tạo phòng", "UNAUTHORIZED");

            try
            {
                var room = new Room
                {
                    Name = model.Name.Trim(),
                    OwnerId = ownerId,
                    Password = string.IsNullOrWhiteSpace(model.Password) ? null : model.Password,
                    Avatar = string.IsNullOrWhiteSpace(model.Avatar) ? null : model.Avatar,
                    CreatedDate = Now,
                };

                await _dataContext.Rooms.AddAsync(room);
                await SaveChangesAsync();

                return ApiResult.Success(room, "Tạo phòng thành công");
            }
            catch (Exception ex)
            {
                return ApiResult.Fail("Tạo phòng thất bại", "INTERNAL_ERROR");
            }
        }


        public async Task<ApiResult> Delete(int id)
        {
            if (id <= 0)
                return ApiResult.Fail("Id không hợp lệ", "VALIDATION_ERROR");

            await using var tran = await _dataContext.Database.BeginTransactionAsync();
            try
            {
                var room = await _dataContext.Rooms.FirstOrDefaultAsync(x => x.Id == id);

                if (room == null)
                    return ApiResult.Fail("Không tìm thấy phòng", "ROOM_NOT_FOUND");

                if (room.OwnerId != _userService.UserId)
                    return ApiResult.Fail("Không có quyền xóa phòng", "FORBIDDEN");

                _dataContext.Rooms.Remove(room);
                await SaveChangesAsync();

                await tran.CommitAsync();

                return ApiResult.Success(null, "Xóa phòng thành công");
            }
            catch (Exception ex)
            {
                await tran.RollbackAsync();
                return ApiResult.Fail(
                    "Xóa phòng thất bại",
                    "INTERNAL_ERROR",
                    new[] { ex.Message }
                );
            }
        }

        public async Task<ApiResult> GetAll()
        {
            try
            {
                var result = await _dataContext.Rooms.AsNoTracking().ToListAsync();
                return ApiResult.Success(result, "Lấy tất cả thành công");
            }
            catch (Exception ex) 
            {
                return ApiResult.Fail(
                    message: "Lấy thất bại",
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
                var room = await _dataContext.Rooms.AsNoTracking()
                    .FirstOrDefaultAsync(x => x.Id == id);

                if (room == null)
                    return ApiResult.Fail("Không tìm thấy phòng", "ROOM_NOT_FOUND");

                return ApiResult.Success(room, "Lấy phòng thành công");
            }
            catch (Exception ex)
            {
                return ApiResult.Fail("Lấy phòng thất bại", "SYSTEM_ERROR", new[] { ex.Message });
            }
        }

        public async Task<ApiResult> GetByUserId(int userId)
        {
            if (userId <= 0)
                return ApiResult.Fail("UserId không hợp lệ", "VALIDATION_ERROR");

            try
            {
                var rooms = await _dataContext.Rooms.AsNoTracking()
                    .Where(x => x.OwnerId == userId)
                    .OrderByDescending(x => x.CreatedDate)
                    .ToListAsync();

                return ApiResult.Success(rooms, "Lấy danh sách phòng thành công");
            }
            catch (Exception ex)
            {
                return ApiResult.Fail("Lấy danh sách phòng thất bại", "SYSTEM_ERROR", new[] { ex.Message });
            }
        }

        public async Task<ApiResult> Update(UpdateRoomModel model)
        {
            if (model == null || model.Id <= 0)
                return ApiResult.Fail("Dữ liệu không hợp lệ", "VALIDATION_ERROR");

            await using var tran = await _dataContext.Database.BeginTransactionAsync();
            try
            {
                var room = await _dataContext.Rooms.FirstOrDefaultAsync(x => x.Id == model.Id);

                if (room == null)
                    return ApiResult.Fail("Không tìm thấy phòng", "ROOM_NOT_FOUND");

                if (room.OwnerId != _userService.UserId)
                    return ApiResult.Fail("Không có quyền cập nhật phòng", "FORBIDDEN");

                if (!string.IsNullOrWhiteSpace(model.Name))
                    room.Name = model.Name.Trim();

                if (model.Password != null)
                    room.Password = string.IsNullOrWhiteSpace(model.Password) ? null : model.Password;

                if (model.Avatar != null)
                    room.Avatar = string.IsNullOrWhiteSpace(model.Avatar) ? null : model.Avatar;

                room.UpdatedDate = Now;

                await SaveChangesAsync();
                await tran.CommitAsync();

                return ApiResult.Success(room, "Cập nhật phòng thành công");
            }
            catch (Exception ex)
            {
                await tran.RollbackAsync();
                return ApiResult.Fail(
                    "Cập nhật phòng thất bại",
                    "INTERNAL_ERROR",
                    new[] { ex.Message }
                );
            }
        }
    }
}
