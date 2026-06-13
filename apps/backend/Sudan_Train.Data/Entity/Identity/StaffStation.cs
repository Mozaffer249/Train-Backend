using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sudan_Train.Data.Entity.Identity
{
    // Many-to-many link between Staff users (StaffCounter / StaffBoarding) and
    // the Stations they're authorised to operate at. Looked up by
    // IStaffAuthorizationService to gate boarding + counter actions.
    public class StaffStation
    {
        [Key]
        public int Id { get; set; }

        public int UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public User User { get; set; } = default!;

        public int StationId { get; set; }

        [ForeignKey(nameof(StationId))]
        public Station Station { get; set; } = default!;

        public DateTime AssignedAt { get; set; } = DateTime.UtcNow;

        // User who created this assignment (for audit).
        public int? AssignedBy { get; set; }
    }
}
