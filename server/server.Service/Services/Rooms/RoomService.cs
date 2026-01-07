using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using server.Domain.Entities;
using server.Domain.Enums;
using server.Infrastructure.Persistence;
using server.Service.Common.IServices;
using server.Service.Interfaces;
using server.Service.Models;
using server.Service.Models.Rooms;
using server.Service.Utilities;

namespace server.Service.Services.Rooms
{
    public class RoomService : BaseService, IRoomService
    {
        private readonly IConfiguration _configuration;

        public RoomService(DataContext dataContext, IUserService userService, IConfiguration configuration) : base(dataContext, userService)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }
        public char GetNameRoom(string name)
        {
            return name[0];
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
            var roomCode = GenerateRoomCode.Generate();
            var existingRoom = await _dataContext.Rooms.AsNoTracking()
                .FirstOrDefaultAsync(x => x.RoomCode == roomCode);
            if (existingRoom != null)
            {
                return ApiResult.Fail("Mã phòng đã tồn tại, vui lòng thử lại", "DUPLICATE_ROOM_CODE");
            }
            if(model.Name.Length > 30)
            {
                return ApiResult.Fail("Tên phòng không được vượt quá 30 ký tự", "VALIDATION_ERROR");
            }
            try
            {
                var room = new Room
                {
                    Name = model.Name.Trim(),
                    OwnerId = ownerId,
                    Password = string.IsNullOrWhiteSpace(model.Password) ? null : model.Password,
                    RoomCode = roomCode,
                    Avatar = GetNameRoom(model.Name).ToString(),
                    CreatedDate = Now,
                };
                await _dataContext.Rooms.AddAsync(room);
                await SaveChangesAsync();
                var userRoom = new UserRoom
                {
                    UserId = ownerId,
                    RoomId = room.Id,
                    Role = RoomRole.GroupLeader,
                    IsBan = false,
                    CreatedDate = Now,
                };
                await _dataContext.UserRooms.AddAsync(userRoom);
           
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

        public async Task<ApiResult> UploadAvatarAsync(int roomId, IFormFile file, CancellationToken cancellationToken = default)
        {
            if (roomId <= 0)
                return ApiResult.Fail("Id phòng không hợp lệ", "VALIDATION_ERROR");

            if (file == null || file.Length == 0)
                return ApiResult.Fail("File ảnh không được để trống", "VALIDATION_ERROR");

            await using var tran = await _dataContext.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                var room = await _dataContext.Rooms.FirstOrDefaultAsync(x => x.Id == roomId, cancellationToken);

                if (room == null)
                    return ApiResult.Fail("Không tìm thấy phòng", "ROOM_NOT_FOUND");

                if (room.OwnerId != _userService.UserId)
                    return ApiResult.Fail("Không có quyền cập nhật avatar phòng", "FORBIDDEN");

                var imageUrl = await ImgBBUploadHelper.UploadImageAsync(file, _configuration, cancellationToken);

                room.Avatar = imageUrl;
                room.UpdatedDate = Now;

                await SaveChangesAsync(cancellationToken);
                await tran.CommitAsync(cancellationToken);

                return ApiResult.Success(room, "Cập nhật avatar phòng thành công");
            }
            catch (InvalidOperationException ex)
            {
                await tran.RollbackAsync(cancellationToken);
                return ApiResult.Fail(ex.Message, "UPLOAD_ERROR");
            }
            catch (Exception ex)
            {
                await tran.RollbackAsync(cancellationToken);
                return ApiResult.Fail(
                    "Cập nhật avatar phòng thất bại",
                    "INTERNAL_ERROR",
                    new[] { ex.Message }
                );
            }
        }
    }
}