using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Sudan_Train.Data.Commons;

namespace Sudan_Train.Data.Entity
{
    public class Refund : AuditableEntity
    {
        [Key]
        public int Id { get; set; }

        public int PaymentId { get; set; }

        [ForeignKey(nameof(PaymentId))]
        public Payment Payment { get; set; } = default!;

        public int BookingId { get; set; }

        [ForeignKey(nameof(BookingId))]
        public Booking Booking { get; set; } = default!;

        [Required, MaxLength(50)]
        public string RefundNumber { get; set; } = default!;

        public decimal Amount { get; set; }

        [MaxLength(10)]
        public string Currency { get; set; } = "SDG";

        public RefundStatus Status { get; set; } = RefundStatus.Pending;
        public RefundMethod Method { get; set; }

        [MaxLength(500)]
        public string? Reason { get; set; }

        public string? ProcessorResponse { get; set; }
        public DateTime? ProcessedAt { get; set; }
    }
}
