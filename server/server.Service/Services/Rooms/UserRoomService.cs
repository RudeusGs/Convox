using Microsoft.EntityFrameworkCore;
using server.Domain.Entities;
using server.Domain.Enums;
using server.Infrastructure.Persistence;
using server.Service.Common.IServices;
using server.Service.Common.Services;
using server.Service.Interfaces;
using server.Service.Models;
using server.Service.Models.UserRooms;

namespace server.Service.Services.Rooms
{
    public class UserRoomService : BaseService, IUserRoomService
    {
        public UserRoomService(DataContext dataContext, IUserService userService) : base(dataContext, userService)
        {
        }

        public async Task<ApiResult> AddUserToRoom(AddUserToRoomModel model)
        {
            if (model == null)
                return ApiResult.Fail("Dữ liệu không hợp lệ", "VALIDATION_ERROR");

            if (model.UserId <= 0 || model.RoomId <= 0)
                return ApiResult.Fail("UserId hoặc RoomId không hợp lệ", "VALIDATION_ERROR");

            var roomExists = await _dataContext.Rooms
                .AsNoTracking()
                .AnyAsync(r => r.Id == model.RoomId);

            if (!roomExists)
                return ApiResult.Fail("Không tìm thấy phòng", "ROOM_NOT_FOUND");
            var existing = await _dataContext.UserRooms
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.UserId == model.UserId && x.RoomId == model.RoomId);

            if (existing != null)
            {
                if (existing.IsBan)
                    return ApiResult.Fail("Người này đang bị cấm khỏi phòng", "USER_BANNED_FROM_ROOM");

                return ApiResult.Fail("Người này đã có trong room rồi", "USER_ALREADY_IN_ROOM");
            }
            try
            {
                var newUserRoom = new UserRoom
                {
                    UserId = model.UserId,
                    RoomId = model.RoomId,
                    Role = model.Role,
                    IsBan = false,
                    CreatedDate = Now,
                };
                await _dataContext.UserRooms.AddAsync(newUserRoom);
                await _dataContext.SaveChangesAsync();

                return ApiResult.Success(newUserRoom, "Thêm người dùng vào room thành công");
            }
            catch (Exception ex)
            {
                return ApiResult.Fail("Thêm người dùng vào room thất bại", "INTERNAL_ERROR", new[] { ex.Message });
            }
        }


        public async Task<ApiResult> BanUserFromRoom(int userId, int roomId)
        {
            if (userId <= 0 || roomId <= 0)
                return ApiResult.Fail("Dữ liệu không hợp lệ", "VALIDATION_ERROR");

            var requesterId = _userService.UserId;
            if (requesterId <= 0)
                return ApiResult.Fail("Không xác định được người thực hiện", "UNAUTHORIZED");

            if (userId == requesterId)
                return ApiResult.Fail("Không thể tự ban chính mình", "CANNOT_BAN_SELF");

            var requesterInRoom = await _dataContext.UserRooms
                .FirstOrDefaultAsync(x =>
                    x.UserId == requesterId &&
                    x.RoomId == roomId &&
                    !x.IsBan);

            if (requesterInRoom == null)
                return ApiResult.Fail("Bạn không thuộc phòng này", "NOT_IN_ROOM");

            if (requesterInRoom.Role != RoomRole.GroupLeader)
                return ApiResult.Fail("Bạn không có quyền ban người dùng", "FORBIDDEN");

            var targetUserRoom = await _dataContext.UserRooms
                .FirstOrDefaultAsync(x =>
                    x.UserId == userId &&
                    x.RoomId == roomId);

            if (targetUserRoom == null)
                return ApiResult.Fail("Người dùng không có trong phòng", "USER_NOT_IN_ROOM");

            if (targetUserRoom.IsBan)
                return ApiResult.Fail("Người dùng đã bị ban trước đó", "USER_ALREADY_BANNED");

            targetUserRoom.IsBan = true;
            targetUserRoom.UpdatedDate = Now;

            await _dataContext.SaveChangesAsync();

            return ApiResult.Success(targetUserRoom, "Ban người dùng khỏi phòng thành công");
        }

        public async Task<ApiResult> ChangeUserRoleInRoom(ChangeUserRoleInRoomModel model)
        {
            if (model == null)
                return ApiResult.Fail("Dữ liệu không hợp lệ", "VALIDATION_ERROR");

            if (model.UserId <= 0 || model.RoomId <= 0)
                return ApiResult.Fail("UserId hoặc RoomId không hợp lệ", "VALIDATION_ERROR");

            var requesterId = _userService.UserId;
            if (requesterId <= 0)
                return ApiResult.Fail("Không xác định được người thực hiện", "UNAUTHORIZED");

            if (!Enum.IsDefined(typeof(RoomRole), model.NewRole))
                return ApiResult.Fail("Role không hợp lệ", "INVALID_ROLE");

            if (model.UserId == requesterId)
                return ApiResult.Fail("Không thể tự thay đổi role của chính mình", "CANNOT_CHANGE_SELF_ROLE");

            var requester = await _dataContext.UserRooms
                .AsNoTracking()
                .FirstOrDefaultAsync(x =>
                    x.RoomId == model.RoomId &&
                    x.UserId == requesterId);

            if (requester == null)
                return ApiResult.Fail("Bạn không thuộc phòng này", "NOT_IN_ROOM");

            if (requester.IsBan)
                return ApiResult.Fail("Bạn đang bị cấm trong phòng", "REQUESTER_BANNED");

            if (requester.Role != RoomRole.GroupLeader)
                return ApiResult.Fail("Bạn không có quyền thay đổi role", "FORBIDDEN");

            var target = await _dataContext.UserRooms
                .FirstOrDefaultAsync(x =>
                    x.RoomId == model.RoomId &&
                    x.UserId == model.UserId);

            if (target == null)
                return ApiResult.Fail("Người dùng không có trong phòng", "USER_NOT_IN_ROOM");

            if (target.IsBan)
                return ApiResult.Fail("Không thể đổi role vì người dùng đang bị ban", "USER_BANNED_FROM_ROOM");

            if (target.Role == model.NewRole)
                return ApiResult.Fail("Role mới trùng với role hiện tại", "ROLE_UNCHANGED");

            if (model.NewRole == RoomRole.GroupLeader)
            {
                var currentLeader = await _dataContext.UserRooms
                    .FirstOrDefaultAsync(x =>
                        x.RoomId == model.RoomId &&
                        x.Role == RoomRole.GroupLeader &&
                        !x.IsBan);

                if (currentLeader != null && currentLeader.UserId != target.UserId)
                {
                    currentLeader.Role = RoomRole.RegularUser;
                    currentLeader.UpdatedDate = Now;
                }
            }

            target.Role = model.NewRole;
            target.UpdatedDate = Now;

            await _dataContext.SaveChangesAsync();

            return ApiResult.Success(target, "Thay đổi role thành công");
        }


        public async Task<ApiResult> GetRoomsForUser(int userId)
        {
            if (userId <= 0)
                return ApiResult.Fail("UserId không hợp lệ", "VALIDATION_ERROR");

            try
            {
                var result = await (
                    from ur in _dataContext.UserRooms.AsNoTracking()
                    join r in _dataContext.Rooms.AsNoTracking()
                        on ur.RoomId equals r.Id
                    where ur.UserId == userId && !ur.IsBan
                    orderby r.CreatedDate descending
                    select new
                    {
                        r.Id,
                        r.Name,
                        r.RoomCode,
                        r.Avatar,
                        r.OwnerId,
                        r.IsLocked,
                        Role = ur.Role,
                        JoinedAt = ur.CreatedDate
                    }
                ).ToListAsync();

                return ApiResult.Success(result, "Lấy danh sách phòng đang tham gia thành công");
            }
            catch (Exception ex)
            {
                return ApiResult.Fail("Lấy danh sách phòng thất bại", "SYSTEM_ERROR", new[] { ex.Message });
            }
        }

        public async Task<ApiResult> GetUsersInRoom(int roomId)
        {
            if (roomId <= 0)
                return ApiResult.Fail("RoomId không hợp lệ", "VALIDATION_ERROR");

            try
            {
                var roomExists = await _dataContext.Rooms
                    .AsNoTracking()
                    .AnyAsync(x => x.Id == roomId);

                if (!roomExists)
                    return ApiResult.Fail("Không tìm thấy phòng", "ROOM_NOT_FOUND");

                var users = await (
                    from ur in _dataContext.UserRooms.AsNoTracking()
                    join u in _dataContext.Users.AsNoTracking()
                        on ur.UserId equals u.Id
                    where ur.RoomId == roomId && !ur.IsBan
                    orderby ur.Role descending, ur.CreatedDate ascending
                    select new
                    {
                        u.Id,
                        u.FullName,
                        u.Avatar,
                        u.Status,
                        Role = ur.Role,
                        JoinedAt = ur.CreatedDate
                    }
                ).ToListAsync();

                return ApiResult.Success(users, "Lấy danh sách người dùng trong phòng thành công");
            }
            catch (Exception ex)
            {
                return ApiResult.Fail(
                    "Lấy danh sách người dùng trong phòng thất bại",
                    "SYSTEM_ERROR",
                    new[] { ex.Message });
            }
        }

        public async Task<ApiResult> JoinRoomCode(JoinRoomCodeModel model)
        {
            if (model == null)
                return ApiResult.Fail("Dữ liệu không hợp lệ", "VALIDATION_ERROR");

            if (model.UserId <= 0)
                return ApiResult.Fail("UserId không hợp lệ", "VALIDATION_ERROR");

            if (string.IsNullOrWhiteSpace(model.RoomCode))
                return ApiResult.Fail("Mã phòng không được để trống", "VALIDATION_ERROR");

            try
            {
                var roomCode = model.RoomCode.Trim();

                var room = await _dataContext.Rooms
                    .FirstOrDefaultAsync(x => x.RoomCode == roomCode);

                if (room == null)
                    return ApiResult.Fail("Mã phòng không tồn tại", "ROOM_CODE_NOT_FOUND");

                if (room.IsLocked)
                    return ApiResult.Fail("Phòng đang bị khóa", "ROOM_LOCKED");

                if (!string.IsNullOrWhiteSpace(room.Password))
                {
                    if (string.IsNullOrWhiteSpace(model.Password))
                        return ApiResult.Fail("Phòng yêu cầu mật khẩu", "ROOM_PASSWORD_REQUIRED");

                    if (!string.Equals(room.Password, model.Password))
                        return ApiResult.Fail("Mật khẩu không đúng", "ROOM_PASSWORD_INVALID");
                }

                var existing = await _dataContext.UserRooms
                    .FirstOrDefaultAsync(x => x.UserId == model.UserId && x.RoomId == room.Id);

                if (existing != null)
                {
                    if (existing.IsBan)
                        return ApiResult.Fail("Bạn đang bị cấm khỏi phòng này", "USER_BANNED_FROM_ROOM");

                    return ApiResult.Fail("Bạn đã tham gia phòng này rồi", "USER_ALREADY_IN_ROOM");
                }

                var userRoom = new UserRoom
                {
                    UserId = model.UserId,
                    RoomId = room.Id,
                    Role = model.Role,
                    IsBan = false,
                    CreatedDate = Now
                };

                await _dataContext.UserRooms.AddAsync(userRoom);
                await _dataContext.SaveChangesAsync();
                return ApiResult.Success(
                    new
                    {
                        room.Id,
                        room.Name,
                        room.RoomCode,
                        room.Avatar,
                        room.OwnerId,
                        room.IsLocked,
                        Role = userRoom.Role
                    },
                    "Tham gia phòng thành công");
            }
            catch (Exception ex)
            {
                return ApiResult.Fail("Tham gia phòng thất bại", "INTERNAL_ERROR", new[] { ex.Message });
            }
        }

        public async Task<ApiResult> RemoveUserFromRoom(int userId, int roomId)
        {
            if (userId <= 0 || roomId <= 0)
                return ApiResult.Fail("Dữ liệu không hợp lệ", "VALIDATION_ERROR");

            var requesterId = _userService.UserId;
            if (requesterId <= 0)
                return ApiResult.Fail("Không xác định được người thực hiện", "UNAUTHORIZED");
            var requester = await _dataContext.UserRooms
                .FirstOrDefaultAsync(x =>
                    x.UserId == requesterId &&
                    x.RoomId == roomId);

            if (requester == null)
                return ApiResult.Fail("Bạn không thuộc phòng này", "NOT_IN_ROOM");

            if (requester.IsBan)
                return ApiResult.Fail("Bạn đang bị cấm trong phòng", "REQUESTER_BANNED");
            var target = await _dataContext.UserRooms
                .FirstOrDefaultAsync(x =>
                    x.UserId == userId &&
                    x.RoomId == roomId);

            if (target == null)
                return ApiResult.Fail("Người dùng không có trong phòng", "USER_NOT_IN_ROOM");
            if (userId == requesterId)
            {
                if (requester.Role == RoomRole.GroupLeader)
                {
                    var otherLeaderExists = await _dataContext.UserRooms
                        .AnyAsync(x =>
                            x.RoomId == roomId &&
                            x.Role == RoomRole.GroupLeader &&
                            x.UserId != requesterId &&
                            !x.IsBan);

                    if (!otherLeaderExists)
                        return ApiResult.Fail(
                            "Bạn cần chuyển quyền leader trước khi rời phòng",
                            "LEADER_CANNOT_LEAVE");
                }

                _dataContext.UserRooms.Remove(target);
                await _dataContext.SaveChangesAsync();

                return ApiResult.Success(message: "Rời phòng thành công");
            }
            if (requester.Role != RoomRole.GroupLeader)
                return ApiResult.Fail("Bạn không có quyền xoá người dùng", "FORBIDDEN");
            if (target.Role == RoomRole.GroupLeader)
                return ApiResult.Fail("Không thể xoá leader khỏi phòng", "CANNOT_REMOVE_LEADER");

            _dataContext.UserRooms.Remove(target);
            await _dataContext.SaveChangesAsync();

            return ApiResult.Success(message: "Xoá người dùng khỏi phòng thành công");
        }


        public async Task<ApiResult> UnbanUserFromRoom(int userId, int roomId)
        {
            if (userId <= 0 || roomId <= 0)
                return ApiResult.Fail("Dữ liệu không hợp lệ", "VALIDATION_ERROR");

            var requesterId = _userService.UserId;
            if (requesterId <= 0)
                return ApiResult.Fail("Không xác định được người thực hiện", "UNAUTHORIZED");

            if (userId == requesterId)
                return ApiResult.Fail("Không thể tự unban chính mình", "CANNOT_UNBAN_SELF");

            var requester = await _dataContext.UserRooms
                .FirstOrDefaultAsync(x =>
                    x.UserId == requesterId &&
                    x.RoomId == roomId);

            if (requester == null)
                return ApiResult.Fail("Bạn không thuộc phòng này", "NOT_IN_ROOM");

            if (requester.IsBan)
                return ApiResult.Fail("Bạn đang bị cấm trong phòng", "REQUESTER_BANNED");

            if (requester.Role != RoomRole.GroupLeader)
                return ApiResult.Fail("Bạn không có quyền unban người dùng", "FORBIDDEN");

            var target = await _dataContext.UserRooms
                .FirstOrDefaultAsync(x =>
                    x.UserId == userId &&
                    x.RoomId == roomId);

            if (target == null)
                return ApiResult.Fail("Người dùng không tồn tại trong phòng", "USER_NOT_IN_ROOM");

            if (!target.IsBan)
                return ApiResult.Fail("Người dùng không bị ban", "USER_NOT_BANNED");

            target.IsBan = false;
            target.UpdatedDate = Now;

            await _dataContext.SaveChangesAsync();

            return ApiResult.Success(message: "Unban người dùng thành công");
        }

    }
}
