using server.Infrastructure.Persistence;
using server.Service.Common.IServices;

namespace server.Service.Services
{
    public class BaseService
    {
        protected readonly DataContext _dataContext;
        protected readonly DateTime _now = DateTime.UtcNow.AddHours(7);
        protected readonly IUserService _userService;
        protected readonly string _userName;
        public BaseService(DataContext dataContext, IUserService userService)
        {
            _dataContext = dataContext;
            _userService = userService;
            _userName = _userService.UserName;
        }
    }
}
