using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sudan_Train.Data.Entity;

namespace Sudan_Train.Infrastructure.Configurations
{
    public class SeatHoldConfiguration : IEntityTypeConfiguration<SeatHold>
    {
        public void Configure(EntityTypeBuilder<SeatHold> builder)
        {
            builder.HasKey(h => h.Id);

            builder.HasIndex(h => new { h.TripId, h.ExpiresAt });
            builder.HasIndex(h => new { h.UserId, h.HoldGroupId });
            builder.HasIndex(h => h.TripSeatId);

            builder.HasOne(h => h.TripSeat)
                .WithMany()
                .HasForeignKey(h => h.TripSeatId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
