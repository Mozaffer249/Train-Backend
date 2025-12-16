using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sudan_Train.Data.Entity.Identity;

namespace Sudan_Train.Infrastructure.Configurations.Identity
{
    public class EmailConfirmationOtpConfiguration : IEntityTypeConfiguration<EmailConfirmationOtp>
    {
        public void Configure(EntityTypeBuilder<EmailConfirmationOtp> builder)
        {
            builder.ToTable("EmailConfirmationOtps", "security");

            builder.HasKey(e => e.Id);

            builder.Property(e => e.OtpCode)
                .IsRequired()
                .HasMaxLength(4);

            builder.Property(e => e.CreatedAt)
                .IsRequired();

            builder.Property(e => e.ExpiresAt)
                .IsRequired();

            builder.HasIndex(e => new { e.UserId, e.OtpCode })
                .HasDatabaseName("IX_EmailConfirmationOtp_UserId_Code");

            builder.HasIndex(e => e.ExpiresAt)
                .HasDatabaseName("IX_EmailConfirmationOtp_ExpiresAt");

            builder.HasOne(e => e.User)
                .WithMany()
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
