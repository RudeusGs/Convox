using server.Infrastructure.Persistence;
using server.Service.Common.IServices;

namespace server.Service.Services
{
    /// <summary>
    /// BaseService: class nền cho toàn bộ Service trong dự án.
    ///
    /// Mục đích:
    /// - Gom các dependency dùng chung (DataContext, IUserService) để service con không phải khai báo lặp.
    /// - Cung cấp tiện ích dùng chung:
    ///   + Now: thời gian hiện tại theo UTC+7 (VN)
    ///   + UserName: user hiện tại (phục vụ audit/log)
    ///   + SaveChangesAsync: helper save DB gọn
    ///
    /// Quy ước sử dụng:
    /// - Mọi service nên kế thừa BaseService.
    /// - Service con bắt buộc gọi base(dataContext, userService) trong constructor.
    /// - Controller gọi service, service chịu trách nhiệm làm việc với DbContext và trả ApiResult.
    ///
    /// Lưu ý:
    /// - Now đang cố định UTC+7. Nếu hệ thống cần multi-timezone, nên thay bằng TimeProvider/IClock.
    /// - BaseService không tự handle transaction; nếu cần transaction, implement riêng (UnitOfWork/TransactionScope).
    /// </summary>
    public abstract class BaseService
    {
        /// <summary>
        /// EF Core DbContext chính để thao tác database.
        /// Dùng cho: query / add / update / remove / transaction...
        /// </summary>
        protected readonly DataContext _dataContext;

        /// <summary>
        /// Service cung cấp thông tin user hiện tại (ví dụ lấy từ JWT claims).
        /// Dùng cho: audit fields (CreatedBy/UpdatedBy), phân quyền, logging...
        /// </summary>
        protected readonly IUserService _userService;

        /// <summary>
        /// Khởi tạo BaseService với dependency bắt buộc.
        /// Throw ArgumentNullException nếu truyền null để fail-fast (tránh lỗi runtime khó debug).
        /// </summary>
        protected BaseService(DataContext dataContext, IUserService userService)
        {
            _dataContext = dataContext ?? throw new ArgumentNullException(nameof(dataContext));
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        }

        /// <summary>
        /// Thời gian hiện tại theo UTC+7 (giờ Việt Nam).
        /// 
        /// Dùng khi set audit fields:
        /// - CreatedAt, UpdatedAt, DeletedAt...
        ///
        /// Ví dụ:
        /// entity.CreatedAt = Now;
        /// entity.UpdatedAt = Now;
        /// </summary>
        protected DateTime Now => DateTime.Now;

        /// <summary>
        /// Tên user hiện tại do IUserService cung cấp.
        ///
        /// Dùng khi set audit fields:
        /// - CreatedBy, UpdatedBy, DeletedBy...
        ///
        /// Fallback:
        /// - Trả "Anonymous" nếu không có user context (background job, request không auth, v.v.)
        /// </summary>
        protected string UserName => _userService.UserName ?? "Anonymous";

        /// <summary>
        /// Helper: SaveChangesAsync nhanh gọn cho DbContext.
        ///
        /// Khuyến nghị:
        /// - Gọi SaveChangesAsync trong Service, KHÔNG gọi ở Controller (để đúng separation of concerns).
        /// - Truyền CancellationToken từ Controller xuống để request có thể cancel đúng chuẩn.
        ///
        /// Trả về:
        /// - Số record bị ảnh hưởng theo EF Core.
        ///
        /// Ví dụ:
        /// _dataContext.Entities.Add(entity);
        /// await SaveChangesAsync(ct);
        /// </summary>
        protected async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _dataContext.SaveChangesAsync(cancellationToken);
        }

        #region Ví dụ sử dụng (tham khảo)

        /*
         * Ví dụ 1: Service kế thừa BaseService
         *
         * public class CategoryService : BaseService
         * {
         *     public CategoryService(DataContext dataContext, IUserService userService)
         *         : base(dataContext, userService) { }
         *
         *     public async Task<ApiResult> CreateAsync(CreateCategoryDto dto, CancellationToken ct)
         *     {
         *         var entity = new Category
         *         {
         *             Name = dto.Name,
         *             CreatedAt = Now,
         *             CreatedBy = UserName
         *         };
         *
         *         _dataContext.Categories.Add(entity);
         *         await SaveChangesAsync(ct);
         *
         *         return ApiResult.Success(entity, "Tạo category thành công");
         *     }
         * }
         *
         * ------------------------------------------------------------
         * Ví dụ 2: Controller gọi Service (chuẩn)
         *
         * [ApiController]
         * [Route("api/categories")]
         * public class CategoriesController : ControllerBase
         * {
         *     private readonly ICategoryService _categoryService;
         *
         *     public CategoriesController(ICategoryService categoryService)
         *     {
         *         _categoryService = categoryService;
         *     }
         *
         *     [HttpPost]
         *     public async Task<IActionResult> Create([FromBody] CreateCategoryDto dto, CancellationToken ct)
         *     {
         *         var result = await _categoryService.CreateAsync(dto, ct);
         *
         *         // Tuỳ convention dự án:
         *         // - Luôn return 200 và FE check IsSuccess
         *         // - Hoặc map IsSuccess -> status code (400/404/500)
         *         return Ok(result);
         *     }
         * }
         */

        #endregion
    }
}
