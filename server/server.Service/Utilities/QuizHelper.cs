using server.Domain.Entities;
using server.Domain.Enums;
using server.Service.Models.Quizzes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace server.Service.Utilities
{
    public static class QuizHelper
    {
        // Hàm mapping quiz
        public static QuizModel MapQuizToModel(Quiz quiz)
        {
            var model = new QuizModel
            {
                Id = quiz.Id,
                RoomId = quiz.RoomId,
                Question = quiz.Question,
                TimeQuestionSeconds = quiz.TimeQuestionSeconds,
                CreatedDate = quiz.CreatedDate,
                Options = new List<string>()
            };

            if (!string.IsNullOrEmpty(quiz.OptionsJson))
            {
                model.Options = JsonSerializer.Deserialize<List<string>>(quiz.OptionsJson) ?? new List<string>();
            }
            return model;
        }

        // check quyền xem đáp án
        public static void ApplyCorrectAnswer(QuizModel model, Quiz quiz, RoomRole role)
        {
            model.CorrectAnswer = null;

            if (role == RoomRole.GroupLeader || role == RoomRole.GroupDeputy)
                model.CorrectAnswer = quiz.CorrectAnswer;
        }


        public static (bool IsValid, string? ErrorMessage) ValidateQuizData(List<string> options, string correctAnswer)
        {
            if (options == null || options.Count < 2)
            {
                return (false, "Câu hỏi phải có ít nhất 2 lựa chọn.");
            }

            // Check đáp án nằm trong list options
            bool exists = options.Any(o => o.Trim().Equals(correctAnswer.Trim(), StringComparison.OrdinalIgnoreCase));

            if (!exists)
            {
                return (false, $"Đáp án đúng '{correctAnswer}' không nằm trong danh sách lựa chọn.");
            }

            // Check trùng
            var distinctCount = options.Select(o => o.Trim().ToLower()).Distinct().Count();
            if (distinctCount != options.Count)
            {
                return (false, "Các lựa chọn không được trùng nhau.");
            }

            return (true, null);
        }
    }
}
