using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace M_RENT_Aplikacija.Models
{
    public class Recenzija
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Ocena je obavezna.")]
        [Range(1, 5, ErrorMessage = "Ocena mora biti između 1 i 5.")]
        public int Rating { get; set; }

        [Required(ErrorMessage = "Komentar je obavezan.")]
        public string Comment { get; set; }

        [Required(ErrorMessage = "UserID je obavezan.")]
        public string UserId { get; set; }
        public virtual IdentityUser User { get; set; }
    }
}
