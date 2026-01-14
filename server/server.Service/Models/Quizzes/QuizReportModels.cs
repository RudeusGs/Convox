using server.Domain.Base;

namespace server.Service.Models.Quizzes
{
    // Model cho thống kê chi tiết 1 câu hỏi
    public class QuizStatsDto
    {
        public int QuizId { get; set; }
        public int TotalAnswers { get; set; } // Tổng số người đã trả lời
        public int CorrectCount { get; set; } // Số người trả lời đúng
        public int IncorrectCount { get; set; } // Số người trả lời sai
        public double AccuracyRate { get; set; } // Tỉ lệ đúng (%)
    }

    // Model danh sách bài làm của 1 câu hỏi (Leader)
    public class QuizSubmissionDto
    {
        public int UserId { get; set; }
        public string FullName { get; set; }
        public string Avatar { get; set; }
        public string Answer { get; set; }
        public bool IsCorrect { get; set; }
        public DateTime SubmittedAt { get; set; }
    }

    // Model bảng điểm
    public class RoomScoreboardDto
    {
        public int UserId { get; set; }
        public string FullName { get; set; }
        public string Avatar { get; set; }
        public int TotalQuestions { get; set; } // Tổng số câu trong phòng
        public int AnsweredCount { get; set; } // Số câu đã làm
        public int CorrectCount { get; set; } // Số câu đúng
        public double CompletionRate { get; set; } // Tỉ lệ hoàn thành (%)
    }

    // Model kết quả của cá nhân
    public class MyQuizResultDto : QuizModel
    {
        public bool IsAnswered { get; set; } // Đã làm
        public string? MyAnswer { get; set; } // Đáp án đã chọn
        public bool? IsCorrect { get; set; } // Đúng hay sai
        public DateTime? SubmittedAt { get; set; }
    }
}