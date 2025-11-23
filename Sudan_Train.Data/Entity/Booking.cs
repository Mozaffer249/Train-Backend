using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Sudan_Train.Data.Entity.Identity;

namespace Sudan_Train.Data.Entity
{
    public class Booking
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(20)]
        public string Reference { get; set; } = default!;

        public int? UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public User? User { get; set; }

        public BookingStatus Status { get; set; } = BookingStatus.Pending;

        public decimal TotalAmount { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<Payment> Payments { get; set; } = new List<Payment>();
        public ICollection<BookingPassenger> BookingPassengers { get; set; } = new List<BookingPassenger>();
    }
}