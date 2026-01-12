using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
