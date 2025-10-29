using server.Domain.Base;

namespace server.Domain.Entities
{
    // Attendance
    public class Attendance : EntityBase
    {
        public int RoomId { get; set; } // Id phòng
        public int UserId { get; set; } // Ai tham gia
        public DateTime JoinTime { get; set; } // Vào lúc
        public string ReasonLeave { get; set; } // Lý do rời sớm
        public DateTime? LeaveTime { get; set; } // Rời lúc
    }
}
