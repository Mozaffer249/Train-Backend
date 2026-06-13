using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sudan_Train.Data.Entity;

namespace Sudan_Train.Infrastructure.Configurations
{
    public class TripConfiguration : IEntityTypeConfiguration<Trip>
    {
        public void Configure(EntityTypeBuilder<Trip> builder)
        {
            builder.HasKey(t => t.Id);

            // Status enum stored as int. Migration converts the old string column.
            builder.Property(t => t.Status)
                .HasConversion<int>()
                .HasDefaultValue(TripStatus.Scheduled);

            builder.HasOne(t => t.Train)
                .WithMany()
                .HasForeignKey(t => t.TrainId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(t => t.Route)
                .WithMany(r => r.Trips)
                .HasForeignKey(t => t.RouteId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(t => t.TripSeats)
                .WithOne(ts => ts.Trip)
                .HasForeignKey(ts => ts.TripId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(t => t.BookingPassengers)
                .WithOne(bp => bp.Trip)
                .HasForeignKey(bp => bp.TripId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
