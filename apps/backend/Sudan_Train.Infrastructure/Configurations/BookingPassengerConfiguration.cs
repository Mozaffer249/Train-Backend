using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sudan_Train.Data.Entity;

namespace Sudan_Train.Infrastructure.Configurations
{
    public class BookingPassengerConfiguration : IEntityTypeConfiguration<BookingPassenger>
    {
        public void Configure(EntityTypeBuilder<BookingPassenger> builder)
        {
            builder.HasKey(bp => bp.Id);

            // Add composite index for booking-passenger queries
            builder.HasIndex(bp => new { bp.BookingId, bp.PassengerId });

            // Add index on TripId for trip passenger queries
            builder.HasIndex(bp => bp.TripId);

            // Add index on TripSeatId for seat assignment queries
            builder.HasIndex(bp => bp.TripSeatId);

            // Composite index that drives the per-segment seat availability query
            // (trip + seat + boarding/alighting overlap check).
            builder.HasIndex(bp => new { bp.TripId, bp.TripSeatId });

            // Indexes on the segment FKs for join speed.
            builder.HasIndex(bp => bp.BoardingStationId);
            builder.HasIndex(bp => bp.AlightingStationId);

            builder.Property(bp => bp.Price)
                .HasColumnType("decimal(18,2)");

            // Ignore computed property SeatNumber (not mapped to database)
            builder.Ignore(bp => bp.SeatNumber);

            builder.HasOne(bp => bp.Booking)
                .WithMany(b => b.BookingPassengers)
                .HasForeignKey(bp => bp.BookingId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(bp => bp.Passenger)
                .WithMany()
                .HasForeignKey(bp => bp.PassengerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(bp => bp.Trip)
                .WithMany(t => t.BookingPassengers)
                .HasForeignKey(bp => bp.TripId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(bp => bp.TripSeat)
                .WithMany()
                .HasForeignKey(bp => bp.TripSeatId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasOne(bp => bp.Fare)
                .WithMany()
                .HasForeignKey(bp => bp.FareId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasOne(bp => bp.Ticket)
                .WithOne(t => t.BookingPassenger)
                .HasForeignKey<Ticket>(t => t.BookingPassengerId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(bp => bp.BoardingStation)
                .WithMany()
                .HasForeignKey(bp => bp.BoardingStationId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(bp => bp.AlightingStation)
                .WithMany()
                .HasForeignKey(bp => bp.AlightingStationId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}

