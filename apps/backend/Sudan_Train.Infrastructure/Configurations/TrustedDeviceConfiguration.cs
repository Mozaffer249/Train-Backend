using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sudan_Train.Data.Entity.Identity;

namespace Sudan_Train.Infrastructure.Configurations
{
    /* Temporarily disabled — TrustedDevice table dropped (DropAdvancedSecurityTables migration).
       Commented out so ApplyConfigurationsFromAssembly does not re-add the entity to the model.
    public class TrustedDeviceConfiguration : IEntityTypeConfiguration<TrustedDevice>
    {
        public void Configure(EntityTypeBuilder<TrustedDevice> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.DeviceId)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(x => x.DeviceName)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(x => x.DeviceFingerprint)
                .IsRequired()
                .HasMaxLength(500);

            // Indexes
            builder.HasIndex(x => x.UserId);
            builder.HasIndex(x => new { x.UserId, x.DeviceId });
            builder.HasIndex(x => new { x.UserId, x.IsActive });

            // Relationships
            builder.HasOne(x => x.User)
                .WithMany(u => u.TrustedDevices)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
    */
}
