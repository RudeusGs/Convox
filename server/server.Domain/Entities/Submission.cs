using server.Domain.Base;

namespace server.Domain.Entities
{
    // Submission bài tập
    public class Submission : EntityBase
    {
        public int AssignmentId { get; set; } // Id bài tập
        public int UserId { get; set; } // Ai nộp
        public string FilePath { get; set; } // File nộp
        public decimal? Grade { get; set; } // Điểm
        public string Feedback { get; set; } // Nhận xét
    }
}
