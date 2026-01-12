using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using server.Service.Interfaces;
using server.Service.Models.Quizzes;

namespace server.Controllers
{
    [Authorize]
    public class QuizController : BaseController
    {
        private readonly IQuizService _quizService;

        public QuizController(IQuizService quizService)
        {
            _quizService = quizService;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateQuiz([FromBody] CreateQuizModel model)
        {
            // method FailResultFromErrors để trả lỗi validation trong BaseController
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                return FailResultFromErrors("Dữ liệu không hợp lệ", errors);
            }

            return FromApiResult(await _quizService.CreateQuiz(model));
        }

        [HttpPost("submit")]
        public async Task<IActionResult> SubmitQuiz([FromBody] SubmitQuizModel model)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                return FailResultFromErrors("Dữ liệu không hợp lệ", errors);
            }

            return FromApiResult(await _quizService.SubmitQuiz(model));
        }

    }
}
