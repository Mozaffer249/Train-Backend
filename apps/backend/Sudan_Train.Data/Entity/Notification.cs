using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Sudan_Train.Data.Commons;
using Sudan_Train.Data.Entity.Identity;

namespace Sudan_Train.Data.Entity
{
    public class Notification : AuditableEntity
    {
        [Key]
        public int Id { get; set; }

        public int? UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public User? User { get; set; }

        public int? BookingId { get; set; }

        [ForeignKey(nameof(BookingId))]
        public Booking? Booking { get; set; }

        public NotificationType Type { get; set; }

        [Required, MaxLength(200)]
        public string Subject { get; set; } = default!;

        [Required]
        public string Message { get; set; } = default!;

        public bool IsRead { get; set; } = false;
        public DateTime? ReadAt { get; set; }

        public NotificationChannel Channel { get; set; }
        public bool IsSent { get; set; } = false;
        public DateTime? SentAt { get; set; }
    }
}
