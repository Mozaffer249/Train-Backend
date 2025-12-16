using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sudan_Train.Data.Entity.Identity;

namespace Sudan_Train.Infrastructure.Configurations
{
    public class LoginSessionConfiguration : IEntityTypeConfiguration<LoginSession>
    {
        public void Configure(EntityTypeBuilder<LoginSession> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.DeviceId)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(x => x.DeviceName)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(x => x.IpAddress)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(x => x.UserAgent)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(x => x.AccessToken)
                .IsRequired()
                .HasMaxLength(2000); // JWT tokens can be 500-1000+ characters

            builder.Property(x => x.RefreshToken)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(x => x.Location)
                .HasMaxLength(100);

            // Indexes
            builder.HasIndex(x => x.UserId);
            builder.HasIndex(x => new { x.UserId, x.IsActive });
            builder.HasIndex(x => x.LoginTime);
            builder.HasIndex(x => x.AccessToken);

            // Relationships
            builder.HasOne(x => x.User)
                .WithMany(u => u.LoginSessions)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
