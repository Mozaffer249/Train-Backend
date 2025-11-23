using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sudan_Train.Data.Entity;

namespace Sudan_Train.Infrastructure.Configurations
{
    public class SeatConfiguration : IEntityTypeConfiguration<Seat>
    {
        public void Configure(EntityTypeBuilder<Seat> builder)
        {
            builder.HasKey(s => s.Id);

            builder.Property(s => s.SeatNumber)
                .IsRequired()
                .HasMaxLength(10);

            builder.HasIndex(s => new { s.CoachId, s.SeatNumber })
                .IsUnique();

            builder.HasOne(s => s.Coach)
                .WithMany(c => c.Seats)
                .HasForeignKey(s => s.CoachId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(s => s.TripSeats)
                .WithOne(ts => ts.Seat)
                .HasForeignKey(ts => ts.SeatId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}

