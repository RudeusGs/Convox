using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using server.Service.Interfaces;

namespace server.Controllers.Chats
{
    [Authorize]
    [Route("api/chat")]
    public class ChatUploadController : ChatBaseController
    {
        private readonly IChatUploadService _uploadService;

        public ChatUploadController(IChatUploadService uploadService)
        {
            _uploadService = uploadService;
        }

        [HttpPost("upload-images")]
        public async Task<IActionResult> UploadImages([FromForm] List<IFormFile> images, CancellationToken cancellationToken)
        {
            var result = await _uploadService.UploadChatImages(images, cancellationToken);
            return FromApiResult(result);
        }
    }
}
