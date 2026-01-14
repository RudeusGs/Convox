using server.Domain.Enums;

namespace server.Service.Models.Quizzes
{
    public class UpdateQuizStatusModel
    {
        public int QuizId { get; set; }
        public QuizStatus NewStatus { get; set; }
    }
}