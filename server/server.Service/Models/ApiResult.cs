namespace server.Service.Models
{
    public class ApiResult
    {
        public bool IsSuccess { get; set; }
        public object? Data { get; set; }
        public string? Message { get; set; }

        public ApiResult() { }

        public ApiResult(bool isSuccess, object? data = null, string? message = null)
        {
            IsSuccess = isSuccess;
            Data = data;
            Message = message;
        }

        public static ApiResult Success(object? data = null)
            => new ApiResult(true, data);

        public static ApiResult Fail(string message)
            => new ApiResult(false, null, message);
    }
}
