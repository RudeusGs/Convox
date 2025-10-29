using server.Domain.Base;

namespace server.Domain.Entities
{
    // Nhóm thảo luận nhỏ
    public class BreakoutRoom : EntityBase
    {
        public string Name { get; set; } // Tên nhóm
        public int ParentRoomId { get; set; } // Id phòng chính
        public List<int> UserIds { get; set; } = new List<int>(); // Thành viên nhóm
    }
}
