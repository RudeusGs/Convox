using server.Infrastructure.Persistence;
using server.Service.Common.IServices;

namespace server.Service.Services
{
    /// <summary>
    /// Base service class cung cấp các tiện ích chung cho tất cả service
    /// </summary>
    public abstract class BaseService
    {
        protected readonly DataContext _dataContext;
        protected readonly IUserService _userService;

        protected BaseService(DataContext dataContext, IUserService userService)
        {
            _dataContext = dataContext ?? throw new ArgumentNullException(nameof(dataContext));
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        }

        /// <summary>
        /// Lấy thời gian hiện tại theo UTC +7
        /// </summary>
        protected DateTime Now => DateTime.UtcNow.AddHours(7);

        /// <summary>
        /// Lấy tên user hiện tại
        /// </summary>
        protected string UserName => _userService.UserName ?? "Anonymous";

        /// <summary>
        /// Async helper: SaveChanges nhanh gọn
        /// </summary>
        protected async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _dataContext.SaveChangesAsync(cancellationToken);
        }
    }
}
