using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sudan_Train.Data.Entity;

namespace Sudan_Train.Infrastructure.Configurations
{
    public class TripSeatConfiguration : IEntityTypeConfiguration<TripSeat>
    {
        public void Configure(EntityTypeBuilder<TripSeat> builder)
        {
            builder.HasKey(ts => ts.Id);

            builder.HasIndex(ts => new { ts.TripId, ts.SeatId })
                .IsUnique();

            // Add index on TripId and Status for availability queries
            builder.HasIndex(ts => new { ts.TripId, ts.Status });

            builder.Property(ts => ts.Price)
                .HasColumnType("decimal(18,2)");

            builder.Property(ts => ts.Status)
                .HasConversion<int>();

            // Ignore computed property Coach (not mapped to database)
            builder.Ignore(ts => ts.Coach);

            builder.HasOne(ts => ts.Trip)
                .WithMany(t => t.TripSeats)
                .HasForeignKey(ts => ts.TripId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(ts => ts.Seat)
                .WithMany(s => s.TripSeats)
                .HasForeignKey(ts => ts.SeatId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}

