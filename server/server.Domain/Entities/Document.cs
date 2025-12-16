using server.Domain.Base;

namespace server.Domain.Entities
{
    // Tài liệu
    public class Document : EntityBase
    {
        public int RoomId { get; set; } // Id phòng chứa tài liệu
        public int UploadedById { get; set; } // Ai upload
        public string FileName { get; set; } // Tên file
        public string FilePath { get; set; } // Đường dẫn file
        public bool IsPublic { get; set; } = true; // Quyền truy cập
    }
}
