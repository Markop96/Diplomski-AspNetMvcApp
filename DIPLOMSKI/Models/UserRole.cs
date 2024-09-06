using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace M_RENT_Aplikacija.Models
{
    [Table("AspNetUserRoles")]
    public class UserRole
    {
        [Key, Column(Order = 0)]
        public string UserId { get; set; }

        [Key, Column(Order = 1)]
        public string RoleId { get; set; }

        [ForeignKey("UserId")]
        public virtual IdentityUser User { get; set; }

        [ForeignKey("RoleId")]
        public virtual IdentityRole Role { get; set; }
    }
}
