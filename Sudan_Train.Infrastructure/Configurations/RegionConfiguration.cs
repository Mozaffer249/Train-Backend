using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sudan_Train.Data.Entity;

namespace Sudan_Train.Infrastructure.Configurations
{
    public class RegionConfiguration : IEntityTypeConfiguration<Region>
    {
        public void Configure(EntityTypeBuilder<Region> builder)
        {
            builder.HasKey(r => r.Id);

            builder.Property(r => r.NameEn)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(r => r.NameAr)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(r => r.Code)
                .IsRequired()
                .HasMaxLength(20);

            builder.HasIndex(r => r.Code)
                .IsUnique();

            builder.HasMany(r => r.States)
                .WithOne(s => s.Region)
                .HasForeignKey(s => s.RegionId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
