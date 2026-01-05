using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using server.Service.Interfaces;
using server.Service.Models.Badge;
using System.Linq;

namespace server.Controllers
{
    public class BadgeController : BaseController
    {
        private readonly IBadgeService _badgeService;

        public BadgeController(IBadgeService badgeService)
        {
            _badgeService = badgeService;
        }

        [HttpGet("get-all")]
        public async Task<IActionResult> GetAll()
            => FromApiResult(await _badgeService.GetAll());

        [Authorize]
        [HttpGet("get-by-id")]
        public async Task<IActionResult> GetById(int id)
            => FromApiResult(await _badgeService.GetById(id));

        [Authorize]
        [HttpPost("add")]
        public async Task<IActionResult> Add([FromBody] AddBadgeModel model)
        {
            if (!ModelState.IsValid)
                return FailResultFromErrors(
                    "Dữ liệu không hợp lệ",
                    ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)
                );

            var result = await _badgeService.Add(model);
            return FromApiResult(result, StatusCodes.Status201Created);
        }

        [Authorize]
        [HttpPut("update")]
        public async Task<IActionResult> Update([FromBody] UpdateBadgeModel model)
        {
            if (!ModelState.IsValid)
                return FailResultFromErrors(
                    "Dữ liệu không hợp lệ",
                    ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)
                );

            return FromApiResult(await _badgeService.Update(model));
        }
        [Authorize]
        [HttpDelete("delete")]
        public async Task<IActionResult> Delete(int id)
            => FromApiResult(await _badgeService.Delete(id));
    }
}
