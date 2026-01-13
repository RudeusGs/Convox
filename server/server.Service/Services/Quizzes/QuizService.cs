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

        public async Task<ApiResult> GetAllQuizzesByRoom(int roomId, CancellationToken ct = default)
        {
            var currentUserId = _userService.UserId;

            //check user trong phòng
            var userRoom = await GetActiveUserRoomAsync(roomId, currentUserId, ct);

            if (userRoom == null)
                return ApiResult.Fail("Bạn không có quyền truy cập phòng này");

            //lấy danh sách quiz
            var quizzes = await _dataContext.Quizzes.AsNoTracking()
                .Where(q => q.RoomId == roomId && q.DeletedDate == null)
                .OrderByDescending(q => q.CreatedDate)
                .ToListAsync(ct);

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
