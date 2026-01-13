using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using server.Domain.Base;

namespace server.Service.Models.Quizzes
{
    // hiển thị quiz
    public class QuizModel : EntityBase
    {
        public int RoomId { get; set; } // Phòng tạo quiz
        public string Question { get; set; } // Câu hỏi
        public List<string> Options { get; set; } // Các lựa chọn
        public int TimeQuestionSeconds { get; set; } // Thời gian hiển thị câu hỏi(giây)

        //nếu teacher thì hiện còn student thì null
        public string? CorrectAnswer { get; set; } // Đáp án
    }
}
