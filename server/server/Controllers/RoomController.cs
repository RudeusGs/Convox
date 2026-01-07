using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using server.Service.Interfaces;
using server.Service.Models.Rooms;

namespace server.Controllers
{
    public class RoomController : BaseController
    {
        private readonly IRoomService _roomService;

        public RoomController(IRoomService roomService)
        {
            _roomService = roomService;
        }

        [HttpGet("get-all")]
        public async Task<IActionResult> GetAll()
            => FromApiResult(await _roomService.GetAll());

        [Authorize]
        [HttpGet("get-by-id")]
        public async Task<IActionResult> GetById(int id)
            => FromApiResult(await _roomService.GetById(id));

        [Authorize]
        [HttpGet("get-by-userId")]
        public async Task<IActionResult> GetByUserId(int userId)
            => FromApiResult(await _roomService.GetByUserId(userId));

        [Authorize]
        [HttpPost("add")]
        public async Task<IActionResult> AddRoom([FromBody] AddRoomModel model)
        {
            if (!ModelState.IsValid)
                return FailResultFromErrors(
                    "Dữ liệu không hợp lệ",
                    ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)
                );

            var result = await _roomService.Add(model);
            return FromApiResult(result, StatusCodes.Status201Created);
        }

        [Authorize]
        [HttpPut("update")]
        public async Task<IActionResult> Update([FromBody] UpdateRoomModel model)
        {
            if (!ModelState.IsValid)
                return FailResultFromErrors(
                    "Dữ liệu không hợp lệ",
                    ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)
                );

            return FromApiResult(await _roomService.Update(model));
        }

        [Authorize]
        [HttpDelete("delete")]
        public async Task<IActionResult> Delete(int id)
            => FromApiResult(await _roomService.Delete(id));

        [Authorize]
        [HttpPost("upload-avatar")]
        public async Task<IActionResult> UploadAvatar(int roomId, IFormFile file, CancellationToken cancellationToken)
        {
            if (file == null || file.Length == 0)
                return BadRequest(new { message = "File ảnh không được để trống" });

            var result = await _roomService.UploadAvatarAsync(roomId, file, cancellationToken);
            return FromApiResult(result);
        }
    }
}