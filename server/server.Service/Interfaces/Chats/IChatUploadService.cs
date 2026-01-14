using Microsoft.AspNetCore.Http;
using server.Service.Models;

namespace server.Service.Interfaces
{
    public interface IChatUploadService
    {
        Task<ApiResult> UploadChatImages(List<IFormFile> images, CancellationToken cancellationToken = default);
    }
}
