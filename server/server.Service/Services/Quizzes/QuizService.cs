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
    }


    
}
