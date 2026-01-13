using server.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace server.Service.Models.Quizzes
{
    public class UpdateQuizStatusModel
    {
        public int QuizId { get; set; }
        public QuizStatus NewStatus { get; set; }
    }
}
