using server.Domain.Base;

namespace server.Domain.Entities
{
    public class UserBadge : EntityBase
    {
        public int UserId { get; set; } // Ai có badge
        public int BadgeId { get; set; } // Id badge
        public int Points { get; set; } // Điểm tích lũy 
        public int CurrentTier { get; set; } = 1; // Cấp bậc hiện tại
    }
}
