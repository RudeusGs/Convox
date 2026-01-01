using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using server.Service.Interfaces;
using server.Service.Models.Room;
using System.Linq;

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

            var result = await _roomService.AddRoom(model);
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
    }
}
