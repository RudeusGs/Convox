using server.Service.Models.Quizzes;
using server.Service.Models;

namespace server.Service.Interfaces
{
    public interface IQuizResponseService
    {
        Task<ApiResult> SubmitQuiz(SubmitQuizModel model);
        // tỉ lệ đúng sai của 1 câu
        Task<ApiResult> GetQuizStats(int quizId, CancellationToken ct = default);

        // thống kê ai đã nộp câu quiz này (Chỉ GV)
        Task<ApiResult> GetQuizSubmissions(int quizId, CancellationToken ct = default);

        // bảng điểm tổng quát của phòng (Danh sách hs, tiến độ làm bài)
        Task<ApiResult> GetRoomScoreboard(int roomId, CancellationToken ct = default);

        // kết quả cá nhân
        Task<ApiResult> GetMyResults(int roomId, CancellationToken ct = default);
    }
}