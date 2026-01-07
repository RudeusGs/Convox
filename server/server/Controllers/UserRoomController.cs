using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using server.Service.Interfaces;
using server.Service.Models.UserRooms;

namespace server.Controllers
{
    [Authorize]
    public class UserRoomController : BaseController
    {
        private readonly IUserRoomService _userRoomService;

        public UserRoomController(IUserRoomService userRoomService)
        {
            _userRoomService = userRoomService;
        }


        [HttpGet("get-rooms-for-user")]
        public async Task<IActionResult> GetRoomsForUser(int userId)
            => FromApiResult(await _userRoomService.GetRoomsForUser(userId));

        [HttpGet("get-users-in-room")]
        public async Task<IActionResult> GetUsersInRoom(int roomId)
            => FromApiResult(await _userRoomService.GetUsersInRoom(roomId));


        [HttpPost("add-user-to-room")]
        public async Task<IActionResult> AddUserToRoom([FromBody] AddUserToRoomModel model)
        {
            if (!ModelState.IsValid)
                return FailResultFromErrors(
                    "Dữ liệu không hợp lệ",
                    ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));

            return FromApiResult(await _userRoomService.AddUserToRoom(model), StatusCodes.Status201Created);
        }

        [HttpPost("join-by-code")]
        public async Task<IActionResult> JoinByCode([FromBody] JoinRoomCodeModel model)
        {
            if (!ModelState.IsValid)
                return FailResultFromErrors(
                    "Dữ liệu không hợp lệ",
                    ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));

            return FromApiResult(await _userRoomService.JoinRoomCode(model), StatusCodes.Status201Created);
        }

        [HttpDelete("remove-user-from-room")]
        public async Task<IActionResult> RemoveUserFromRoom(int userId, int roomId)
            => FromApiResult(await _userRoomService.RemoveUserFromRoom(userId, roomId));


        [HttpPut("change-user-role")]
        public async Task<IActionResult> ChangeUserRole([FromBody] ChangeUserRoleInRoomModel model)
        {
            if (!ModelState.IsValid)
                return FailResultFromErrors(
                    "Dữ liệu không hợp lệ",
                    ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));

            return FromApiResult(await _userRoomService.ChangeUserRoleInRoom(model));
        }

        [HttpPut("ban-user")]
        public async Task<IActionResult> BanUser(int userId, int roomId)
            => FromApiResult(await _userRoomService.BanUserFromRoom(userId, roomId));

        [HttpPut("unban-user")]
        public async Task<IActionResult> UnbanUser(int userId, int roomId)
            => FromApiResult(await _userRoomService.UnbanUserFromRoom(userId, roomId));
    }
}
