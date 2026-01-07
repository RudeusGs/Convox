using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using server.Service.Interfaces;
using server.Service.Models.Badge;
using System.Linq;

namespace server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BadgeController : BaseController
    {
        private readonly IBadgeService _badgeService;

        public BadgeController(IBadgeService badgeService)
        {
            _badgeService = badgeService ?? throw new ArgumentNullException(nameof(badgeService));
        }

        /// <summary>
        /// Lấy tất cả badge.
        /// </summary>
        [HttpGet("get-all")]
        public async Task<IActionResult> GetAll()
            => FromApiResult(await _badgeService.GetAll());

        /// <summary>
        /// Lấy badge theo ID.
        /// </summary>
        [Authorize]
        [HttpGet("get-by-id")]
        public async Task<IActionResult> GetById(int id)
            => FromApiResult(await _badgeService.GetById(id));

        /// <summary>
        /// Tạo badge mới.
        /// Request: multipart/form-data
        /// - Name: Tên badge (text)
        /// - Description: Mô tả (text, tùy chọn)
        /// - IconFile: File icon (binary)
        /// </summary>
        [Authorize]
        [HttpPost("add")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Add([FromForm] AddBadgeModel model, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
                return FailResultFromErrors(
                    "Dữ liệu không hợp lệ",
                    ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)
                );

            var result = await _badgeService.Add(model, cancellationToken);
            return FromApiResult(result, StatusCodes.Status201Created);
        }

        /// <summary>
        /// Cập nhật badge.
        /// Request: multipart/form-data
        /// - Id: ID badge (text)
        /// - Name: Tên mới (text)
        /// - Description: Mô tả mới (text, tùy chọn)
        /// - IconFile: File icon mới (binary, tùy chọn)
        /// </summary>
        [Authorize]
        [HttpPut("update")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Update([FromForm] UpdateBadgeModel model, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
                return FailResultFromErrors(
                    "Dữ liệu không hợp lệ",
                    ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)
                );

            var result = await _badgeService.Update(model, cancellationToken);
            return FromApiResult(result);
        }

        /// <summary>
        /// Xóa badge theo ID.
        /// </summary>
        [Authorize]
        [HttpDelete("delete")]
        public async Task<IActionResult> Delete(int id)
            => FromApiResult(await _badgeService.Delete(id));
    }
}
    