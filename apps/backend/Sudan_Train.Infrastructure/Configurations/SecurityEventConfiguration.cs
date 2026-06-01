using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sudan_Train.Data.Entity.Identity;

namespace Sudan_Train.Infrastructure.Configurations
{
    /* Temporarily disabled — SecurityEvent table dropped (DropAdvancedSecurityTables migration).
       Commented out so ApplyConfigurationsFromAssembly does not re-add the entity to the model.
    public class SecurityEventConfiguration : IEntityTypeConfiguration<SecurityEvent>
    {
        public void Configure(EntityTypeBuilder<SecurityEvent> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.EventType)
                .IsRequired();

            builder.Property(x => x.IpAddress)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(x => x.Details)
                .IsRequired();

            // Indexes
            builder.HasIndex(x => x.UserId);
            builder.HasIndex(x => x.EventType);
            builder.HasIndex(x => x.OccurredAt);
            builder.HasIndex(x => new { x.UserId, x.EventType });
            builder.HasIndex(x => new { x.UserId, x.WasNotified });

            // Relationships
            builder.HasOne(x => x.User)
                .WithMany(u => u.SecurityEvents)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
    */
}
