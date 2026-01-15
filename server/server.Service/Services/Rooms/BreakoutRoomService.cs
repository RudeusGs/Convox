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
    /// Service xử lý BreakoutRoom (phòng thảo luận nhỏ thuộc một Room cha).
    ///
    /// Cách dùng chung:
    /// - Controller gọi các hàm trong service và trả về ApiResult.
    /// - Service tự validate input, query DB và xử lý nghiệp vụ.
    /// - Chỉ thao tác trên record chưa xoá mềm: DeletedDate == null.
    ///
    /// Quy ước thời gian:
    /// - CreatedDate set bằng BaseService.Now khi tạo mới.
    /// - UpdatedDate/DeletedDate set bằng EntityBase.MarkUpdated/MarkDeleted (DateTime.Now).
    ///
    /// Lưu ý:
    /// - ExpireAt:
    ///   + null  => không giới hạn thời gian
    ///   + có giá trị => phải lớn hơn thời điểm hiện tại (Now)
    /// </summary>
    public class BreakoutRoomService : BaseService, IBreakoutRoomService
    {
        /// <summary>
        /// Khởi tạo BreakoutRoomService.
        ///
        /// Cách dùng:
        /// - DI container sẽ inject DataContext và IUserService.
        /// - Service con kế thừa BaseService để dùng _dataContext, _userService và Now.
        /// </summary>
        public BreakoutRoomService(DataContext dataContext, IUserService userService)
            : base(dataContext, userService) { }

        /// <summary>
        /// Lấy tất cả BreakoutRoom (chưa xoá mềm).
        ///
        /// Cách dùng:
        /// - Gọi khi cần admin/list tổng tất cả breakout rooms.
        ///
        /// Trả về:
        /// - ApiResult.Success(List&lt;BreakoutRoom&gt;) nếu thành công.
        /// - ApiResult.Fail nếu lỗi hệ thống.
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
        /// Lấy BreakoutRoom theo Id.
        ///
        /// Cách dùng:
        /// - Gọi khi cần xem chi tiết một breakout room.
        ///
        /// Trả về:
        /// - Success: object có các field tiện cho FE:
        ///   Id, RoomName, ParentRoomId, StartAt, ExpireAt, DurationMinutes, RemainingMinutes, CreatedDate, UpdatedDate
        /// - Fail:
        ///   + VALIDATION_ERROR nếu id &lt;= 0
        ///   + BREAKOUT_ROOM_NOT_FOUND nếu không tồn tại hoặc đã xoá mềm
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

                var now = Now;
                return ApiResult.Success(ToRoomResponse(room, now), "Lấy breakout room thành công");
            }
            catch (Exception ex)
            {
                return ApiResult.Fail("Lấy breakout room thất bại", "SYSTEM_ERROR", new[] { ex.Message });
            }
        }

        /// <summary>
        /// Lấy danh sách BreakoutRoom theo ParentRoomId (phòng cha).
        ///
        /// Cách dùng:
        /// - Gọi khi FE mở phòng cha và cần list các breakout room con.
        ///
        /// Trả về:
        /// - Success: List object response (mỗi item có StartAt/ExpireAt/DurationMinutes/RemainingMinutes)
        /// - Fail:
        ///   + VALIDATION_ERROR nếu parentRoomId &lt;= 0
        ///   + PARENT_ROOM_NOT_FOUND nếu phòng cha không tồn tại
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

                var rooms = await _dataContext.BreakoutRooms
                    .AsNoTracking()
                    .Where(x => x.ParentRoomId == parentRoomId && x.DeletedDate == null)
                    .OrderByDescending(x => x.CreatedDate)
                    .ToListAsync();

                // Dùng cùng 1 mốc now cho list để RemainingMinutes nhất quán
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
        /// Tạo mới BreakoutRoom.
        ///
        /// Cách dùng:
        /// - FE gửi AddBreakoutRoomModel:
        ///   + RoomName: tên phòng
        ///   + ParentRoomId: id phòng cha
        ///   + ExpireAt: thời điểm kết thúc (null nếu không giới hạn)
        ///
        /// Logic chính:
        /// - Validate input
        /// - Check phòng cha tồn tại
        /// - Chặn trùng tên trong cùng phòng cha (case-insensitive)
        /// - Validate ExpireAt (ExpireAt phải &gt; Now nếu có)
        /// - Tạo record, set CreatedDate = Now
        ///
        /// Trả về:
        /// - Success: object response (kèm StartAt/ExpireAt/DurationMinutes/RemainingMinutes)
        /// - Fail: VALIDATION_ERROR / PARENT_ROOM_NOT_FOUND / DUPLICATE_BREAKOUT_ROOM_NAME / INTERNAL_ERROR
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
                var parentExists = await _dataContext.Rooms
                    .AsNoTracking()
                    .AnyAsync(r => r.Id == model.ParentRoomId);

                if (!parentExists)
                    return ApiResult.Fail("Phòng cha không tồn tại", "PARENT_ROOM_NOT_FOUND");

                // Trùng tên trong cùng parent (case-insensitive)
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
                    CreatedDate = startAt
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
        /// Cập nhật BreakoutRoom (tên + expireAt) theo Id.
        ///
        /// Cách dùng:
        /// - FE gửi UpdateBreakoutRoomModel:
        ///   + Id: breakoutRoomId
        ///   + RoomName: tên mới
        ///   + ExpireAt: thời điểm kết thúc (null để bỏ giới hạn)
        ///
        /// Logic chính:
        /// - Validate input
        /// - Find entity (DeletedDate == null)
        /// - Chặn trùng tên trong cùng parent (case-insensitive) trừ chính nó
        /// - Validate ExpireAt (ExpireAt phải &gt; Now nếu có)
        /// - Update entity:
        ///   + entity.Name
        ///   + entity.ExpireAt
        ///   + entity.MarkUpdated() (UpdatedDate = DateTime.Now theo EntityBase)
        ///
        /// Trả về:
        /// - Success: object response (kèm StartAt/ExpireAt/DurationMinutes/RemainingMinutes)
        /// - Fail: VALIDATION_ERROR / BREAKOUT_ROOM_NOT_FOUND / DUPLICATE_BREAKOUT_ROOM_NAME / INTERNAL_ERROR
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
                var entity = await _dataContext.BreakoutRooms
                    .FirstOrDefaultAsync(x => x.Id == model.Id && x.DeletedDate == null);

                if (entity == null)
                    return ApiResult.Fail("Không tìm thấy breakout room", "BREAKOUT_ROOM_NOT_FOUND");

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

                // Đồng bộ theo EntityBase (không tự set UpdatedDate bằng Now)
                entity.MarkUpdated();

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
        /// Xoá mềm BreakoutRoom theo Id.
        ///
        /// Cách dùng:
        /// - Gọi khi người dùng muốn xoá breakout room.
        ///
        /// Logic chính:
        /// - Find entity (DeletedDate == null)
        /// - entity.MarkDeleted() (DeletedDate/UpdatedDate = DateTime.Now theo EntityBase)
        ///
        /// Trả về:
        /// - Success: null data
        /// - Fail: VALIDATION_ERROR / BREAKOUT_ROOM_NOT_FOUND / INTERNAL_ERROR
        /// </summary>
        public async Task<ApiResult> DeleteBreakoutRoom(int breakoutRoomId)
        {
            if (breakoutRoomId <= 0)
                return ApiResult.Fail("Id không hợp lệ", "VALIDATION_ERROR");

            await using var tran = await _dataContext.Database.BeginTransactionAsync();
            try
            {
                var entity = await _dataContext.BreakoutRooms
                    .FirstOrDefaultAsync(x => x.Id == breakoutRoomId && x.DeletedDate == null);

                if (entity == null)
                    return ApiResult.Fail("Không tìm thấy breakout room", "BREAKOUT_ROOM_NOT_FOUND");

                // Đồng bộ theo EntityBase (không tự set DeletedDate/UpdatedDate bằng Now)
                entity.MarkDeleted();

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
        /// Validate ExpireAt theo rule:
        /// - ExpireAt == null => hợp lệ (không giới hạn)
        /// - ExpireAt != null => phải lớn hơn startAt
        ///
        /// Cách dùng:
        /// - Dùng trong Add/Update trước khi gán entity.ExpireAt
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
        /// Tính ExpireAt theo duration phút tính từ startAt.
        ///
        /// Cách dùng:
        /// - Nếu muốn tạo phòng tồn tại 30 phút:
        ///   var startAt = Now;
        ///   var expireAt = CalculateExpireAtFromMinutes(startAt, 30);
        ///   // gán entity.ExpireAt = expireAt;
        /// </summary>
        private static DateTime CalculateExpireAtFromMinutes(DateTime startAt, int minutes)
        {
            if (minutes <= 0) throw new ArgumentOutOfRangeException(nameof(minutes));
            return startAt.AddMinutes(minutes);
        }

        /// <summary>
        /// Map BreakoutRoom entity sang object response phục vụ FE.
        ///
        /// Cách dùng:
        /// - Dùng trong GetById / GetByParent / Add / Update để trả về dữ liệu đầy đủ:
        ///   + StartAt: thời điểm bắt đầu (CreatedDate nếu có, fallback = now)
        ///   + ExpireAt: thời điểm kết thúc (nullable)
        ///   + DurationMinutes: tổng thời lượng (nullable nếu ExpireAt null)
        ///   + RemainingMinutes: số phút còn lại (nullable nếu ExpireAt null)
        ///
        /// Lưu ý:
        /// - CreatedDate của EntityBase là DateTime? nên cần fallback tránh TimeSpan?
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
    }
}
