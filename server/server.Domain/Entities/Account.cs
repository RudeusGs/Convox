using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
namespace server.Domain.Entities
{
    public class Account : IdentityUser<int>
    {
        [StringLength(32, ErrorMessage = "Name cannot exceed 32 characters.")]
        public string Name { get; set; }
        public string Email { get; set; }
        public decimal Coin { get; set; } = 0;
        public string? Avatar { get; set; }
        public bool IsAuthen { get; set; } = false;
        public virtual DateTime? CreatedDate { get; set; }
        public virtual DateTime? UpdatedDate { get; set; }
    }
}
