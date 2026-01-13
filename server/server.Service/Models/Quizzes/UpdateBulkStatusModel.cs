using server.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace server.Service.Models.Quizzes
{
    // update trạng thái nhiều quiz cùng lúc
    public class UpdateBulkStatusModel
    {
        public int RoomId { get; set; }
        public List<int> QuizIds { get; set; } // Danh sách ID các câu muốn mở
        public QuizStatus NewStatus { get; set; }
    }
}
