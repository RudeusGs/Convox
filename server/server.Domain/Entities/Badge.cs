using server.Domain.Base;

namespace server.Domain.Entities
{
    // Badge / Gamification
    public class Badge : EntityBase
    {
        public string Name { get; set; } // Tên badge
        public string Description { get; set; } // Mô tả
    }
}
