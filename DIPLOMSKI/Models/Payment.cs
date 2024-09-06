using System;
using Microsoft.AspNetCore.Identity;

namespace M_RENT_Aplikacija.Models
{
    public class Payment
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public int ReservationId { get; set; }
        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; }
        public string StripeChargeId { get; set; }

        // Navigacione osobine
        public virtual Reservation Reservation { get; set; }
        public virtual IdentityUser User { get; set; }
    }
}
