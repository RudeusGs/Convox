using server.Domain.Base;

namespace server.Domain.Entities
{
    public class QuizResponse : EntityBase
    {
        public int QuizId { get; set; } // Id quiz
        public int UserId { get; set; } // Ai trả lời
        public string Answer { get; set; } // Trả lời
        public bool IsCorrect { get; set; } // Đúng / sai
    }
}
