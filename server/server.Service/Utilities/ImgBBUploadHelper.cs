using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace server.Service.Utilities
{
    /// <summary>
    /// ImgBBUploadHelper: Utility class static dùng chung để upload ảnh lên ImgBB.
    /// 
    /// Mục đích:
    /// - Cung cấp hàm static để upload ảnh từ bất kỳ service nào
    /// - Tái sử dụng logic upload, không lặp code
    /// - Các bảng lưu URL ảnh thông qua thuộc tính riêng
    /// - OPTIMIZE: Sử dụng HttpClient singleton và upload song song
    /// 
    /// Cách sử dụng:
    /// var imageUrl = await ImgBBUploadHelper.UploadImageAsync(file, configuration);
    /// entity.AvatarUrl = imageUrl;
    /// 
    /// Lưu ý:
    /// - Cần inject IConfiguration để lấy ApiKey và ApiUrl từ appsettings.json
    /// - File phải validate size trước khi gọi hàm (sẽ check lại ở đây)
    /// - Throw exception nếu upload thất bại
    /// </summary>
    public static class ImgBBUploadHelper
    {
        /// <summary>
        /// Singleton HttpClient cho performance tốt hơn (connection pooling)
        /// </summary>
        private static readonly HttpClient _sharedHttpClient = new HttpClient
        {
            Timeout = TimeSpan.FromSeconds(60) // Default timeout, sẽ override khi cần
        };

        /// <summary>
        /// JsonSerializerOptions cho deserialize ImgBB response.
        /// </summary>
        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        /// <summary>
        /// Upload nhiều ảnh song song để tăng tốc độ.
        /// </summary>
        /// <param name="files">Danh sách file ảnh cần upload</param>
        /// <param name="configuration">IConfiguration để lấy settings</param>
        /// <param name="maxConcurrency">Số lượng upload đồng thời tối đa (mặc định 4)</param>
        /// <param name="cancellationToken">Token để cancel request</param>
        /// <returns>Tuple chứa danh sách URL thành công và danh sách lỗi</returns>
        public static async Task<(List<string> SuccessUrls, List<string> Errors)> UploadImagesParallelAsync(
            List<IFormFile> files,
            IConfiguration configuration,
            int maxConcurrency = 4,
            CancellationToken cancellationToken = default)
        {
            if (files == null || files.Count == 0)
                return (new List<string>(), new List<string> { "Không có file nào được upload" });

            var successUrls = new List<string>();
            var errors = new List<string>();
            var semaphore = new SemaphoreSlim(maxConcurrency, maxConcurrency);

            var uploadTasks = files.Select(async file =>
            {
                await semaphore.WaitAsync(cancellationToken);
                try
                {
                    var url = await UploadImageAsync(file, configuration, cancellationToken);
                    lock (successUrls)
                    {
                        successUrls.Add(url);
                    }
                }
                catch (Exception ex)
                {
                    lock (errors)
                    {
                        errors.Add($"Lỗi upload file {file.FileName}: {ex.Message}");
                    }
                }
                finally
                {
                    semaphore.Release();
                }
            });

            await Task.WhenAll(uploadTasks);

            return (successUrls, errors);
        }

        /// <summary>
        /// Upload ảnh lên ImgBB từ IFormFile.
        /// </summary>
        /// <param name="file">File ảnh cần upload (từ HTTP request)</param>
        /// <param name="configuration">IConfiguration để lấy settings</param>
        /// <param name="cancellationToken">Token để cancel request</param>
        /// <returns>URL của ảnh sau upload</returns>
        /// <exception cref="ArgumentNullException">File hoặc configuration null</exception>
        /// <exception cref="InvalidOperationException">File size vượt quá giới hạn hoặc upload thất bại</exception>
        public static async Task<string> UploadImageAsync(
            IFormFile file,
            IConfiguration configuration,
            CancellationToken cancellationToken = default)
        {
            if (file == null)
                throw new ArgumentNullException(nameof(file), "File không được null");

            if (configuration == null)
                throw new ArgumentNullException(nameof(configuration), "Configuration không được null");

            var apiKey = configuration["ImgBB:ApiKey"];
            var apiUrl = configuration["ImgBB:ApiUrl"];
            var maxFileSizeMB = int.TryParse(configuration["ImgBB:MaxFileSizeInMB"], out var size) ? size : 32;
            var timeoutSeconds = int.TryParse(configuration["ImgBB:TimeoutInSeconds"], out var timeout) ? timeout : 30;

            if (string.IsNullOrWhiteSpace(apiKey) || string.IsNullOrWhiteSpace(apiUrl))
                throw new InvalidOperationException("ImgBB settings không được cấu hình đúng");

            var maxFileSizeBytes = maxFileSizeMB * 1024 * 1024;
            if (file.Length > maxFileSizeBytes)
                throw new InvalidOperationException(
                    $"Kích thước file vượt quá {maxFileSizeMB}MB. Kích thước hiện tại: {file.Length / (1024 * 1024)}MB");

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp", ".bmp" };
            var fileExtension = Path.GetExtension(file.FileName).ToLower();
            if (!allowedExtensions.Contains(fileExtension))
                throw new InvalidOperationException(
                    $"Định dạng file không hỗ trợ. Chấp nhận: {string.Join(", ", allowedExtensions)}");

            try
            {
                using (var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken))
                {
                    cts.CancelAfter(TimeSpan.FromSeconds(timeoutSeconds));

                    using (var content = new MultipartFormDataContent())
                    {
                        // Đọc file vào memory để tránh giữ stream mở lâu
                        byte[] fileBytes;
                        using (var memoryStream = new MemoryStream())
                        {
                            await file.CopyToAsync(memoryStream, cts.Token);
                            fileBytes = memoryStream.ToArray();
                        }

                        var fileContent = new ByteArrayContent(fileBytes);
                        fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(
                            file.ContentType ?? "application/octet-stream");

                        content.Add(fileContent, "image", file.FileName);
                        content.Add(new StringContent(apiKey), "key");

                        var response = await _sharedHttpClient.PostAsync(apiUrl, content, cts.Token);

                        if (!response.IsSuccessStatusCode)
                            throw new InvalidOperationException(
                                $"ImgBB API trả về error: {response.StatusCode}");

                        var jsonString = await response.Content.ReadAsStringAsync(cts.Token);
                        var responseContent = JsonSerializer.Deserialize<ImgBBApiResponse>(jsonString, JsonOptions);

                        if (responseContent?.Data?.Url == null)
                            throw new InvalidOperationException("Không thể lấy URL ảnh từ ImgBB");

                        return responseContent.Data.Url;
                    }
                }
            }
            catch (OperationCanceledException)
            {
                throw new InvalidOperationException(
                    $"Upload timeout sau {timeoutSeconds} giây");
            }
            catch (HttpRequestException ex)
            {
                throw new InvalidOperationException(
                    "Lỗi kết nối tới ImgBB API",
                    ex);
            }
        }

        /// <summary>
        /// Upload ảnh lên ImgBB từ byte array.
        /// </summary>
        /// <param name="imageBytes">Byte array của ảnh</param>
        /// <param name="fileName">Tên file (phải chứa extension)</param>
        /// <param name="configuration">IConfiguration để lấy settings</param>
        /// <param name="cancellationToken">Token để cancel request</param>
        /// <returns>URL của ảnh sau upload</returns>
        public static async Task<string> UploadImageBytesAsync(
            byte[] imageBytes,
            string fileName,
            IConfiguration configuration,
            CancellationToken cancellationToken = default)
        {
            if (imageBytes == null || imageBytes.Length == 0)
                throw new ArgumentNullException(nameof(imageBytes), "Byte array không được null hoặc rỗng");

            if (string.IsNullOrWhiteSpace(fileName))
                throw new ArgumentNullException(nameof(fileName), "Tên file không được null");

            var fileName_safe = Path.GetFileName(fileName);
            var apiKey = configuration["ImgBB:ApiKey"];
            var apiUrl = configuration["ImgBB:ApiUrl"];
            var maxFileSizeMB = int.TryParse(configuration["ImgBB:MaxFileSizeInMB"], out var size) ? size : 32;
            var timeoutSeconds = int.TryParse(configuration["ImgBB:TimeoutInSeconds"], out var timeout) ? timeout : 30;

            if (string.IsNullOrWhiteSpace(apiKey) || string.IsNullOrWhiteSpace(apiUrl))
                throw new InvalidOperationException("ImgBB settings không được cấu hình đúng");
            var maxFileSizeBytes = maxFileSizeMB * 1024 * 1024;
            if (imageBytes.Length > maxFileSizeBytes)
                throw new InvalidOperationException(
                    $"Kích thước file vượt quá {maxFileSizeMB}MB. Kích thước hiện tại: {imageBytes.Length / (1024 * 1024)}MB");

            try
            {
                using (var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken))
                {
                    cts.CancelAfter(TimeSpan.FromSeconds(timeoutSeconds));

                    using (var content = new MultipartFormDataContent())
                    {
                        var fileContent = new ByteArrayContent(imageBytes);
                        var extension = Path.GetExtension(fileName_safe).ToLower();
                        var contentType = GetMimeType(extension);
                        fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(contentType);

                        content.Add(fileContent, "image", fileName_safe);
                        content.Add(new StringContent(apiKey), "key");

                        var response = await _sharedHttpClient.PostAsync(apiUrl, content, cts.Token);

                        if (!response.IsSuccessStatusCode)
                            throw new InvalidOperationException(
                                $"ImgBB API trả về error: {response.StatusCode}");

                        var jsonString = await response.Content.ReadAsStringAsync(cts.Token);
                        var responseContent = JsonSerializer.Deserialize<ImgBBApiResponse>(jsonString, JsonOptions);

                        if (responseContent?.Data?.Url == null)
                            throw new InvalidOperationException("Không thể lấy URL ảnh từ ImgBB");

                        return responseContent.Data.Url;
                    }
                }
            }
            catch (OperationCanceledException)
            {
                throw new InvalidOperationException(
                    $"Upload timeout sau {timeoutSeconds} giây");
            }
            catch (HttpRequestException ex)
            {
                throw new InvalidOperationException(
                    "Lỗi kết nối tới ImgBB API",
                    ex);
            }
        }

        /// <summary>
        /// Helper: Lấy MIME type từ file extension.
        /// </summary>
        private static string GetMimeType(string extension)
        {
            return extension.ToLower() switch
            {
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".gif" => "image/gif",
                ".webp" => "image/webp",
                ".bmp" => "image/bmp",
                _ => "application/octet-stream"
            };
        }

        /// <summary>
        /// Model để deserialize ImgBB API response.
        /// </summary>
        private class ImgBBApiResponse
        {
            [JsonPropertyName("data")]
            public ImgBBDataResponse Data { get; set; }

            [JsonPropertyName("success")]
            public bool Success { get; set; }
        }

        private class ImgBBDataResponse
        {
            [JsonPropertyName("url")]
            public string Url { get; set; }

            [JsonPropertyName("display_url")]
            public string Display_Url { get; set; }

            [JsonPropertyName("id")]
            public string Id { get; set; }

            [JsonPropertyName("expiration")]
            public long Expiration { get; set; }
        }
    }
}