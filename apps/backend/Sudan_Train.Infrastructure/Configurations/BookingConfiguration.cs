using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sudan_Train.Data.Entity;

namespace Sudan_Train.Infrastructure.Configurations
{
    public class BookingConfiguration : IEntityTypeConfiguration<Booking>
    {
        public void Configure(EntityTypeBuilder<Booking> builder)
        {
            builder.HasKey(b => b.Id);

            builder.Property(b => b.Reference)
                .IsRequired()
                .HasMaxLength(20);

            // Add unique index on Reference for fast lookups
            builder.HasIndex(b => b.Reference)
                .IsUnique();

            // Add index on UserId for user's bookings queries
            builder.HasIndex(b => b.UserId);

            // Add index on CreatedAt for date range queries
            builder.HasIndex(b => b.CreatedAt);

            // Add index on Status for filtering by status
            builder.HasIndex(b => b.Status);

            builder.Property(b => b.TotalAmount)
                .HasColumnType("decimal(18,2)");

            builder.Property(b => b.RefundAmount)
                .HasColumnType("decimal(18,2)");

            builder.Property(b => b.Status)
                .HasConversion<int>();

            builder.Property(b => b.CreatedAt)
                .IsRequired();

            builder.Property(b => b.CancellationReason)
                .HasMaxLength(500);

            builder.HasOne(b => b.User)
                .WithMany(u => u.Bookings)
                .HasForeignKey(b => b.UserId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasMany(b => b.Payments)
                .WithOne(p => p.Booking)
                .HasForeignKey(p => p.BookingId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(b => b.BookingPassengers)
                .WithOne(bp => bp.Booking)
                .HasForeignKey(bp => bp.BookingId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(b => b.Refunds)
                .WithOne(r => r.Booking)
                .HasForeignKey(r => r.BookingId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(b => b.Notifications)
                .WithOne(n => n.Booking)
                .HasForeignKey(n => n.BookingId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(b => b.PromotionUsages)
                .WithOne(pu => pu.Booking)
                .HasForeignKey(pu => pu.BookingId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}

