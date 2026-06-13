using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Sudan_Train.Data.Entity.Identity;

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

        public TicketStatus Status { get; set; } = TicketStatus.Issued;

        // Audit: who boarded this ticket and when. Both null until Status flips
        // to Boarded via the manifest/scan flow.
        public DateTime? BoardedAt { get; set; }

        public int? BoardedByUserId { get; set; }

        [ForeignKey(nameof(BoardedByUserId))]
        public User? BoardedByUser { get; set; }
    }
}
