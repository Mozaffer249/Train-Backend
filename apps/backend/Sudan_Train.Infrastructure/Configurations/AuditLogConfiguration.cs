using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sudan_Train.Data.Entity.Identity;

namespace Sudan_Train.Infrastructure.Configurations
{
    /* Temporarily disabled — AuditLog table dropped (DropAdvancedSecurityTables migration).
       Commented out so ApplyConfigurationsFromAssembly does not re-add the entity to the model.
    public class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
    {
        public void Configure(EntityTypeBuilder<AuditLog> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Action)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(x => x.EntityType)
                .HasMaxLength(100);

            builder.Property(x => x.EntityId)
                .HasMaxLength(100);

            builder.Property(x => x.IpAddress)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(x => x.UserAgent)
                .HasMaxLength(500);

            builder.Property(x => x.FailureReason)
                .HasMaxLength(500);

            // Indexes
            builder.HasIndex(x => x.UserId);
            builder.HasIndex(x => x.Action);
            builder.HasIndex(x => x.Timestamp);
            builder.HasIndex(x => x.IpAddress);
            builder.HasIndex(x => new { x.UserId, x.Action });

            // Relationships
            builder.HasOne(x => x.User)
                .WithMany(u => u.AuditLogs)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
    */
}
