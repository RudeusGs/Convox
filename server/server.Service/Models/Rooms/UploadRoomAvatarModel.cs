using Microsoft.AspNetCore.Http;

namespace server.Service.Models.Room
{
    public class UploadRoomAvatarModel
    {
        public int RoomId { get; set; }
        public IFormFile AvatarFile { get; set; }
    }
}