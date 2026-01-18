using Microsoft.EntityFrameworkCore;
using server.Domain.Entities;
using server.Infrastructure.Persistence;
using server.Service.Common.IServices;
using server.Service.Interfaces;
using server.Service.Models;
using server.Service.Models.BreakoutRooms;

namespace server.Service.Services.BreakoutRooms
{
    /// <summary>
    /// BreakoutRoomService: xử lý nghiệp vụ cho BreakoutRoom (phòng thảo luận nhỏ thuộc một Room cha).
    ///
    /// Quy tắc quyền (theo yêu cầu):
    /// - User nào cũng có thể tạo/update/delete breakout room
    /// - Nhưng bắt buộc user phải là thành viên của Room cha (UserRooms tồn tại, DeletedDate == null)
    /// - Và không bị ban (IsBan == false)
    ///
    /// Quy ước:
    /// - Chỉ thao tác record chưa xoá mềm: DeletedDate == null
    /// - Thời gian audit dùng CHUNG BaseService.Now (UTC+7) để đồng bộ realtime:
    ///   + CreatedDate khi tạo
    ///   + UpdatedDate khi update
    ///   + DeletedDate khi delete (soft delete)
    /// - KHÔNG dùng EntityBase.MarkUpdated/MarkDeleted vì DateTime.Now phụ thuộc timezone của server (có thể là UTC).
    /// </summary>
    public class BreakoutRoomService : BaseService, IBreakoutRoomService
    {
        /// <summary>
        /// Khởi tạo service với các dependency bắt buộc.
        /// </summary>
        public BreakoutRoomService(DataContext dataContext, IUserService userService)
            : base(dataContext, userService) { }

        /// <summary>
        /// Lấy tất cả breakout rooms (chưa xoá mềm).
        ///
        /// Cách dùng:
        /// - Dùng cho admin/debug hoặc màn hình tổng hợp.
        /// </summary>
        public async Task<ApiResult> GetAllBreakoutRooms()
        {
            try
            {
                var rooms = await _dataContext.BreakoutRooms
                    .AsNoTracking()
                    .Where(x => x.DeletedDate == null)
                    .OrderByDescending(x => x.CreatedDate)
                    .ToListAsync();

                return ApiResult.Success(rooms, "Lấy danh sách breakout room thành công");
            }
            catch (Exception ex)
            {
                return ApiResult.Fail("Lấy danh sách breakout room thất bại", "SYSTEM_ERROR", new[] { ex.Message });
            }
        }

        /// <summary>
        /// Lấy breakout room theo Id (chưa xoá mềm).
        ///
        /// Cách dùng:
        /// - Dùng khi mở chi tiết breakout room.
        /// - Chỉ member của phòng cha mới xem được (không bị ban).
        /// </summary>
        public async Task<ApiResult> GetBreakoutRoomById(int id)
        {
            if (id <= 0)
                return ApiResult.Fail("Id không hợp lệ", "VALIDATION_ERROR");

            try
            {
                var room = await _dataContext.BreakoutRooms
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.Id == id && x.DeletedDate == null);

                if (room == null)
                    return ApiResult.Fail("Không tìm thấy breakout room", "BREAKOUT_ROOM_NOT_FOUND");

                var auth = await EnsureUserInParentRoomAsync(room.ParentRoomId);
                if (!auth.IsSuccess) return auth;

                var now = Now;
                return ApiResult.Success(ToRoomResponse(room, now), "Lấy breakout room thành công");
            }
            catch (Exception ex)
            {
                return ApiResult.Fail("Lấy breakout room thất bại", "SYSTEM_ERROR", new[] { ex.Message });
            }
        }

        /// <summary>
        /// Lấy danh sách breakout rooms theo ParentRoomId (phòng cha).
        ///
        /// Cách dùng:
        /// - FE mở phòng cha và cần list các breakout room con.
        /// - Chỉ member của phòng cha mới xem được (không bị ban).
        /// </summary>
        public async Task<ApiResult> GetBreakoutRoomsByParentRoomId(int parentRoomId)
        {
            if (parentRoomId <= 0)
                return ApiResult.Fail("ParentRoomId không hợp lệ", "VALIDATION_ERROR");

            try
            {
                var parentExists = await _dataContext.Rooms
                    .AsNoTracking()
                    .AnyAsync(r => r.Id == parentRoomId);

                if (!parentExists)
                    return ApiResult.Fail("Phòng cha không tồn tại", "PARENT_ROOM_NOT_FOUND");

                var auth = await EnsureUserInParentRoomAsync(parentRoomId);
                if (!auth.IsSuccess) return auth;

                var rooms = await _dataContext.BreakoutRooms
                    .AsNoTracking()
                    .Where(x => x.ParentRoomId == parentRoomId && x.DeletedDate == null)
                    .OrderByDescending(x => x.CreatedDate)
                    .ToListAsync();

                var now = Now;
                var data = rooms.Select(r => ToRoomResponse(r, now)).ToList();

                return ApiResult.Success(data, "Lấy danh sách breakout room theo phòng cha thành công");
            }
            catch (Exception ex)
            {
                return ApiResult.Fail("Lấy danh sách breakout room thất bại", "SYSTEM_ERROR", new[] { ex.Message });
            }
        }

        /// <summary>
        /// Tạo mới breakout room.
        ///
        /// Quyền:
        /// - User nào cũng tạo được
        /// - Nhưng phải là thành viên Room cha và không bị ban
        /// </summary>
        public async Task<ApiResult> AddBreakoutRoom(AddBreakoutRoomModel model)
        {
            if (model == null)
                return ApiResult.Fail("Dữ liệu không hợp lệ", "VALIDATION_ERROR");

            if (model.ParentRoomId <= 0)
                return ApiResult.Fail("ParentRoomId không hợp lệ", "VALIDATION_ERROR");

            if (string.IsNullOrWhiteSpace(model.RoomName))
                return ApiResult.Fail("Tên breakout room không được để trống", "VALIDATION_ERROR");

            var roomName = model.RoomName.Trim();
            if (roomName.Length > 30)
                return ApiResult.Fail("Tên breakout room không được vượt quá 30 ký tự", "VALIDATION_ERROR");

            try
            {
                var currentUserId = _userService.UserId;
                if (currentUserId <= 0)
                    return ApiResult.Fail("Bạn chưa đăng nhập", "UNAUTHORIZED");

                var parentExists = await _dataContext.Rooms
                    .AsNoTracking()
                    .AnyAsync(r => r.Id == model.ParentRoomId);

                if (!parentExists)
                    return ApiResult.Fail("Phòng cha không tồn tại", "PARENT_ROOM_NOT_FOUND");

                var auth = await EnsureUserInParentRoomAsync(model.ParentRoomId);
                if (!auth.IsSuccess) return auth;

                var lowerName = roomName.ToLower();
                var duplicate = await _dataContext.BreakoutRooms
                    .AsNoTracking()
                    .AnyAsync(x =>
                        x.ParentRoomId == model.ParentRoomId &&
                        x.DeletedDate == null &&
                        x.Name.ToLower() == lowerName);

                if (duplicate)
                    return ApiResult.Fail("Tên breakout room đã tồn tại trong phòng này", "DUPLICATE_BREAKOUT_ROOM_NAME");

                var startAt = Now;

                var (ok, expireAt, error) = ValidateExpireAt(startAt, model.ExpireAt);
                if (!ok) return error!;

                var entity = new BreakoutRoom
                {
                    Name = roomName,
                    ParentRoomId = model.ParentRoomId,
                    ExpireAt = expireAt,
                    CreatedDate = startAt,
                    UpdatedDate = null,
                    DeletedDate = null
                };

                await _dataContext.BreakoutRooms.AddAsync(entity);
                await SaveChangesAsync();

                return ApiResult.Success(ToRoomResponse(entity, startAt), "Tạo breakout room thành công");
            }
            catch (Exception ex)
            {
                return ApiResult.Fail("Tạo breakout room thất bại", "INTERNAL_ERROR", new[] { ex.Message });
            }
        }

        /// <summary>
        /// Cập nhật breakout room (tên + expireAt).
        ///
        /// Quyền:
        /// - User nào cũng update được
        /// - Nhưng phải là thành viên Room cha của breakout room và không bị ban
        /// </summary>
        public async Task<ApiResult> UpdateBreakoutRoom(UpdateBreakoutRoomModel model)
        {
            if (model == null || model.Id <= 0)
                return ApiResult.Fail("Dữ liệu không hợp lệ", "VALIDATION_ERROR");

            if (string.IsNullOrWhiteSpace(model.RoomName))
                return ApiResult.Fail("Tên breakout room không được để trống", "VALIDATION_ERROR");

            var roomName = model.RoomName.Trim();
            if (roomName.Length > 30)
                return ApiResult.Fail("Tên breakout room không được vượt quá 30 ký tự", "VALIDATION_ERROR");

            await using var tran = await _dataContext.Database.BeginTransactionAsync();
            try
            {
                var currentUserId = _userService.UserId;
                if (currentUserId <= 0)
                    return ApiResult.Fail("Bạn chưa đăng nhập", "UNAUTHORIZED");

                var entity = await _dataContext.BreakoutRooms
                    .FirstOrDefaultAsync(x => x.Id == model.Id && x.DeletedDate == null);

                if (entity == null)
                    return ApiResult.Fail("Không tìm thấy breakout room", "BREAKOUT_ROOM_NOT_FOUND");

                var auth = await EnsureUserInParentRoomAsync(entity.ParentRoomId);
                if (!auth.IsSuccess) return auth;

                var lowerName = roomName.ToLower();
                var duplicate = await _dataContext.BreakoutRooms
                    .AsNoTracking()
                    .AnyAsync(x =>
                        x.Id != model.Id &&
                        x.ParentRoomId == entity.ParentRoomId &&
                        x.DeletedDate == null &&
                        x.Name.ToLower() == lowerName);

                if (duplicate)
                    return ApiResult.Fail("Tên breakout room đã tồn tại trong phòng này", "DUPLICATE_BREAKOUT_ROOM_NAME");

                var now = Now;

                var (ok, expireAt, error) = ValidateExpireAt(now, model.ExpireAt);
                if (!ok) return error!;

                entity.Name = roomName;
                entity.ExpireAt = expireAt;
                entity.UpdatedDate = now;

                await SaveChangesAsync();
                await tran.CommitAsync();

                return ApiResult.Success(ToRoomResponse(entity, now), "Cập nhật breakout room thành công");
            }
            catch (Exception ex)
            {
                await tran.RollbackAsync();
                return ApiResult.Fail("Cập nhật breakout room thất bại", "INTERNAL_ERROR", new[] { ex.Message });
            }
        }

        /// <summary>
        /// Xoá mềm breakout room theo Id.
        ///
        /// Quyền:
        /// - User nào cũng delete được
        /// - Nhưng phải là thành viên Room cha của breakout room và không bị ban
        /// </summary>
        public async Task<ApiResult> DeleteBreakoutRoom(int breakoutRoomId)
        {
            if (breakoutRoomId <= 0)
                return ApiResult.Fail("Id không hợp lệ", "VALIDATION_ERROR");

            await using var tran = await _dataContext.Database.BeginTransactionAsync();
            try
            {
                var currentUserId = _userService.UserId;
                if (currentUserId <= 0)
                    return ApiResult.Fail("Bạn chưa đăng nhập", "UNAUTHORIZED");

                var entity = await _dataContext.BreakoutRooms
                    .FirstOrDefaultAsync(x => x.Id == breakoutRoomId);

                if (entity == null)
                    return ApiResult.Fail("Không tìm thấy breakout room", "BREAKOUT_ROOM_NOT_FOUND");

                var auth = await EnsureUserInParentRoomAsync(entity.ParentRoomId);
                if (!auth.IsSuccess) return auth;

                _dataContext.BreakoutRooms.Remove(entity);

                await SaveChangesAsync();
                await tran.CommitAsync();

                return ApiResult.Success(null, "Xóa breakout room thành công");
            }
            catch (Exception ex)
            {
                await tran.RollbackAsync();
                return ApiResult.Fail("Xóa breakout room thất bại", "INTERNAL_ERROR", new[] { ex.Message });
            }
        }


        /// <summary>
        /// Validate ExpireAt:
        /// - null => hợp lệ (không giới hạn)
        /// - có giá trị => phải lớn hơn startAt
        /// </summary>
        private static (bool ok, DateTime? expireAt, ApiResult? error) ValidateExpireAt(DateTime startAt, DateTime? expireAt)
        {
            if (!expireAt.HasValue)
                return (true, null, null);

            if (expireAt.Value <= startAt)
                return (false, null, ApiResult.Fail("ExpireAt phải lớn hơn thời gian hiện tại", "VALIDATION_ERROR"));

            return (true, expireAt.Value, null);
        }

        /// <summary>
        /// Map BreakoutRoom entity sang object response cho FE:
        /// - StartAt: CreatedDate (fallback now)
        /// - ExpireAt: nullable
        /// - DurationMinutes/RemainingMinutes: nullable nếu ExpireAt null
        /// </summary>
        private static object ToRoomResponse(BreakoutRoom room, DateTime now)
        {
            var startAt = room.CreatedDate ?? now;

            int? durationMinutes = null;
            int? remainingMinutes = null;

            if (room.ExpireAt.HasValue)
            {
                var total = room.ExpireAt.Value - startAt;
                durationMinutes = total.TotalMinutes <= 0 ? 0 : (int)Math.Ceiling(total.TotalMinutes);

                var remain = room.ExpireAt.Value - now;
                remainingMinutes = remain.TotalMinutes <= 0 ? 0 : (int)Math.Ceiling(remain.TotalMinutes);
            }

            return new
            {
                room.Id,
                RoomName = room.Name,
                room.ParentRoomId,
                StartAt = startAt,
                ExpireAt = room.ExpireAt,
                DurationMinutes = durationMinutes,
                RemainingMinutes = remainingMinutes,
                room.CreatedDate,
                room.UpdatedDate
            };
        }

        /// <summary>
        /// Ensure user hiện tại là thành viên của roomId và không bị ban.
        /// </summary>
        private async Task<ApiResult> EnsureUserInParentRoomAsync(int roomId)
        {
            var currentUserId = _userService.UserId;
            if (currentUserId <= 0)
                return ApiResult.Fail("Bạn chưa đăng nhập", "UNAUTHORIZED");

            var userRoom = await _dataContext.UserRooms
                .AsNoTracking()
                .FirstOrDefaultAsync(ur =>
                    ur.RoomId == roomId &&
                    ur.UserId == currentUserId &&
                    ur.DeletedDate == null);

            if (userRoom == null)
                return ApiResult.Fail("Bạn không phải thành viên của phòng này", "NOT_IN_ROOM");

            if (userRoom.IsBan)
                return ApiResult.Fail("Bạn đã bị ban khỏi phòng này", "ACCESS_DENIED");

            return ApiResult.Success(null);
        }
    }
}
