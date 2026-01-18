using Microsoft.EntityFrameworkCore;
using server.Domain.Entities;
using server.Domain.Enums;
using server.Infrastructure.Persistence;
using server.Service.Common.IServices;
using server.Service.Interfaces;
using server.Service.Models;
using server.Service.Models.Quizzes;
using server.Service.Utilities;
using System.Text.Json;

namespace server.Service.Services.Quizzes
{
    public class QuizService : BaseService, IQuizService
    {
        public QuizService(DataContext dataContext, IUserService userService)
            : base(dataContext, userService) { }

        public async Task<ApiResult> CreateQuiz(CreateQuizModel model)
        {
            if (model == null)
                return ApiResult.Fail("Dữ liệu không hợp lệ", "VALIDATION_ERROR");

            if (model.RoomId <= 0)
                return ApiResult.Fail("RoomId không hợp lệ", "VALIDATION_ERROR");

            if (string.IsNullOrWhiteSpace(model.Question))
                return ApiResult.Fail("Câu hỏi không được để trống", "VALIDATION_ERROR");

            if (model.Options == null || !model.Options.Any())
                return ApiResult.Fail("Danh sách đáp án không hợp lệ", "VALIDATION_ERROR");

            if (string.IsNullOrWhiteSpace(model.CorrectAnswer))
                return ApiResult.Fail("Đáp án đúng không được để trống", "VALIDATION_ERROR");

            var currentUserId = _userService.UserId;
            if (currentUserId <= 0)
                return ApiResult.Fail("Bạn chưa đăng nhập", "UNAUTHORIZED");

            try
            {
                var userRoom = await _dataContext.UserRooms
                    .FirstOrDefaultAsync(ur => ur.RoomId == model.RoomId
                                           && ur.UserId == currentUserId
                                           && ur.DeletedDate == null);

                if (userRoom == null)
                    return ApiResult.Fail("Bạn không phải thành viên phòng này", "NOT_IN_ROOM");

                if (userRoom.IsBan)
                    return ApiResult.Fail("Bạn đã bị cấm khỏi phòng này", "FORBIDDEN");

                // chỉ leader và deputy mới được tạo quiz
                if (userRoom.Role == RoomRole.RegularUser)
                    return ApiResult.Fail("Bạn không có quyền tạo quiz trong phòng này", "FORBIDDEN");

                var optionsJson = JsonSerializer.Serialize(model.Options);

                var quiz = new Quiz
                {
                    RoomId = model.RoomId,
                    Question = model.Question.Trim(),
                    OptionsJson = optionsJson,
                    CorrectAnswer = model.CorrectAnswer.Trim(),
                    TimeQuestionSeconds = model.TimeQuestionSeconds,
                    CreatedDate = Now,
                };

                _dataContext.Quizzes.Add(quiz);
                await SaveChangesAsync();

                return ApiResult.Success(quiz, "Tạo quiz thành công");
            }
            catch (Exception ex)
            {
                return ApiResult.Fail("Tạo quiz thất bại", "INTERNAL_ERROR", new[] { ex.Message });
            }
        }

        public async Task<ApiResult> UpdateQuiz(UpdateQuizModel model, CancellationToken ct)
        {
            if (model == null || model.Id <= 0)
                return ApiResult.Fail("Dữ liệu không hợp lệ", "VALIDATION_ERROR");

            if (string.IsNullOrWhiteSpace(model.Question))
                return ApiResult.Fail("Câu hỏi không được để trống", "VALIDATION_ERROR");

            if (model.Options == null || !model.Options.Any())
                return ApiResult.Fail("Danh sách đáp án không hợp lệ", "VALIDATION_ERROR");

            if (string.IsNullOrWhiteSpace(model.CorrectAnswer))
                return ApiResult.Fail("Đáp án đúng không được để trống", "VALIDATION_ERROR");

            var currentUserId = _userService.UserId;
            if (currentUserId <= 0)
                return ApiResult.Fail("Bạn chưa đăng nhập", "UNAUTHORIZED");

            try
            {
                var quiz = await _dataContext.Quizzes
                    .FirstOrDefaultAsync(q => q.Id == model.Id && q.DeletedDate == null, ct);

                if (quiz == null)
                    return ApiResult.Fail("Quiz không tồn tại", "QUIZ_NOT_FOUND");

                // check quyền user trong phòng
                var userRoom = await GetActiveUserRoomAsync(quiz.RoomId, currentUserId, ct);
                if (userRoom == null)
                    return ApiResult.Fail("Bạn không thuộc phòng này", "NOT_IN_ROOM");

                if (userRoom.Role == RoomRole.RegularUser)
                    return ApiResult.Fail("Bạn không có quyền sửa quiz trong phòng này", "FORBIDDEN");

                // update data
                quiz.Question = model.Question.Trim();
                quiz.OptionsJson = JsonSerializer.Serialize(model.Options);
                quiz.CorrectAnswer = model.CorrectAnswer.Trim();
                quiz.TimeQuestionSeconds = model.TimeQuestionSeconds;

                quiz.MarkUpdated();
                await SaveChangesAsync(ct);

                return ApiResult.Success(quiz, "Cập nhật thành công");
            }
            catch (Exception ex)
            {
                return ApiResult.Fail("Cập nhật quiz thất bại", "INTERNAL_ERROR", new[] { ex.Message });
            }
        }

        public async Task<ApiResult> DeleteQuiz(int quizId, CancellationToken ct = default)
        {
            if (quizId <= 0)
                return ApiResult.Fail("QuizId không hợp lệ", "VALIDATION_ERROR");

            var currentUserId = _userService.UserId;
            if (currentUserId <= 0)
                return ApiResult.Fail("Bạn chưa đăng nhập", "UNAUTHORIZED");

            try
            {
                var quiz = await _dataContext.Quizzes
                    .FirstOrDefaultAsync(q => q.Id == quizId && q.DeletedDate == null, ct);

                if (quiz == null)
                    return ApiResult.Fail("Quiz không tồn tại", "QUIZ_NOT_FOUND");

                var userRoom = await GetActiveUserRoomAsync(quiz.RoomId, currentUserId, ct);

                // Chỉ Leader/Deputy được xóa (bạn comment "chỉ Leader", nhưng code cũ đang chặn RegularUser => Leader+Deputy đều OK)
                if (userRoom == null)
                    return ApiResult.Fail("Bạn không thuộc phòng này", "NOT_IN_ROOM");

                if (userRoom.Role == RoomRole.RegularUser)
                    return ApiResult.Fail("Bạn không có quyền xóa quiz này", "FORBIDDEN");

                quiz.MarkDeleted();
                await SaveChangesAsync(ct);

                return ApiResult.Success(null, "Xóa quiz thành công");
            }
            catch (Exception ex)
            {
                return ApiResult.Fail("Xóa quiz thất bại", "INTERNAL_ERROR", new[] { ex.Message });
            }
        }

        public async Task<ApiResult> DeleteAllQuizzesInRoom(int roomId, CancellationToken ct = default)
        {
            if (roomId <= 0)
                return ApiResult.Fail("RoomId không hợp lệ", "VALIDATION_ERROR");

            var currentUserId = _userService.UserId;
            if (currentUserId <= 0)
                return ApiResult.Fail("Bạn chưa đăng nhập", "UNAUTHORIZED");

            try
            {
                var userRoom = await GetActiveUserRoomAsync(roomId, currentUserId, ct);

                // Chỉ Leader được clear
                if (userRoom == null)
                    return ApiResult.Fail("Bạn không thuộc phòng này", "NOT_IN_ROOM");

                if (userRoom.Role != RoomRole.GroupLeader)
                    return ApiResult.Fail("Chỉ Trưởng phòng mới có quyền xóa toàn bộ câu hỏi", "FORBIDDEN");

                var quizzes = await _dataContext.Quizzes
                    .Where(q => q.RoomId == roomId && q.DeletedDate == null)
                    .ToListAsync(ct);

                if (!quizzes.Any())
                    return ApiResult.Fail("Phòng này chưa có câu hỏi nào", "NO_DATA");

                foreach (var quiz in quizzes)
                    quiz.MarkDeleted();

                await SaveChangesAsync(ct);

                return ApiResult.Success(null, $"Đã xóa {quizzes.Count} câu hỏi");
            }
            catch (Exception ex)
            {
                return ApiResult.Fail("Xóa toàn bộ câu hỏi thất bại", "INTERNAL_ERROR", new[] { ex.Message });
            }
        }

        public async Task<ApiResult> GetAllQuizzesByRoom(int roomId, CancellationToken ct = default)
        {
            if (roomId <= 0)
                return ApiResult.Fail("RoomId không hợp lệ", "VALIDATION_ERROR");

            var currentUserId = _userService.UserId;
            if (currentUserId <= 0)
                return ApiResult.Fail("Bạn chưa đăng nhập", "UNAUTHORIZED");

            try
            {
                var userRoom = await GetActiveUserRoomAsync(roomId, currentUserId, ct);
                if (userRoom == null)
                    return ApiResult.Fail("Bạn không có quyền truy cập phòng này", "FORBIDDEN");

                var query = _dataContext.Quizzes.AsNoTracking()
                    .Where(q => q.RoomId == roomId && q.DeletedDate == null);

                // Nếu là học sinh, lọc bỏ bản nháp
                if (userRoom.Role == RoomRole.RegularUser)
                    query = query.Where(q => q.Status != QuizStatus.Draft);

                var quizzes = await query
                    .OrderByDescending(q => q.CreatedDate)
                    .ToListAsync(ct);

                if (!quizzes.Any())
                    return ApiResult.Success(new List<QuizModel>(), "Chưa có câu hỏi nào");

                var result = quizzes.Select(q =>
                {
                    var m = QuizHelper.MapQuizToModel(q);
                    QuizHelper.ApplyCorrectAnswer(m, q, userRoom.Role);
                    return m;
                }).ToList();

                return ApiResult.Success(result, "Lấy danh sách quiz thành công");
            }
            catch (Exception ex)
            {
                return ApiResult.Fail("Lấy danh sách quiz thất bại", "INTERNAL_ERROR", new[] { ex.Message });
            }
        }

        public async Task<ApiResult> GetQuizById(int id, CancellationToken ct = default)
        {
            if (id <= 0)
                return ApiResult.Fail("Id không hợp lệ", "VALIDATION_ERROR");

            var currentUserId = _userService.UserId;
            if (currentUserId <= 0)
                return ApiResult.Fail("Bạn chưa đăng nhập", "UNAUTHORIZED");

            try
            {
                var quiz = await _dataContext.Quizzes.AsNoTracking()
                    .FirstOrDefaultAsync(q => q.Id == id && q.DeletedDate == null, ct);

                if (quiz == null)
                    return ApiResult.Fail("Quiz không tồn tại", "QUIZ_NOT_FOUND");

                var userRoom = await GetActiveUserRoomAsync(quiz.RoomId, currentUserId, ct);
                if (userRoom == null)
                    return ApiResult.Fail("Bạn không có quyền truy cập quiz này", "FORBIDDEN");

                var dto = QuizHelper.MapQuizToModel(quiz);
                QuizHelper.ApplyCorrectAnswer(dto, quiz, userRoom.Role);

                return ApiResult.Success(dto, "Lấy quiz thành công");
            }
            catch (Exception ex)
            {
                return ApiResult.Fail("Lấy quiz thất bại", "INTERNAL_ERROR", new[] { ex.Message });
            }
        }

        public async Task<ApiResult> UpdateStatus(UpdateQuizStatusModel model, CancellationToken ct = default)
        {
            if (model == null || model.QuizId <= 0)
                return ApiResult.Fail("Dữ liệu không hợp lệ", "VALIDATION_ERROR");

            var currentUserId = _userService.UserId;
            if (currentUserId <= 0)
                return ApiResult.Fail("Bạn chưa đăng nhập", "UNAUTHORIZED");

            try
            {
                var quiz = await _dataContext.Quizzes
                    .FirstOrDefaultAsync(q => q.Id == model.QuizId && q.DeletedDate == null, ct);

                if (quiz == null)
                    return ApiResult.Fail("Quiz không tồn tại", "QUIZ_NOT_FOUND");

                var userRoom = await GetActiveUserRoomAsync(quiz.RoomId, currentUserId, ct);
                if (userRoom == null)
                    return ApiResult.Fail("Bạn không thuộc phòng này", "NOT_IN_ROOM");

                // Chỉ Leader mới được đóng/mở quiz
                if (userRoom.Role != RoomRole.GroupLeader)
                    return ApiResult.Fail("Bạn không có quyền thay đổi trạng thái Quiz này", "FORBIDDEN");

                quiz.Status = model.NewStatus;
                quiz.MarkUpdated();

                await SaveChangesAsync(ct);

                var msg = model.NewStatus == QuizStatus.Active ? "Đã bắt đầu Quiz" : "Đã đóng Quiz";
                return ApiResult.Success(new { quiz.Id, quiz.Status }, msg);
            }
            catch (Exception ex)
            {
                return ApiResult.Fail("Cập nhật trạng thái quiz thất bại", "INTERNAL_ERROR", new[] { ex.Message });
            }
        }

        public async Task<ApiResult> UpdateBulkStatus(UpdateBulkStatusModel model, CancellationToken ct = default)
        {
            if (model == null)
                return ApiResult.Fail("Dữ liệu không hợp lệ", "VALIDATION_ERROR");

            if (model.RoomId <= 0)
                return ApiResult.Fail("RoomId không hợp lệ", "VALIDATION_ERROR");

            if (model.QuizIds == null || !model.QuizIds.Any())
                return ApiResult.Fail("Danh sách QuizIds không hợp lệ", "VALIDATION_ERROR");

            var currentUserId = _userService.UserId;
            if (currentUserId <= 0)
                return ApiResult.Fail("Bạn chưa đăng nhập", "UNAUTHORIZED");

            try
            {
                var userRoom = await GetActiveUserRoomAsync(model.RoomId, currentUserId, ct);
                if (userRoom == null)
                    return ApiResult.Fail("Bạn không thuộc phòng này", "NOT_IN_ROOM");

                if (userRoom.Role != RoomRole.GroupLeader)
                    return ApiResult.Fail("Bạn không có quyền thay đổi trạng thái Quiz này", "FORBIDDEN");

                var quizzes = await _dataContext.Quizzes
                    .Where(q => q.RoomId == model.RoomId
                             && model.QuizIds.Contains(q.Id)
                             && q.DeletedDate == null)
                    .ToListAsync(ct);

                if (!quizzes.Any())
                    return ApiResult.Fail("Không tìm thấy câu hỏi nào hợp lệ", "NO_DATA");

                foreach (var quiz in quizzes)
                {
                    quiz.Status = model.NewStatus;
                    quiz.MarkUpdated();
                }

                await SaveChangesAsync(ct);

                return ApiResult.Success(null, $"Đã cập nhật trạng thái cho {quizzes.Count} câu hỏi");
            }
            catch (Exception ex)
            {
                return ApiResult.Fail("Cập nhật trạng thái hàng loạt thất bại", "INTERNAL_ERROR", new[] { ex.Message });
            }
        }

        // check user trong phòng và không bị ban
        private Task<UserRoom?> GetActiveUserRoomAsync(int roomId, int userId, CancellationToken ct = default)
        {
            return _dataContext.UserRooms.AsNoTracking()
                .FirstOrDefaultAsync(ur =>
                    ur.RoomId == roomId &&
                    ur.UserId == userId &&
                    !ur.IsBan &&
                    ur.DeletedDate == null, ct);
        }
    }
}
