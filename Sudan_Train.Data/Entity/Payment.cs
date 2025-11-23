using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sudan_Train.Data.Entity
{
    public class Payment
    {
        [Key]
        public int Id { get; set; }

        public int BookingId { get; set; }

        [ForeignKey(nameof(BookingId))]
        public Booking Booking { get; set; } = default!;

        public PaymentMethod Method { get; set; }

        public decimal Amount { get; set; }
        public string Currency { get; set; } = "SDG";

        public PaymentStatus Status { get; set; }

        public string? ProcessorResponse { get; set; }
        public string? Reference { get; set; }
        public string? CardLast4 { get; set; }
        public string? CardBrand { get; set; }
        public string? CardToken { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}