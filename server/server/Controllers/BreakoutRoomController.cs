using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using server.Service.Interfaces;
using server.Service.Models.BreakoutRooms;

namespace server.Controllers
{
    /// <summary>
    /// BreakoutRoomController: API endpoints cho BreakoutRoom.
    ///
    /// Cách dùng:
    /// - Controller chỉ nhận request và gọi IBreakoutRoomService.
    /// - Không xử lý business logic trong controller.
    /// - Response chuẩn hóa qua BaseController.FromApiResult / FailResultFromErrors.
    /// </summary>
    public class BreakoutRoomController : BaseController
    {
        private readonly IBreakoutRoomService _breakoutRoomService;

        public BreakoutRoomController(IBreakoutRoomService breakoutRoomService)
        {
            _breakoutRoomService = breakoutRoomService;
        }

        [HttpGet("get-all")]
        public async Task<IActionResult> GetAll()
            => FromApiResult(await _breakoutRoomService.GetAllBreakoutRooms());

        [Authorize]
        [HttpGet("get-by-id")]
        public async Task<IActionResult> GetById(int id)
            => FromApiResult(await _breakoutRoomService.GetBreakoutRoomById(id));

        [Authorize]
        [HttpGet("get-by-parentRoomId")]
        public async Task<IActionResult> GetByParentRoomId(int parentRoomId)
            => FromApiResult(await _breakoutRoomService.GetBreakoutRoomsByParentRoomId(parentRoomId));

        [Authorize]
        [HttpPost("add")]
        public async Task<IActionResult> Add([FromBody] AddBreakoutRoomModel model)
        {
            if (!ModelState.IsValid)
                return FailResultFromErrors(
                    "Dữ liệu không hợp lệ",
                    ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)
                );

            var result = await _breakoutRoomService.AddBreakoutRoom(model);
            return FromApiResult(result, StatusCodes.Status201Created);
        }

        [Authorize]
        [HttpPut("update")]
        public async Task<IActionResult> Update([FromBody] UpdateBreakoutRoomModel model)
        {
            if (!ModelState.IsValid)
                return FailResultFromErrors(
                    "Dữ liệu không hợp lệ",
                    ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)
                );

            return FromApiResult(await _breakoutRoomService.UpdateBreakoutRoom(model));
        }

        [Authorize]
        [HttpDelete("delete")]
        public async Task<IActionResult> Delete(int breakoutRoomId)
            => FromApiResult(await _breakoutRoomService.DeleteBreakoutRoom(breakoutRoomId));
    }
}
