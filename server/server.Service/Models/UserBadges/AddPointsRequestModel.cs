using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace server.Service.Models.UserBadge
{
    public class AddPointsRequestModel
    {
        /// <summary>
        /// ID user cần cộng điểm (bắt buộc).
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// Số điểm kinh nghiệm cần cộng (bắt buộc, >= 0).
        /// </summary>
        public int ExperiencePoints { get; set; }
    }
}
