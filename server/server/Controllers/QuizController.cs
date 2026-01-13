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

        [HttpGet("room/{roomId}")]
        public async Task<IActionResult> GetAllByRoom(int roomId, CancellationToken ct)
        {
            return FromApiResult(await _quizService.GetAllQuizzesByRoom(roomId, ct));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id, CancellationToken ct)
        {
            return FromApiResult(await _quizService.GetQuizById(id, ct));
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

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateQuiz(int id, [FromBody] UpdateQuizModel model, CancellationToken ct)
        {
            if (id != model.Id)
            {
                return FailResultFromErrors("ID không khớp", new[] { "Id trên URL và Body phải giống nhau" });
            }

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                return FailResultFromErrors("Dữ liệu không hợp lệ", errors);
            }

            return FromApiResult(await _quizService.UpdateQuiz(model, ct));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteQuiz(int quizId, CancellationToken ct)
        {
            return FromApiResult(await _quizService.DeleteQuiz(quizId, ct));
        }

        [HttpDelete("room/{roomId}")]
        public async Task<IActionResult> DeleteAllInRoom(int roomId, CancellationToken ct)
        {
            return FromApiResult(await _quizService.DeleteAllQuizzesInRoom(roomId, ct));
        }

        [HttpPut("update-status")]
        public async Task<IActionResult> UpdateStatus([FromBody] UpdateQuizStatusModel model, CancellationToken ct)
        {
            return FromApiResult(await _quizService.UpdateStatus(model, ct));
        }

        [HttpPut("update-status-bulk")]
        public async Task<IActionResult> UpdateBulkStatus([FromBody] UpdateBulkStatusModel model, CancellationToken ct)
        {
            return FromApiResult(await _quizService.UpdateBulkStatus(model, ct));
        }

    }
}
