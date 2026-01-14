using System.ComponentModel.DataAnnotations;


namespace server.Service.Models.Quizzes
{
    public class UpdateQuizModel
    {
        [Required]
        public int Id { get; set; }

        [Required(ErrorMessage = "Câu hỏi không được để trống")]
        public string Question { get; set; } // Câu hỏi

        [Required(ErrorMessage = "Phải có ít nhất 2 lựa chọn")]
        public List<string> Options { get; set; } // Các lựa chọn

        [Required(ErrorMessage = "Đáp án không được để trống")]
        public string CorrectAnswer { get; set; } // Đáp án

        [Range(10, 3600)]
        public int TimeQuestionSeconds { get; set; } // Thời gian hiển thị câu hỏi(giây)
    }
}