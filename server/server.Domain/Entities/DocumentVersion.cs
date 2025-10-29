using server.Domain.Base;

namespace server.Domain.Entities
{
    // Phiên bản tài liệu
    public class DocumentVersion : EntityBase
    {
        public int DocumentId { get; set; } // Id tài liệu
        public string FilePath { get; set; } // File version này lưu ở đâu
        public int VersionNumber { get; set; } // Số version
    }
}
