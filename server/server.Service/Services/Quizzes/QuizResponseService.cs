using Microsoft.EntityFrameworkCore;
using server.Domain.Entities;
using server.Domain.Enums;
using server.Infrastructure.Persistence;
using server.Service.Common.IServices;
using server.Service.Models.Quizzes;
using server.Service.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using server.Service.Interfaces;

namespace server.Service.Services.Quizzes
{
    public class QuizResponseService : BaseService, IQuizResponseService
    {
        public QuizResponseService(DataContext dataContext, IUserService userService)
            : base(dataContext, userService) { }

        public async Task<ApiResult> SubmitQuiz(SubmitQuizModel model)
        {
            var currentUserId = _userService.UserId;

            var quiz = await _dataContext.Quizzes.FindAsync(model.QuizId);

            if (quiz == null) return ApiResult.Fail("Quiz không tồn tại", "QUIZ_NOT_FOUND");

            if (quiz.Status != QuizStatus.Active)
            {
                return ApiResult.Fail("Câu hỏi này chưa mở hoặc đã kết thúc, không thể nộp bài.", "QUIZ_NOT_ACTIVE");
            }
            //check quyền user trong phòng
            var userRoom = await _dataContext.UserRooms
                .FirstOrDefaultAsync(ur => ur.RoomId == quiz.RoomId
                                        && ur.UserId == currentUserId
                                        && ur.DeletedDate == null);

            if (userRoom == null || userRoom.IsBan)
                return ApiResult.Fail("Bạn không có quyền tham gia trả lời", "QUIZ_ACCESS_DENIED");

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

        // thống kê tỉ lệ đúng sai của 1 câu hỏi
        public async Task<ApiResult> GetQuizStats(int quizId, CancellationToken ct = default)
        {
            var currentUserId = _userService.UserId;

            // Check quyền
            var quiz = await _dataContext.Quizzes.FindAsync(new object[] { quizId }, ct);
            if (quiz == null) return ApiResult.Fail("Quiz không tồn tại", "QUIZ_NOT_FOUND");

            var userRoom = await GetActiveUserRoomAsync(quiz.RoomId, currentUserId, ct);
            if (userRoom == null) return ApiResult.Fail("Bạn không có quyền xem", "QUIZ_ACCESS_DENIED");

            // Lấy dữ liệu thống kê
            var stats = await _dataContext.QuizResponses.AsNoTracking()
                .Where(qr => qr.QuizId == quizId && qr.DeletedDate == null)
                .GroupBy(qr => qr.QuizId)
                .Select(g => new QuizStatsDto
                {
                    QuizId = quizId,
                    TotalAnswers = g.Count(),
                    CorrectCount = g.Count(x => x.IsCorrect),
                    IncorrectCount = g.Count(x => !x.IsCorrect)
                }).FirstOrDefaultAsync(ct);

            if (stats == null) // Chưa ai làm
            {
                return ApiResult.Success(new QuizStatsDto { QuizId = quizId }, "Chưa có câu trả lời");
            }

            // Tính %
            if (stats.TotalAnswers > 0)
            {
                stats.AccuracyRate = Math.Round((double)stats.CorrectCount / stats.TotalAnswers * 100, 2);
            }

            return ApiResult.Success(stats, "Lấy thống kê thành công");
        }

        // danh sách bài nộp của 1 câu
        public async Task<ApiResult> GetQuizSubmissions(int quizId, CancellationToken ct = default)
        {
            var currentUserId = _userService.UserId;

            var quiz = await _dataContext.Quizzes.FindAsync(new object[] { quizId }, ct);
            if (quiz == null) return ApiResult.Fail("Quiz không tồn tại", "QUIZ_NOT_FOUND");

            // Chỉ Leader/Deputy được xem chi tiết
            var userRoom = await GetActiveUserRoomAsync(quiz.RoomId, currentUserId, ct);
            if (userRoom == null || userRoom.Role == RoomRole.RegularUser)
                return ApiResult.Fail("Bạn không có quyền truy cập danh sách nộp bài", "QUIZ_ACCESS_DENIED");

            // lấy thông tin user + câu trả lời
            var submissions = await (from qr in _dataContext.QuizResponses.AsNoTracking()
                                     join u in _dataContext.Users.AsNoTracking() on qr.UserId equals u.Id
                                     where qr.QuizId == quizId && qr.DeletedDate == null
                                     orderby qr.CreatedDate descending
                                     select new QuizSubmissionDto
                                     {
                                         UserId = u.Id,
                                         FullName = u.FullName ?? "Unknown",
                                         Avatar = u.Avatar,
                                         Answer = qr.Answer,
                                         IsCorrect = qr.IsCorrect,
                                         SubmittedAt = qr.CreatedDate ?? DateTime.UtcNow
                                     }).ToListAsync(ct);

            return ApiResult.Success(submissions, $"Lấy danh sách bài nộp của quiz {quizId} thành công");
        }

        // bảng điểm tổng quát (Danh sách hs, tiến độ làm bài)
        public async Task<ApiResult> GetRoomScoreboard(int roomId, CancellationToken ct = default)
        {
            var currentUserId = _userService.UserId;

            // Check quyền
            var userRoom = await GetActiveUserRoomAsync(roomId, currentUserId, ct);
            if (userRoom == null || userRoom.Role == RoomRole.RegularUser)
                return ApiResult.Fail("Bạn không có quyền truy cập bảng điểm", "QUIZ_ACCESS_DENIED");

            // Đếm tổng số quiz active trong phòng
            var totalQuizzes = await _dataContext.Quizzes
                .CountAsync(q => q.RoomId == roomId && q.Status != QuizStatus.Draft && q.DeletedDate == null, ct);

            if (totalQuizzes == 0) return ApiResult.Success(new List<RoomScoreboardDto>(), "Chưa có câu hỏi nào");

            // Lấy danh sách học viên trong phòng
            var students = await (from ur in _dataContext.UserRooms.AsNoTracking()
                                  join u in _dataContext.Users.AsNoTracking() on ur.UserId equals u.Id
                                  where ur.RoomId == roomId && ur.Role == RoomRole.RegularUser && ur.DeletedDate == null
                                  select new { u.Id, u.FullName, u.Avatar }).ToListAsync(ct);

            // Lấy tất cả kết quả làm bài trong phòng này
            var roomQuizIds = _dataContext.Quizzes.Where(q => q.RoomId == roomId).Select(q => q.Id);

            var allResponses = await _dataContext.QuizResponses.AsNoTracking()
                .Where(qr => roomQuizIds.Contains(qr.QuizId) && qr.DeletedDate == null)
                .Select(qr => new { qr.UserId, qr.IsCorrect })
                .ToListAsync(ct);

            // Tính điểm số cho từng user
            var result = students.Select(s =>
            {
                var studentResponses = allResponses.Where(r => r.UserId == s.Id).ToList();
                var answered = studentResponses.Count;
                var correct = studentResponses.Count(r => r.IsCorrect);

                return new RoomScoreboardDto
                {
                    UserId = s.Id,
                    FullName = s.FullName ?? "Unknown",
                    Avatar = s.Avatar,
                    TotalQuestions = totalQuizzes,
                    AnsweredCount = answered,
                    CorrectCount = correct,
                    CompletionRate = totalQuizzes > 0 ? Math.Round((double)answered / totalQuizzes * 100, 1) : 0
                };
            }).OrderByDescending(x => x.CorrectCount).ThenByDescending(x => x.AnsweredCount).ToList();

            return ApiResult.Success(result, "Lấy bảng điểm thành công");
        }

        // xem kết quả cá nhân
        public async Task<ApiResult> GetMyResults(int roomId, CancellationToken ct = default)
        {
            var currentUserId = _userService.UserId;

            // Check quyền
            var userRoom = await GetActiveUserRoomAsync(roomId, currentUserId, ct);
            if (userRoom == null) return ApiResult.Fail("Bạn không thuộc phòng này", "QUIZ_ACCESS_DENIED");

            // Lấy tất cả Quiz trong phòng
            var quizzes = await _dataContext.Quizzes.AsNoTracking()
                .Where(q => q.RoomId == roomId && q.Status != QuizStatus.Draft && q.DeletedDate == null)
                .OrderByDescending(q => q.CreatedDate)
                .ToListAsync(ct);

            // Lấy câu trả lời của user này
            var quizIds = quizzes.Select(q => q.Id).ToList();
            var myResponses = await _dataContext.QuizResponses.AsNoTracking()
                .Where(qr => quizIds.Contains(qr.QuizId) && qr.UserId == currentUserId && qr.DeletedDate == null)
                .ToListAsync(ct);

            // Map data
            var result = quizzes.Select(q =>
            {
                // Map cơ bản từ Quiz sang DTO
                var dto = new MyQuizResultDto
                {
                    Id = q.Id,
                    Question = q.Question,
                    CreatedDate = q.CreatedDate,
                    TimeQuestionSeconds = q.TimeQuestionSeconds,
                    Options = string.IsNullOrEmpty(q.OptionsJson)
                              ? new List<string>()
                              : JsonSerializer.Deserialize<List<string>>(q.OptionsJson),

                    // Nếu đã làm rồi thì hiện đáp án, chưa thì ẩn
                    CorrectAnswer = null
                };

                var response = myResponses.FirstOrDefault(r => r.QuizId == q.Id);
                if (response != null)
                {
                    dto.IsAnswered = true;
                    dto.MyAnswer = response.Answer;
                    dto.IsCorrect = response.IsCorrect;
                    dto.SubmittedAt = response.CreatedDate;
                    dto.CorrectAnswer = q.CorrectAnswer; // Đã làm xong thì cho xem đáp án
                }
                else
                {
                    dto.IsAnswered = false;
                }

                return dto;
            }).ToList();

            return ApiResult.Success(result, "Lấy kết quả bài làm thành công");
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
    }
}
