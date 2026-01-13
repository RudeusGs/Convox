using server.Service.Models;
using server.Service.Models.Quizzes;

namespace server.Service.Interfaces
{
    public interface IQuizService
    {
        Task<ApiResult> CreateQuiz(CreateQuizModel model);
        Task<ApiResult> SubmitQuiz(SubmitQuizModel model);
        Task<ApiResult> UpdateQuiz(UpdateQuizModel model, CancellationToken ct = default);

        Task<ApiResult> DeleteQuiz(int quizId, CancellationToken ct = default);

        // Clear quiz
        Task<ApiResult> DeleteAllQuizzesInRoom(int roomId, CancellationToken ct = default);

        Task<ApiResult> GetAllQuizzesByRoom(int roomId, CancellationToken ct = default);
        Task<ApiResult> GetQuizById(int id, CancellationToken ct = default);
        Task<ApiResult> UpdateStatus(UpdateQuizStatusModel model, CancellationToken ct = default);
        Task<ApiResult> UpdateBulkStatus(UpdateBulkStatusModel model, CancellationToken ct = default);
    }
}
