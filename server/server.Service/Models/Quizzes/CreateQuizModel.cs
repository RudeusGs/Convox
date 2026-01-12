using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace server.Service.Models.Quizzes
{
    public class CreateQuizModel
    {
        [Required]
        public int RoomId { get; set; }

        [Required(ErrorMessage = "Câu hỏi không được để trống")]
        public string Question { get; set; }

        [Required]
        public List<string> Options { get; set; } // nhận danh sách các lựa chọn từ client

        [Required]
        public string CorrectAnswer { get; set; }

        [Range(10, 3600, ErrorMessage = "Thời gian tối thiểu 10s")]
        public int TimeQuestionSeconds { get; set; } = 30;
    }
}
