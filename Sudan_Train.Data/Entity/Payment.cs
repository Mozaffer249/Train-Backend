using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using EntityFrameworkCore.EncryptColumn.Attribute;

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

        [MaxLength(10)]
        public string Currency { get; set; } = "SDG";

        public PaymentStatus Status { get; set; }

        [EncryptColumn]
        public string? ProcessorResponse { get; set; }

        [MaxLength(100)]
        public string? Reference { get; set; }

        [MaxLength(4)]
        public string? CardLast4 { get; set; }

        [MaxLength(50)]
        public string? CardBrand { get; set; }

        [EncryptColumn]
        public string? CardToken { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<Refund> Refunds { get; set; } = new List<Refund>();
    }
}