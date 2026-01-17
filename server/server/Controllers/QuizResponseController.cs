using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using server.Service.Interfaces;
using server.Service.Models.Quizzes;
using server.Service.Services.Quizzes;

namespace server.Controllers
{
    [Authorize]
    public class QuizResponseController : BaseController
    {
        private readonly IQuizResponseService _responseService;

        public QuizResponseController(IQuizResponseService responseService)
        {
            _responseService = responseService;
        }

        [HttpPost("submit-quiz")]
        public async Task<IActionResult> SubmitQuiz([FromBody] SubmitQuizModel model)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                return FailResultFromErrors("Dữ liệu không hợp lệ", errors);
            }

            return FromApiResult(await _responseService.SubmitQuiz(model));
        }

        [HttpGet("get-stats")]
        public async Task<IActionResult> GetStats(int id, CancellationToken ct)
        {
            return FromApiResult(await _responseService.GetQuizStats(id, ct));
        }

        [HttpGet("get-submissions")]
        public async Task<IActionResult> GetSubmissions(int id, CancellationToken ct)
        {
            return FromApiResult(await _responseService.GetQuizSubmissions(id, ct));
        }


        [HttpGet("get-score-board")]
        public async Task<IActionResult> GetScoreboard(int roomId, CancellationToken ct)
        {
            return FromApiResult(await _responseService.GetRoomScoreboard(roomId, ct));
        }

        [HttpGet("get-my-results")]
        public async Task<IActionResult> GetMyResults(int roomId, CancellationToken ct)
        {
            return FromApiResult(await _responseService.GetMyResults(roomId, ct));
        }
    }
}