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

            builder.Property(bp => bp.Price)
                .HasColumnType("decimal(18,2)");

            builder.Property(bp => bp.SeatNumber)
                .HasMaxLength(10);

            builder.HasOne(bp => bp.Booking)
                .WithMany(b => b.BookingPassengers)
                .HasForeignKey(bp => bp.BookingId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(bp => bp.Passenger)
                .WithMany()
                .HasForeignKey(bp => bp.PassengerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(bp => bp.Trip)
                .WithMany()
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
        }
    }
}

