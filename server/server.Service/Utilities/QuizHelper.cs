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
    }
}
