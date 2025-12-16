using server.Domain.Base;

namespace server.Domain.Entities
{
    // Nhóm thảo luận nhỏ con của phòng lớn(thảo luận nhóm cho các sinh viên) giống như kênh của Discord
    public class BreakoutRoom : EntityBase
    {
        public string Name { get; set; } // Tên nhóm
        public int ParentRoomId { get; set; } // Id phòng chính
        public DateTime? ExpireAt { get; set; } // Thời hạn của phòng
    }
}
