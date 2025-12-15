using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using server.Service.Models;
using System.Net;

namespace server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public abstract class BaseController : ControllerBase
    {
        protected string TraceId =>
            HttpContext?.TraceIdentifier ?? Guid.NewGuid().ToString("N");

        /// <summary>
        /// Trả về chuẩn ApiResult (Success) với status code tùy chọn.
        /// </summary>
        protected IActionResult OkResult(object? data = null, string? message = null, int statusCode = StatusCodes.Status200OK)
        {
            var result = ApiResult.Success(data, message);
            return StatusCode(statusCode, result);
        }

        /// <summary>
        /// Trả về chuẩn ApiResult (Fail) với status code + errorCode + errors để debug.
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
        /// Mapping IdentityResult / validation sang ApiResult.Fail một cách nhất quán.
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
        /// Chuẩn ProblemDetails cho lỗi hệ thống (500) hoặc lỗi nghiệp vụ nghiêm trọng.
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

            problem.Extensions["traceId"] = TraceId;

            return StatusCode(statusCode, problem);
        }

        /// <summary>
        /// Helper: nếu service đã trả ApiResult thì controller chỉ việc trả về.
        /// </summary>
        protected IActionResult FromApiResult(ApiResult apiResult, int successStatusCode = StatusCodes.Status200OK)
        {
            if (apiResult == null)
                return ProblemResult("Null result", "Service returned null.", StatusCodes.Status500InternalServerError);

            return apiResult.IsSuccess
                ? StatusCode(successStatusCode, apiResult)
                : StatusCode(StatusCodes.Status400BadRequest, apiResult);
        }
    }
}
