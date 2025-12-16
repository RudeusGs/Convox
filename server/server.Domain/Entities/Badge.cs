using server.Domain.Base;

namespace server.Domain.Entities
{
    // Badge / Gamification
    public class Badge : EntityBase
    {
        public string Name { get; set; } // Tên badge
        public string Icon { get; set; } // Icon của huy hiệu
        public string Description { get; set; } // Mô tả
    }
}
