using Microsoft.EntityFrameworkCore;
using server.Domain.Entities;
using server.Domain.Enums;
using server.Infrastructure.Persistence;
using server.Service.Common.IServices;
using server.Service.Common.Services;
using server.Service.Interfaces;
using server.Service.Models;
using server.Service.Models.Quizzes;
using System.Text.Json;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace server.Service.Services.Quizzes
{
    public class QuizService : BaseService, IQuizService
    {
        public QuizService(DataContext dataContext, IUserService userService)
            : base(dataContext, userService) { }

        public async Task<ApiResult> CreateQuiz(CreateQuizModel model)
        {
            var currentUserId = _userService.UserId;

            var userRoom = await _dataContext.UserRooms
                .FirstOrDefaultAsync(ur => ur.RoomId == model.RoomId
                                        && ur.UserId == currentUserId
                                        && ur.DeletedDate == null);

            if (userRoom == null)
                return ApiResult.Fail("Bạn không phải thành viên phòng này");

            if (userRoom.IsBan)
                return ApiResult.Fail("Bạn đã bị cấm khỏi phòng này");

            // chỉ leader và deputy mới được tạo quiz
            if (userRoom.Role == RoomRole.RegularUser)
                return ApiResult.Fail("Bạn không có quyền tạo quiz trong phòng này");

            var optionsJson = JsonSerializer.Serialize(model.Options);

            var quiz = new Quiz
            {
                RoomId = model.RoomId,
                Question = model.Question,
                OptionsJson = optionsJson,
                CorrectAnswer = model.CorrectAnswer,
                TimeQuestionSeconds = model.TimeQuestionSeconds,
                CreatedDate = Now,

            };

            _dataContext.Quizzes.Add(quiz);
            await SaveChangesAsync();

            return ApiResult.Success(quiz);

        }

        public async Task<ApiResult> SubmitQuiz(SubmitQuizModel model)
        {
            var currentUserId = _userService.UserId;

            var quiz = await _dataContext.Quizzes.FindAsync(model.QuizId);

            if (quiz == null) return ApiResult.Fail("Quiz không tồn tại");

            //check quyền user trong phòng
            var userRoom = await _dataContext.UserRooms
                .FirstOrDefaultAsync(ur => ur.RoomId == quiz.RoomId
                                        && ur.UserId == currentUserId
                                        && ur.DeletedDate == null);

            if (userRoom == null || userRoom.IsBan)
                return ApiResult.Fail("Bạn không có quyền tham gia trả lời");

            //check đáp áp, không phân biệt hoa thường và khoảng trắng
            bool isCorrect = model.Answer.Trim().Equals(quiz.CorrectAnswer.Trim(), StringComparison.OrdinalIgnoreCase);

            var response = new QuizResponse
            {
                QuizId = model.QuizId,
                UserId = currentUserId,
                Answer = model.Answer,
                IsCorrect = isCorrect,
                CreatedDate = Now
            };

            _dataContext.QuizResponses.Add(response);
            await SaveChangesAsync();

            return ApiResult.Success(new { IsCorrect = isCorrect }, isCorrect ? "Chính xác!" : "Rất tiếc, sai rồi!");
        }

        public async Task<ApiResult> UpdateQuiz(UpdateQuizModel model, CancellationToken ct)
        {
            var currentUserId = _userService.UserId;

            var quiz = await _dataContext.Quizzes
                .FirstOrDefaultAsync(q => q.Id == model.Id && q.DeletedDate == null, ct);

            if (quiz == null)
                return ApiResult.Fail("Quiz không tồn tại");

            //check quyền user trong phòng
            var userRoom = await GetActiveUserRoomAsync(quiz.RoomId, currentUserId, ct);

            if (userRoom == null)
                return ApiResult.Fail("Bạn không có truy cập phòng này");

            if (userRoom.Role == RoomRole.RegularUser)
                return ApiResult.Fail("Bạn không có quyền sửa quiz trong phòng này");

            // update data
            quiz.Question = model.Question;
            quiz.OptionsJson = JsonSerializer.Serialize(model.Options); // Serialize lại JSON
            quiz.CorrectAnswer = model.CorrectAnswer;
            quiz.TimeQuestionSeconds = model.TimeQuestionSeconds;

            // update ngày
            quiz.MarkUpdated();
            await SaveChangesAsync(ct);

            return ApiResult.Success(quiz, "Cập nhật thành công");

        }

        public async Task<ApiResult> DeleteQuiz(int quizId, CancellationToken ct = default)
        {
            var currentUserId = _userService.UserId;

            var quiz = await _dataContext.Quizzes
                .FirstOrDefaultAsync(q => q.Id == quizId && q.DeletedDate == null, ct);

            if (quiz == null) return ApiResult.Fail("Quiz không tồn tại");

            // Check quyền
            var userRoom = await GetActiveUserRoomAsync(quiz.RoomId, currentUserId, ct);

            // Chỉ Leader mới được xóa
            if (userRoom == null || userRoom.Role == RoomRole.RegularUser)
                return ApiResult.Fail("Bạn không có quyền xóa quiz này");

            quiz.MarkDeleted();

            await SaveChangesAsync(ct);

            return ApiResult.Success(null, "Xóa quiz thành công");
        }

        public async Task<ApiResult> DeleteAllQuizzesInRoom(int roomId, CancellationToken ct = default)
        {
            var currentUserId = _userService.UserId;

            // Check quyền
            var userRoom = await GetActiveUserRoomAsync(roomId, currentUserId, ct);

            // Chỉ Leader được clear
            if (userRoom == null || userRoom.Role != RoomRole.GroupLeader)
                return ApiResult.Fail("Chỉ Trưởng phòng mới có quyền xóa toàn bộ câu hỏi");

            // Lấy tất cả quiz chưa xóa (vì MarkDeleted chỉ ẩn chứ không xóa hẳn khỏi db)
            var quizzes = await _dataContext.Quizzes
                .Where(q => q.RoomId == roomId && q.DeletedDate == null)
                .ToListAsync(ct);

            if (!quizzes.Any())
                return ApiResult.Fail("Phòng này chưa có câu hỏi nào");

            // Đánh dấu xóa tất cả
            foreach (var quiz in quizzes)
            {
                quiz.MarkDeleted();
            }

            await SaveChangesAsync(ct);

            return ApiResult.Success(null, $"Đã xóa {quizzes.Count} câu hỏi");
        }

        public async Task<ApiResult> GetAllQuizzesByRoom(int roomId, CancellationToken ct = default)
        {
            var currentUserId = _userService.UserId;

            //check user trong phòng
            var userRoom = await GetActiveUserRoomAsync(roomId, currentUserId, ct);

            if (userRoom == null)
                return ApiResult.Fail("Bạn không có quyền truy cập phòng này");

            var query = _dataContext.Quizzes.AsNoTracking()
                .Where(q => q.RoomId == roomId && q.DeletedDate == null);

            // Nếu là học sinh, lọc bỏ bản nháp
            if (userRoom.Role == RoomRole.RegularUser)
            {
                query = query.Where(q => q.Status != QuizStatus.Draft);
            }

            //lấy danh sách quiz
            var quizzes = await query.OrderByDescending(q => q.CreatedDate)
                .ToListAsync(ct);

            // trả về list rỗng, không hiện lỗi quiz trống vì quiz có thể null
            if (!quizzes.Any() || quizzes.Count == 0)
                return ApiResult.Success(new List<QuizModel>()); 

            // convert sang DTO xử lý json options với List<string>
            var result = quizzes.Select(q =>
            {
                var model = MapQuizToModel(q);

                // Chỉ Leader / Deputy mới được xem đáp án
                ApplyCorrectAnswer(model, q, userRoom.Role);

                return model;
            }).ToList();

            return ApiResult.Success(result);
        }

        public async Task<ApiResult> GetQuizById(int id, CancellationToken ct = default)
        {
            var currentUserId = _userService.UserId;

            var quiz = await _dataContext.Quizzes.AsNoTracking()
                .FirstOrDefaultAsync(q => q.Id == id && q.DeletedDate == null, ct);

            if (quiz == null) return ApiResult.Fail("Quiz không tồn tại");

            // Check quyền trong phòng chứa quiz này
            var userRoom = await GetActiveUserRoomAsync(quiz.RoomId, currentUserId, ct);

            if (userRoom == null) return ApiResult.Fail("Bạn không có quyền truy cập quiz này");

            // Map sang DTO
            var dto = MapQuizToModel(quiz);
            // học viên thì ẩn đáp án
            ApplyCorrectAnswer(dto, quiz, userRoom.Role);

            return ApiResult.Success(dto);
        }

        public async Task<ApiResult> UpdateStatus(UpdateQuizStatusModel model, CancellationToken ct = default)
        {
            var currentUserId = _userService.UserId;

            var quiz = await _dataContext.Quizzes
                .FirstOrDefaultAsync(q => q.Id == model.QuizId && q.DeletedDate == null, ct);

            if (quiz == null) return ApiResult.Fail("Quiz không tồn tại");

            // Check quyền: Chỉ Leader mới được đóng/mở quiz
            var userRoom = await GetActiveUserRoomAsync(quiz.RoomId, currentUserId, ct);
            if (userRoom == null || userRoom.Role != RoomRole.GroupLeader)
                return ApiResult.Fail("Bạn không có quyền thay đổi trạng thái Quiz này");

            // update status
            quiz.Status = model.NewStatus;
            quiz.MarkUpdated();

            await SaveChangesAsync(ct);

            string msg;
            if (model.NewStatus == QuizStatus.Active)
            {
                msg = "Đã bắt đầu Quiz";
            }
            else
            {
                msg = "Đã đóng Quiz";
            }
            return ApiResult.Success(new { quiz.Id, quiz.Status }, msg);
        }

        //check user trong phòng và không bị ban
        private Task<UserRoom?> GetActiveUserRoomAsync(int roomId, int userId, CancellationToken ct = default)
        {
            return _dataContext.UserRooms.AsNoTracking()
                .FirstOrDefaultAsync(ur =>
                    ur.RoomId == roomId &&
                    ur.UserId == userId &&
                    !ur.IsBan &&
                    ur.DeletedDate == null, ct);
        }

        // mapping Quiz 
        private static QuizModel MapQuizToModel(Quiz quiz)
        {
            var model = new QuizModel
            {
                Id = quiz.Id,
                RoomId = quiz.RoomId,
                Question = quiz.Question,
                TimeQuestionSeconds = quiz.TimeQuestionSeconds,
                CreatedDate = quiz.CreatedDate,
                Options = new List<string>()
            };

            if (!string.IsNullOrEmpty(quiz.OptionsJson))
            {
                model.Options = JsonSerializer.Deserialize<List<string>>(quiz.OptionsJson) ?? new List<string>();
            }

            return model;
        }

        // check quyền xem đáp án
        private static void ApplyCorrectAnswer(QuizModel model, Quiz quiz, RoomRole role)
        {
            model.CorrectAnswer = null;

            if (role == RoomRole.GroupLeader || role == RoomRole.GroupDeputy)
                model.CorrectAnswer = quiz.CorrectAnswer;
        }
    }
}
