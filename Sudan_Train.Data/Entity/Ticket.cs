using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sudan_Train.Data.Entity
{
    public class Ticket
    {
        [Key]
        public int Id { get; set; }

        public int BookingPassengerId { get; set; }

        [ForeignKey(nameof(BookingPassengerId))]
        public BookingPassenger BookingPassenger { get; set; } = default!;

        public string TicketNumber { get; set; } = default!;
        public string? QrCode { get; set; }
        public string? PdfUrl { get; set; }

        public DateTime IssuedAt { get; set; } = DateTime.UtcNow;
        public DateTime? ValidFrom { get; set; }
        public DateTime? ValidUntil { get; set; }

        public string Status { get; set; } = "Issued";
    }
}