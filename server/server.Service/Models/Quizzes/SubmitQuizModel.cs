using System.ComponentModel.DataAnnotations;


namespace server.Service.Models.Quizzes
{
    public class SubmitQuizModel
    {
        [Required]
        public int QuizId { get; set; }
        [Required(ErrorMessage = "Vui lòng chọn đáp án")]
        public string Answer { get; set; }
    }
}