using server.Domain.Base;

namespace server.Domain.Entities
{
    public class UserRoomControl : EntityBase
    {
        public int RoomId { get; set; }
        public int UserId { get; set; }
        public bool ForceMuteMic { get; set; } = false;          // ép mute mic
        public bool ForceDisableCam { get; set; } = false;       // ép tắt cam
        public bool DisableChat { get; set; } = false;           // cấm chat
        public bool DisableShare { get; set; } = false;          // cấm share
    }

}
