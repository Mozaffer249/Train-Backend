using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sudan_Train.Data.Entity.Identity;

namespace Sudan_Train.Infrastructure.Configurations.Identity
{
    public class IpLoginAttemptConfiguration : IEntityTypeConfiguration<IpLoginAttempt>
    {
        public void Configure(EntityTypeBuilder<IpLoginAttempt> builder)
        {
            builder.ToTable("IpLoginAttempts", "security");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.IpAddress)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(x => x.AttemptTime)
                .IsRequired();

            builder.Property(x => x.WasSuccessful)
                .IsRequired();

            builder.Property(x => x.UserName)
                .HasMaxLength(256);

            // Create composite index for efficient queries
            builder.HasIndex(x => new { x.IpAddress, x.AttemptTime })
                .HasDatabaseName("IX_IpLoginAttempts_IpAddress_AttemptTime");

            // Index for cleanup queries
            builder.HasIndex(x => x.AttemptTime)
                .HasDatabaseName("IX_IpLoginAttempts_AttemptTime");
        }
    }
}
