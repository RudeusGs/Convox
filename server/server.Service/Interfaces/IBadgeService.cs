using server.Service.Models;
using server.Service.Models.Badge;

namespace server.Service.Interfaces
{
    public interface IBadgeService
    {
        public Task<ApiResult> GetAll();
        public Task<ApiResult> GetById(int id);
        public Task<ApiResult> Add(AddBadgeModel modelBadge);
        public Task<ApiResult> Update(UpdateBadgeModel modelBadge);
        public Task<ApiResult> Delete(int id);
    }
}
