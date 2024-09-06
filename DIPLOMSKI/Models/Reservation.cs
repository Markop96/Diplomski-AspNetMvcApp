using M_RENT_Aplikacija.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace M_RENT_Aplikacija.Models
{
    public class Reservation
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("User")]
        public string IdUser { get; set; }

        [ForeignKey("Car")]
        public int IdCar { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime DateStart { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime DateEnd { get; set; }

        public bool IsPaid { get; set; }
        public int NumberOfDays { get; set; } 

        [Column(TypeName = "decimal(10, 2)")]
        public decimal FullPrice { get; set; } 

        public virtual IdentityUser User { get; set; }
        public virtual Automobil Car { get; set; }


    }
}
