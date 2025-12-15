using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
namespace server.Domain.Entities
{
    public class User : IdentityUser<int>
    {
        public string? FullName { get; set; }
        public decimal Coin { get; set; } = 0;
        public string? Avatar { get; set; }
        public string? Status { get; set; }
        public bool IsAuthen { get; set; } = false;
        public virtual DateTime? CreatedDate { get; set; }
        public virtual DateTime? UpdatedDate { get; set; }
    }
}
