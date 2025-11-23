using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sudan_Train.Data.Entity;

namespace Sudan_Train.Infrastructure.Configurations
{
    public class CoachConfiguration : IEntityTypeConfiguration<Coach>
    {
        public void Configure(EntityTypeBuilder<Coach> builder)
        {
            builder.HasKey(c => c.Id);

            builder.Property(c => c.CoachNumber)
                .IsRequired()
                .HasMaxLength(20);

            builder.HasIndex(c => new { c.TrainId, c.CoachNumber })
                .IsUnique();

            builder.Property(c => c.Class)
                .HasConversion<int>();

            builder.HasOne(c => c.Train)
                .WithMany(t => t.Coaches)
                .HasForeignKey(c => c.TrainId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(c => c.Seats)
                .WithOne(s => s.Coach)
                .HasForeignKey(s => s.CoachId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}

