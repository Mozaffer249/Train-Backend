using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sudan_Train.Data.Entity.Identity;

namespace Sudan_Train.Infrastructure.Configurations.Identity
{
    public class PasswordResetOtpConfiguration : IEntityTypeConfiguration<PasswordResetOtp>
    {
        public void Configure(EntityTypeBuilder<PasswordResetOtp> builder)
        {
            builder.ToTable("PasswordResetOtps", "security");

            builder.HasKey(e => e.Id);

            builder.Property(e => e.OtpCode)
                .IsRequired()
                .HasMaxLength(6);

            builder.Property(e => e.CreatedAt)
                .IsRequired();

            builder.Property(e => e.ExpiresAt)
                .IsRequired();

            builder.HasIndex(e => new { e.UserId, e.OtpCode })
                .HasDatabaseName("IX_PasswordResetOtp_UserId_Code");

            builder.HasIndex(e => e.ExpiresAt)
                .HasDatabaseName("IX_PasswordResetOtp_ExpiresAt");

            builder.HasOne(e => e.User)
                .WithMany()
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
