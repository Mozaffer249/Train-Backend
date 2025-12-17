using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sudan_Train.Data.Entity;

namespace Sudan_Train.Infrastructure.Configurations
{
    public class PromotionUsageConfiguration : IEntityTypeConfiguration<PromotionUsage>
    {
        public void Configure(EntityTypeBuilder<PromotionUsage> builder)
        {
            builder.HasKey(pu => pu.Id);

            // Add indexes for usage tracking
            builder.HasIndex(pu => pu.PromotionId);
            builder.HasIndex(pu => pu.BookingId);
            builder.HasIndex(pu => pu.UserId);
            builder.HasIndex(pu => new { pu.PromotionId, pu.UserId });

            builder.Property(pu => pu.DiscountAmount)
                .HasColumnType("decimal(18,2)");

            builder.HasOne(pu => pu.Promotion)
                .WithMany(p => p.PromotionUsages)
                .HasForeignKey(pu => pu.PromotionId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(pu => pu.Booking)
                .WithMany(b => b.PromotionUsages)
                .HasForeignKey(pu => pu.BookingId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(pu => pu.User)
                .WithMany(u => u.PromotionUsages)
                .HasForeignKey(pu => pu.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
