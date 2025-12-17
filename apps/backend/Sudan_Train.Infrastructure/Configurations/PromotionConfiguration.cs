using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sudan_Train.Data.Entity;

namespace Sudan_Train.Infrastructure.Configurations
{
    public class PromotionConfiguration : IEntityTypeConfiguration<Promotion>
    {
        public void Configure(EntityTypeBuilder<Promotion> builder)
        {
            builder.HasKey(p => p.Id);

            builder.Property(p => p.Code)
                .IsRequired()
                .HasMaxLength(100);

            // Add unique index on promotion code
            builder.HasIndex(p => p.Code)
                .IsUnique();

            // Add index for active promotions queries
            builder.HasIndex(p => new { p.IsActive, p.ValidFrom, p.ValidTo });

            builder.Property(p => p.NameEn)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(p => p.NameAr)
                .HasMaxLength(200);

            builder.Property(p => p.DescriptionEn)
                .HasMaxLength(1000);

            builder.Property(p => p.DescriptionAr)
                .HasMaxLength(1000);

            builder.Property(p => p.Type)
                .HasConversion<int>();

            builder.Property(p => p.DiscountValue)
                .HasColumnType("decimal(18,2)");

            builder.Property(p => p.MaxDiscount)
                .HasColumnType("decimal(18,2)");

            builder.Property(p => p.MinimumPurchase)
                .HasColumnType("decimal(18,2)");

            builder.HasMany(p => p.PromotionUsages)
                .WithOne(pu => pu.Promotion)
                .HasForeignKey(pu => pu.PromotionId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
