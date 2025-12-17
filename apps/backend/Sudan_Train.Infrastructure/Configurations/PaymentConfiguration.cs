using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sudan_Train.Data.Entity;

namespace Sudan_Train.Infrastructure.Configurations
{
    public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
    {
        public void Configure(EntityTypeBuilder<Payment> builder)
        {
            builder.HasKey(p => p.Id);

            builder.Property(p => p.Amount)
                .HasColumnType("decimal(18,2)");

            builder.Property(p => p.Currency)
                .HasMaxLength(3)
                .HasDefaultValue("SDG");

            builder.Property(p => p.Method)
                .HasConversion<int>();

            builder.Property(p => p.Status)
                .HasConversion<int>();

            builder.Property(p => p.Reference)
                .HasMaxLength(50);

            builder.Property(p => p.CardLast4)
                .HasMaxLength(4);

            builder.Property(p => p.CardBrand)
                .HasMaxLength(50);

            builder.HasOne(p => p.Booking)
                .WithMany(b => b.Payments)
                .HasForeignKey(p => p.BookingId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}

