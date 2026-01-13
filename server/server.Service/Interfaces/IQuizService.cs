using server.Service.Models;
using server.Service.Models.Quizzes;

namespace server.Service.Interfaces
{
    public interface IQuizService
    {
        Task<ApiResult> CreateQuiz(CreateQuizModel model);
        Task<ApiResult> SubmitQuiz(SubmitQuizModel model);

        Task<ApiResult> GetAllQuizzesByRoom(int roomId, CancellationToken ct = default);
        Task<ApiResult> GetQuizById(int id, CancellationToken ct = default);
    }
}
