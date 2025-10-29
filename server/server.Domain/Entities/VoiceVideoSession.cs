using server.Domain.Base;

namespace server.Domain.Entities
{
    // Voice / Video session
    public class VoiceVideoSession : EntityBase
    {
        public int RoomId { get; set; } // Id phòng
        public string SessionType { get; set; } // voice hoặc video
        public bool IsActive { get; set; } // Phiên đang chạy hay không
    }
}
