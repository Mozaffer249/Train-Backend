using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sudan_Train.Data.Entity;

namespace Sudan_Train.Infrastructure.Configurations
{
    public class StateConfiguration : IEntityTypeConfiguration<State>
    {
        public void Configure(EntityTypeBuilder<State> builder)
        {
            builder.HasKey(s => s.Id);

            builder.Property(s => s.NameEn)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(s => s.NameAr)
                .IsRequired()
                .HasMaxLength(100);

            builder.HasIndex(s => s.NameEn)
                .IsUnique();

            builder.HasMany(s => s.Cities)
                .WithOne(c => c.State)
                .HasForeignKey(c => c.StateId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}

