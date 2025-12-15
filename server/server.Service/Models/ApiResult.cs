namespace server.Service.Models
{
    public class ApiResult
    {
        public bool IsSuccess { get; set; }
        public object? Data { get; set; }
        public string? Message { get; set; }
        public string? ErrorCode { get; set; }
        public IEnumerable<string>? Errors { get; set; }

        public ApiResult() { }

        public ApiResult(bool isSuccess, object? data = null, string? message = null)
        {
            IsSuccess = isSuccess;
            Data = data;
            Message = message;
        }

        public static ApiResult Success(object? data = null, string? message = null)
            => new ApiResult(true, data, message);

        public static ApiResult Fail(string message, string? errorCode = null, IEnumerable<string>? errors = null)
            => new ApiResult(false, null, message)
            {
                ErrorCode = errorCode,
                Errors = errors
            };
    }
}
