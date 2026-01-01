namespace server.Service.Models
{
    /// <summary>
    /// Chuẩn response dùng chung cho toàn bộ API.
    /// 
    /// ApiResult giúp:
    /// - Chuẩn hóa format response giữa các endpoint
    /// - Frontend chỉ cần kiểm tra IsSuccess để xử lý
    /// - Hỗ trợ trả dữ liệu, message, mã lỗi và danh sách lỗi chi tiết
    /// 
    /// Quy ước sử dụng:
    /// - IsSuccess = true  → request xử lý thành công
    /// - IsSuccess = false → request thất bại (validation, business, system error...)
    /// 
    /// NÊN dùng static method:
    /// - ApiResult.Success(...) cho response thành công
    /// - ApiResult.Fail(...) cho response thất bại
    /// 
    /// KHÔNG nên khởi tạo trực tiếp bằng new ApiResult(...) ở Controller.
    /// </summary>
    public class ApiResult
    {
        /// <summary>
        /// Trạng thái xử lý của request
        /// true  = thành công
        /// false = thất bại
        /// </summary>
        public bool IsSuccess { get; set; }

        /// <summary>
        /// Dữ liệu trả về cho client (object, list, DTO...)
        /// Chỉ có giá trị khi IsSuccess = true
        /// </summary>
        public object? Data { get; set; }

        /// <summary>
        /// Message hiển thị cho client
        /// - Thành công: thông báo ngắn gọn
        /// - Thất bại: mô tả lỗi chính
        /// </summary>
        public string? Message { get; set; }

        /// <summary>
        /// Mã lỗi nội bộ (dùng cho FE hoặc logging)
        /// Ví dụ: VALIDATION_ERROR, USER_NOT_FOUND, PERMISSION_DENIED
        /// </summary>
        public string? ErrorCode { get; set; }

        /// <summary>
        /// Danh sách lỗi chi tiết (thường dùng cho validation)
        /// Ví dụ: ["Email không hợp lệ", "Password tối thiểu 8 ký tự"]
        /// </summary>
        public IEnumerable<string>? Errors { get; set; }

        public ApiResult() { }

        public ApiResult(bool isSuccess, object? data = null, string? message = null)
        {
            IsSuccess = isSuccess;
            Data = data;
            Message = message;
        }

        /// <summary>
        /// Tạo response thành công
        /// </summary>
        /// <param name="data">Dữ liệu trả về cho client</param>
        /// <param name="message">Thông báo thành công</param>
        /// <returns>ApiResult với IsSuccess = true</returns>
        public static ApiResult Success(object? data = null, string? message = null)
            => new ApiResult(true, data, message);

        /// <summary>
        /// Tạo response thất bại
        /// </summary>
        /// <param name="message">Thông báo lỗi chính</param>
        /// <param name="errorCode">Mã lỗi nội bộ</param>
        /// <param name="errors">Danh sách lỗi chi tiết</param>
        /// <returns>ApiResult với IsSuccess = false</returns>
        public static ApiResult Fail(
            string message,
            string? errorCode = null,
            IEnumerable<string>? errors = null)
            => new ApiResult(false, null, message)
            {
                ErrorCode = errorCode,
                Errors = errors
            };
    }
}
