using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sudan_Train.Data.Entity;

namespace Sudan_Train.Infrastructure.Configurations
{
    public class TrainConfiguration : IEntityTypeConfiguration<Train>
    {
        public void Configure(EntityTypeBuilder<Train> builder)
        {
            builder.HasKey(t => t.Id);

            builder.Property(t => t.TrainNumber)
                .IsRequired()
                .HasMaxLength(50);

            builder.HasIndex(t => t.TrainNumber)
                .IsUnique();

            builder.Property(t => t.NameEn)
                .HasMaxLength(200);

            builder.Property(t => t.NameAr)
                .HasMaxLength(200);

            builder.Property(t => t.Type)
                .HasConversion<int>();

            builder.HasMany(t => t.Coaches)
                .WithOne(c => c.Train)
                .HasForeignKey(c => c.TrainId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}

