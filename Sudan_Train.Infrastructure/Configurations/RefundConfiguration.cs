using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sudan_Train.Data.Entity;

namespace Sudan_Train.Infrastructure.Configurations
{
    public class RefundConfiguration : IEntityTypeConfiguration<Refund>
    {
        public void Configure(EntityTypeBuilder<Refund> builder)
        {
            builder.HasKey(r => r.Id);

            builder.Property(r => r.RefundNumber)
                .IsRequired()
                .HasMaxLength(50);

            builder.HasIndex(r => r.RefundNumber)
                .IsUnique();

            builder.HasIndex(r => r.BookingId);
            builder.HasIndex(r => r.PaymentId);
            builder.HasIndex(r => r.Status);
            builder.HasIndex(r => r.CreatedAt);

            builder.Property(r => r.Amount)
                .HasColumnType("decimal(18,2)");

            builder.Property(r => r.Currency)
                .IsRequired()
                .HasMaxLength(10);

            builder.Property(r => r.Status)
                .HasConversion<int>();

            builder.Property(r => r.Method)
                .HasConversion<int>();

            builder.Property(r => r.Reason)
                .HasMaxLength(500);

            builder.HasOne(r => r.Payment)
                .WithMany(p => p.Refunds)
                .HasForeignKey(r => r.PaymentId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(r => r.Booking)
                .WithMany(b => b.Refunds)
                .HasForeignKey(r => r.BookingId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
