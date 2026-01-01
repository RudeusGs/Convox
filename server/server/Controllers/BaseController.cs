using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using server.Service.Models;
using System.Net;

namespace server.Controllers
{
    /// <summary>
    /// BaseController: class nền cho tất cả API Controller trong hệ thống.
    ///
    /// Mục tiêu:
    /// - Chuẩn hóa response API (ApiResult / ProblemDetails).
    /// - Giảm code lặp trong controller (Ok, Fail, Validation, Problem).
    /// - Thống nhất cách trả status code + format JSON.
    ///
    /// Quy ước chung:
    /// - Controller kế thừa BaseController, KHÔNG return Ok(), BadRequest() trực tiếp.
    /// - Thành công → dùng OkResult(...) hoặc FromApiResult(...)
    /// - Lỗi nghiệp vụ / validation → dùng FailResult(...)
    /// - Lỗi hệ thống nghiêm trọng → dùng ProblemResult(...)
    ///
    /// Controller KHÔNG xử lý business logic:
    /// - Chỉ nhận request, gọi service, map response.
    /// - Không thao tác trực tiếp DbContext.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public abstract class BaseController : ControllerBase
    {
        /// <summary>
        /// TraceId của request hiện tại.
        /// Dùng để log, debug, trace lỗi giữa FE ↔ BE.
        /// </summary>
        protected string TraceId =>
            HttpContext?.TraceIdentifier ?? Guid.NewGuid().ToString("N");

        /// <summary>
        /// Trả về response thành công theo chuẩn ApiResult.
        ///
        /// Thường dùng khi:
        /// - Controller tự build response đơn giản
        /// - Hoặc không cần gọi service trả ApiResult
        ///
        /// Ví dụ:
        /// return OkResult(data, "Thành công");
        /// return OkResult(null, "Xóa thành công", StatusCodes.Status204NoContent);
        /// </summary>
        protected IActionResult OkResult(
            object? data = null,
            string? message = null,
            int statusCode = StatusCodes.Status200OK)
        {
            var result = ApiResult.Success(data, message);
            return StatusCode(statusCode, result);
        }

        /// <summary>
        /// Trả về response thất bại theo chuẩn ApiResult.
        ///
        /// Dùng cho:
        /// - Lỗi nghiệp vụ
        /// - Validation thủ công
        /// - Dữ liệu không hợp lệ
        ///
        /// Ví dụ:
        /// return FailResult("Dữ liệu không hợp lệ");
        /// return FailResult("Không tìm thấy user", 404, "USER_NOT_FOUND");
        /// </summary>
        protected IActionResult FailResult(
            string message,
            int statusCode = StatusCodes.Status400BadRequest,
            string? errorCode = null,
            IEnumerable<string>? errors = null)
        {
            var result = ApiResult.Fail(message, errorCode, errors);
            return StatusCode(statusCode, result);
        }

        /// <summary>
        /// Helper mapping lỗi validation / IdentityResult sang ApiResult.Fail.
        ///
        /// Dùng khi:
        /// - Validate model thủ công
        /// - ASP.NET Identity trả về danh sách lỗi
        ///
        /// Ví dụ:
        /// return FailResultFromErrors("Validate thất bại", errors);
        /// </summary>
        protected IActionResult FailResultFromErrors(
            string message,
            IEnumerable<string> errors,
            int statusCode = StatusCodes.Status400BadRequest,
            string? errorCode = "VALIDATION_ERROR")
        {
            return FailResult(message, statusCode, errorCode, errors);
        }

        /// <summary>
        /// Trả về ProblemDetails theo RFC 7807 cho lỗi hệ thống hoặc lỗi nghiêm trọng.
        ///
        /// Dùng cho:
        /// - Exception không mong muốn
        /// - Lỗi hệ thống (DB down, timeout, crash...)
        ///
        /// Không dùng cho lỗi nghiệp vụ thông thường.
        ///
        /// Ví dụ:
        /// return ProblemResult("Internal error", ex.Message);
        /// </summary>
        protected IActionResult ProblemResult(
            string title,
            string detail,
            int statusCode = StatusCodes.Status500InternalServerError,
            string? errorCode = "INTERNAL_ERROR")
        {
            var problem = new ProblemDetails
            {
                Title = title,
                Detail = detail,
                Status = statusCode,
                Instance = HttpContext?.Request?.Path.Value,
                Type = $"urn:errors:{errorCode}"
            };

            // Gắn TraceId để FE/QA/Dev trace log
            problem.Extensions["traceId"] = TraceId;

            return StatusCode(statusCode, problem);
        }

        /// <summary>
        /// Helper: Controller chỉ việc return kết quả từ Service.
        ///
        /// Dùng khi:
        /// - Service đã trả ApiResult
        /// - Controller không cần xử lý thêm logic
        ///
        /// Mapping:
        /// - IsSuccess = true  → statusCode successStatusCode (mặc định 200)
        /// - IsSuccess = false → 400 BadRequest
        ///
        /// Ví dụ:
        /// var result = await _service.CreateAsync(dto, ct);
        /// return FromApiResult(result);
        /// </summary>
        protected IActionResult FromApiResult(
            ApiResult apiResult,
            int successStatusCode = StatusCodes.Status200OK)
        {
            if (apiResult == null)
            {
                return ProblemResult(
                    "Null result",
                    "Service returned null.",
                    StatusCodes.Status500InternalServerError);
            }

            return apiResult.IsSuccess
                ? StatusCode(successStatusCode, apiResult)
                : StatusCode(StatusCodes.Status400BadRequest, apiResult);
        }

        #region Ví dụ sử dụng (tham khảo)

        /*
         * Ví dụ Controller chuẩn:
         *
         * public class CategoriesController : BaseController
         * {
         *     private readonly ICategoryService _categoryService;
         *
         *     public CategoriesController(ICategoryService categoryService)
         *     {
         *         _categoryService = categoryService;
         *     }
         *
         *     [HttpPost]
         *     public async Task<IActionResult> Create(
         *         [FromBody] CreateCategoryDto dto,
         *         CancellationToken ct)
         *     {
         *         var result = await _categoryService.CreateAsync(dto, ct);
         *
         *         // Controller không xử lý business logic
         *         return FromApiResult(result);
         *     }
         *
         *     [HttpGet("{id}")]
         *     public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
         *     {
         *         var result = await _categoryService.GetByIdAsync(id, ct);
         *         return FromApiResult(result);
         *     }
         * }
         */

        #endregion
    }
}
