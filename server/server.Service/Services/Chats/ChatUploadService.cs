using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using server.Infrastructure.Persistence;
using server.Service.Common.IServices;
using server.Service.Common.Services;
using server.Service.Interfaces;
using server.Service.Models;
using server.Service.Utilities;

namespace server.Service.Services.Chats
{
    public class ChatUploadService : BaseService, IChatUploadService
    {
        private readonly IConfiguration _configuration;

        public ChatUploadService(DataContext dataContext, IUserService userService, IConfiguration configuration)
            : base(dataContext, userService)
        {
            _configuration = configuration;
        }

        public async Task<ApiResult> UploadChatImages(List<IFormFile> images, CancellationToken cancellationToken = default)
        {
            if (images == null || images.Count == 0)
                return ApiResult.Fail("Không có file nào được upload");

            var maxConcurrency = int.TryParse(_configuration["ImgBB:MaxConcurrentUploads"], out var concurrent)
                ? concurrent
                : 4;

            var (uploadedUrls, errors) = await ImgBBUploadHelper.UploadImagesParallelAsync(
                images,
                _configuration,
                maxConcurrency,
                cancellationToken);

            if (uploadedUrls.Count == 0)
                return ApiResult.Fail("Không upload được file nào", errors: errors);

            return ApiResult.Success(new
            {
                UploadedUrls = uploadedUrls,
                SuccessCount = uploadedUrls.Count,
                FailedCount = errors.Count,
                Errors = errors
            }, $"Upload thành công {uploadedUrls.Count}/{images.Count} file");
        }
    }
}
