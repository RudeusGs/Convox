using server.Domain.Base;

namespace server.Domain.Entities
{
    // Quiz / Poll
    public class Quiz : EntityBase
    {
        public int RoomId { get; set; } // Phòng tạo quiz
        public string Question { get; set; } // Câu hỏi
        public string OptionsJson { get; set; } // Lưu các lựa chọn dưới dạng JSON
        public string CorrectAnswer { get; set; } // Đáp án
        public int TimeQuestionSeconds { get; set; } // Thời gian hiển thị câu hỏi(giây)
    }
}
