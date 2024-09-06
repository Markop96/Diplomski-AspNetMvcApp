using Microsoft.AspNetCore.Identity;

namespace M_RENT_Aplikacija.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string PhoneNumber { get; set; }
    }
}
