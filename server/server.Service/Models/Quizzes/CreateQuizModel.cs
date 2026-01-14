using System.ComponentModel.DataAnnotations;


namespace server.Service.Models.Quizzes
{
    public class CreateQuizModel
    {
        [Required]
        public int RoomId { get; set; }

        [Required(ErrorMessage = "Câu hỏi không được để trống")]
        public string Question { get; set; }

        [Required]
        public List<string> Options { get; set; }

        [Required]
        public string CorrectAnswer { get; set; }

        [Range(10, 3600, ErrorMessage = "Thời gian tối thiểu 10s")]
        public int TimeQuestionSeconds { get; set; } = 30;
    }
}