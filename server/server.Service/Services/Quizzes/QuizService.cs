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

        // SUMMARY: Tạo quiz mới trong room. Chỉ GroupLeader/GroupDeputy được tạo. User phải thuộc room và không bị ban.
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

            if (userRoom.Role == RoomRole.RegularUser)
                return ApiResult.Fail("Bạn không có quyền tạo quiz trong phòng này");

            var quiz = new Quiz
            {
                RoomId = model.RoomId,
                Question = model.Question,
                OptionsJson = JsonSerializer.Serialize(model.Options),
                CorrectAnswer = model.CorrectAnswer,
                TimeQuestionSeconds = model.TimeQuestionSeconds,
                CreatedDate = Now,
            };

            _dataContext.Quizzes.Add(quiz);
            await SaveChangesAsync();

            return ApiResult.Success(quiz, "Tạo quiz thành công");
        }

        // SUMMARY: User nộp đáp án cho quiz. Quiz phải Active. User thuộc room và không bị ban. Chấm đúng/sai và lưu QuizResponse.
        public async Task<ApiResult> SubmitQuiz(SubmitQuizModel model)
        {
            var currentUserId = _userService.UserId;

            var quiz = await _dataContext.Quizzes.FindAsync(model.QuizId);
            if (quiz == null) return ApiResult.Fail("Quiz không tồn tại");

            if (quiz.Status != QuizStatus.Active)
                return ApiResult.Fail("Câu hỏi này chưa mở hoặc đã kết thúc, không thể nộp bài.");

            var userRoom = await _dataContext.UserRooms
                .FirstOrDefaultAsync(ur => ur.RoomId == quiz.RoomId
                                        && ur.UserId == currentUserId
                                        && ur.DeletedDate == null);

            if (userRoom == null || userRoom.IsBan)
                return ApiResult.Fail("Bạn không có quyền tham gia trả lời");

            bool isCorrect = model.Answer.Trim()
                .Equals(quiz.CorrectAnswer.Trim(), StringComparison.OrdinalIgnoreCase);

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

        // SUMMARY: Sửa quiz. User phải thuộc room, không bị ban. Chỉ GroupLeader/GroupDeputy được sửa. Soft update timestamp.
        public async Task<ApiResult> UpdateQuiz(UpdateQuizModel model, CancellationToken ct)
        {
            var currentUserId = _userService.UserId;

            var quiz = await _dataContext.Quizzes
                .FirstOrDefaultAsync(q => q.Id == model.Id && q.DeletedDate == null, ct);

            if (quiz == null)
                return ApiResult.Fail("Quiz không tồn tại");

            var userRoom = await GetActiveUserRoomAsync(quiz.RoomId, currentUserId, ct);
            if (userRoom == null)
                return ApiResult.Fail("Bạn không có truy cập phòng này");

            if (userRoom.Role == RoomRole.RegularUser)
                return ApiResult.Fail("Bạn không có quyền sửa quiz trong phòng này");

            quiz.Question = model.Question;
            quiz.OptionsJson = JsonSerializer.Serialize(model.Options);
            quiz.CorrectAnswer = model.CorrectAnswer;
            quiz.TimeQuestionSeconds = model.TimeQuestionSeconds;

            quiz.MarkUpdated();
            await SaveChangesAsync(ct);

            return ApiResult.Success(quiz, "Cập nhật thành công");
        }

        // SUMMARY: Xóa (soft delete) 1 quiz. User thuộc room. Role không được là RegularUser.
        public async Task<ApiResult> DeleteQuiz(int quizId, CancellationToken ct = default)
        {
            var currentUserId = _userService.UserId;

            var quiz = await _dataContext.Quizzes
                .FirstOrDefaultAsync(q => q.Id == quizId && q.DeletedDate == null, ct);

            if (quiz == null) return ApiResult.Fail("Quiz không tồn tại");

            var userRoom = await GetActiveUserRoomAsync(quiz.RoomId, currentUserId, ct);

            if (userRoom == null || userRoom.Role == RoomRole.RegularUser)
                return ApiResult.Fail("Bạn không có quyền xóa quiz này");

            quiz.MarkDeleted();
            await SaveChangesAsync(ct);

            return ApiResult.Success(null, "Xóa quiz thành công");
        }

        // SUMMARY: Xóa (soft delete) toàn bộ quiz trong room. Chỉ GroupLeader được phép.
        public async Task<ApiResult> DeleteAllQuizzesInRoom(int roomId, CancellationToken ct = default)
        {
            var currentUserId = _userService.UserId;

            var userRoom = await GetActiveUserRoomAsync(roomId, currentUserId, ct);

            if (userRoom == null || userRoom.Role != RoomRole.GroupLeader)
                return ApiResult.Fail("Chỉ Trưởng phòng mới có quyền xóa toàn bộ câu hỏi");

            var quizzes = await _dataContext.Quizzes
                .Where(q => q.RoomId == roomId && q.DeletedDate == null)
                .ToListAsync(ct);

            if (!quizzes.Any())
                return ApiResult.Fail("Phòng này chưa có câu hỏi nào");

            foreach (var quiz in quizzes)
            {
                quiz.MarkDeleted();
            }

            await SaveChangesAsync(ct);

            return ApiResult.Success(null, $"Đã xóa {quizzes.Count} câu hỏi");
        }

        // SUMMARY: Lấy danh sách quiz theo room. User phải thuộc room. RegularUser sẽ không thấy quiz Draft và không thấy CorrectAnswer.
        public async Task<ApiResult> GetAllQuizzesByRoom(int roomId, CancellationToken ct = default)
        {
            var currentUserId = _userService.UserId;

            var userRoom = await GetActiveUserRoomAsync(roomId, currentUserId, ct);
            if (userRoom == null)
                return ApiResult.Fail("Bạn không có quyền truy cập phòng này");

            var query = _dataContext.Quizzes.AsNoTracking()
                .Where(q => q.RoomId == roomId && q.DeletedDate == null);

            if (userRoom.Role == RoomRole.RegularUser)
                query = query.Where(q => q.Status != QuizStatus.Draft);

            var quizzes = await query.OrderByDescending(q => q.CreatedDate)
                .ToListAsync(ct);

            if (!quizzes.Any())
                return ApiResult.Success(new List<QuizModel>());

            var result = quizzes.Select(q =>
            {
                var model = MapQuizToModel(q);
                ApplyCorrectAnswer(model, q, userRoom.Role);
                return model;
            }).ToList();

            return ApiResult.Success(result);
        }

        // SUMMARY: Lấy chi tiết 1 quiz theo Id. User phải thuộc room. RegularUser không thấy CorrectAnswer.
        public async Task<ApiResult> GetQuizById(int id, CancellationToken ct = default)
        {
            var currentUserId = _userService.UserId;

            var quiz = await _dataContext.Quizzes.AsNoTracking()
                .FirstOrDefaultAsync(q => q.Id == id && q.DeletedDate == null, ct);

            if (quiz == null) return ApiResult.Fail("Quiz không tồn tại");

            var userRoom = await GetActiveUserRoomAsync(quiz.RoomId, currentUserId, ct);
            if (userRoom == null) return ApiResult.Fail("Bạn không có quyền truy cập quiz này");

            var dto = MapQuizToModel(quiz);
            ApplyCorrectAnswer(dto, quiz, userRoom.Role);

            return ApiResult.Success(dto);
        }

        // SUMMARY: Đổi trạng thái quiz (Active/Closed/...). Chỉ GroupLeader được phép.
        public async Task<ApiResult> UpdateStatus(UpdateQuizStatusModel model, CancellationToken ct = default)
        {
            var currentUserId = _userService.UserId;

            var quiz = await _dataContext.Quizzes
                .FirstOrDefaultAsync(q => q.Id == model.QuizId && q.DeletedDate == null, ct);

            if (quiz == null) return ApiResult.Fail("Quiz không tồn tại");

            var userRoom = await GetActiveUserRoomAsync(quiz.RoomId, currentUserId, ct);
            if (userRoom == null || userRoom.Role != RoomRole.GroupLeader)
                return ApiResult.Fail("Bạn không có quyền thay đổi trạng thái Quiz này");

            quiz.Status = model.NewStatus;
            quiz.MarkUpdated();

            await SaveChangesAsync(ct);

            var msg = model.NewStatus == QuizStatus.Active ? "Đã bắt đầu Quiz" : "Đã đóng Quiz";
            return ApiResult.Success(new { quiz.Id, quiz.Status }, msg);
        }

        // SUMMARY: Đổi trạng thái hàng loạt cho danh sách quiz trong room. Chỉ GroupLeader được phép.
        public async Task<ApiResult> UpdateBulkStatus(UpdateBulkStatusModel model, CancellationToken ct = default)
        {
            var currentUserId = _userService.UserId;

            var userRoom = await GetActiveUserRoomAsync(model.RoomId, currentUserId, ct);
            if (userRoom == null || userRoom.Role != RoomRole.GroupLeader)
                return ApiResult.Fail("Bạn không có quyền thay đổi trạng thái Quiz này");

            var quizzes = await _dataContext.Quizzes
                .Where(q => q.RoomId == model.RoomId
                         && model.QuizIds.Contains(q.Id)
                         && q.DeletedDate == null)
                .ToListAsync(ct);

            if (!quizzes.Any()) return ApiResult.Fail("Không tìm thấy câu hỏi nào hợp lệ");

            foreach (var quiz in quizzes)
            {
                quiz.Status = model.NewStatus;
                quiz.MarkUpdated();
            }

            await SaveChangesAsync(ct);

            return ApiResult.Success(null, $"Đã cập nhật trạng thái cho {quizzes.Count} câu hỏi");
        }

        // SUMMARY: Thống kê đúng/sai của 1 quiz. User phải thuộc room. Trả về total/correct/incorrect và accuracy %.
        public async Task<ApiResult> GetQuizStats(int quizId, CancellationToken ct = default)
        {
            var currentUserId = _userService.UserId;

            var quiz = await _dataContext.Quizzes.FindAsync(new object[] { quizId }, ct);
            if (quiz == null) return ApiResult.Fail("Quiz không tồn tại");

            var userRoom = await GetActiveUserRoomAsync(quiz.RoomId, currentUserId, ct);
            if (userRoom == null) return ApiResult.Fail("Bạn không có quyền xem");

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

            if (stats == null)
                return ApiResult.Success(new QuizStatsDto { QuizId = quizId });

            if (stats.TotalAnswers > 0)
                stats.AccuracyRate = Math.Round((double)stats.CorrectCount / stats.TotalAnswers * 100, 2);

            return ApiResult.Success(stats);
        }

        // SUMMARY: Lấy danh sách bài nộp cho 1 quiz. Chỉ GroupLeader/GroupDeputy được xem. Join user để lấy tên/avatar.
        public async Task<ApiResult> GetQuizSubmissions(int quizId, CancellationToken ct = default)
        {
            var currentUserId = _userService.UserId;

            var quiz = await _dataContext.Quizzes.FindAsync(new object[] { quizId }, ct);
            if (quiz == null) return ApiResult.Fail("Quiz không tồn tại");

            var userRoom = await GetActiveUserRoomAsync(quiz.RoomId, currentUserId, ct);
            if (userRoom == null || userRoom.Role == RoomRole.RegularUser)
                return ApiResult.Fail("Bạn không có quyền truy cập danh sách nộp bài");

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
                                         SubmittedAt = qr.CreatedDate ?? DateTime.Now
                                     }).ToListAsync(ct);

            return ApiResult.Success(submissions);
        }

        // SUMMARY: Bảng điểm tổng quát trong room (danh sách học viên + số câu đã làm + số câu đúng + % hoàn thành). Chỉ Leader/Deputy.
        public async Task<ApiResult> GetRoomScoreboard(int roomId, CancellationToken ct = default)
        {
            var currentUserId = _userService.UserId;

            var userRoom = await GetActiveUserRoomAsync(roomId, currentUserId, ct);
            if (userRoom == null || userRoom.Role == RoomRole.RegularUser)
                return ApiResult.Fail("Bạn không có quyền truy cập bảng điểm");

            var totalQuizzes = await _dataContext.Quizzes
                .CountAsync(q => q.RoomId == roomId && q.Status != QuizStatus.Draft && q.DeletedDate == null, ct);

            if (totalQuizzes == 0)
                return ApiResult.Success(new List<RoomScoreboardDto>(), "Chưa có câu hỏi nào");

            var students = await (from ur in _dataContext.UserRooms.AsNoTracking()
                                  join u in _dataContext.Users.AsNoTracking() on ur.UserId equals u.Id
                                  where ur.RoomId == roomId
                                     && ur.Role == RoomRole.RegularUser
                                     && ur.DeletedDate == null
                                  select new { u.Id, u.FullName, u.Avatar }).ToListAsync(ct);

            var roomQuizIds = _dataContext.Quizzes
                .Where(q => q.RoomId == roomId)
                .Select(q => q.Id);

            var allResponses = await _dataContext.QuizResponses.AsNoTracking()
                .Where(qr => roomQuizIds.Contains(qr.QuizId) && qr.DeletedDate == null)
                .Select(qr => new { qr.UserId, qr.IsCorrect })
                .ToListAsync(ct);

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
                    CompletionRate = totalQuizzes > 0
                        ? Math.Round((double)answered / totalQuizzes * 100, 1)
                        : 0
                };
            }).OrderByDescending(x => x.CorrectCount)
              .ThenByDescending(x => x.AnsweredCount)
              .ToList();

            return ApiResult.Success(result);
        }

        // SUMMARY: Xem kết quả cá nhân trong room. User phải thuộc room. Nếu đã làm thì trả về đáp án + trạng thái đúng/sai.
        public async Task<ApiResult> GetMyResults(int roomId, CancellationToken ct = default)
        {
            var currentUserId = _userService.UserId;

            var userRoom = await GetActiveUserRoomAsync(roomId, currentUserId, ct);
            if (userRoom == null) return ApiResult.Fail("Bạn không thuộc phòng này");

            var quizzes = await _dataContext.Quizzes.AsNoTracking()
                .Where(q => q.RoomId == roomId && q.Status != QuizStatus.Draft && q.DeletedDate == null)
                .OrderByDescending(q => q.CreatedDate)
                .ToListAsync(ct);

            var quizIds = quizzes.Select(q => q.Id).ToList();

            var myResponses = await _dataContext.QuizResponses.AsNoTracking()
                .Where(qr => quizIds.Contains(qr.QuizId)
                          && qr.UserId == currentUserId
                          && qr.DeletedDate == null)
                .ToListAsync(ct);

            var result = quizzes.Select(q =>
            {
                var dto = new MyQuizResultDto
                {
                    Id = q.Id,
                    Question = q.Question,
                    CreatedDate = q.CreatedDate,
                    TimeQuestionSeconds = q.TimeQuestionSeconds,
                    Options = string.IsNullOrEmpty(q.OptionsJson)
                        ? new List<string>()
                        : JsonSerializer.Deserialize<List<string>>(q.OptionsJson),
                    CorrectAnswer = null
                };

                var response = myResponses.FirstOrDefault(r => r.QuizId == q.Id);
                if (response != null)
                {
                    dto.IsAnswered = true;
                    dto.MyAnswer = response.Answer;
                    dto.IsCorrect = response.IsCorrect;
                    dto.SubmittedAt = response.CreatedDate;
                    dto.CorrectAnswer = q.CorrectAnswer;
                }
                else
                {
                    dto.IsAnswered = false;
                }

                return dto;
            }).ToList();

            return ApiResult.Success(result);
        }

        private Task<UserRoom?> GetActiveUserRoomAsync(int roomId, int userId, CancellationToken ct = default)
        {
            return _dataContext.UserRooms.AsNoTracking()
                .FirstOrDefaultAsync(ur =>
                    ur.RoomId == roomId &&
                    ur.UserId == userId &&
                    !ur.IsBan &&
                    ur.DeletedDate == null, ct);
        }

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
                model.Options = JsonSerializer.Deserialize<List<string>>(quiz.OptionsJson) ?? new List<string>();

            return model;
        }

        private static void ApplyCorrectAnswer(QuizModel model, Quiz quiz, RoomRole role)
        {
            model.CorrectAnswer = null;

            if (role == RoomRole.GroupLeader || role == RoomRole.GroupDeputy)
                model.CorrectAnswer = quiz.CorrectAnswer;
        }
    }
}
