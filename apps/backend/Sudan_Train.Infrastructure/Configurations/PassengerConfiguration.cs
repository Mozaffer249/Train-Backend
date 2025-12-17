using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sudan_Train.Data.Entity;

namespace Sudan_Train.Infrastructure.Configurations
{
    public class PassengerConfiguration : IEntityTypeConfiguration<Passenger>
    {
        public void Configure(EntityTypeBuilder<Passenger> builder)
        {
            builder.HasKey(p => p.Id);

            builder.Property(p => p.FullNameEn)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(p => p.FullNameAr)
                .HasMaxLength(200);

            builder.Property(p => p.IdNumber)
                .IsRequired()
                .HasMaxLength(50);

            builder.HasIndex(p => p.IdNumber)
                .IsUnique();

            builder.Property(p => p.Gender)
                .HasMaxLength(10);

            builder.Property(p => p.Nationality)
                .HasMaxLength(50);

            builder.HasOne(p => p.User)
                .WithMany()
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasOne(p => p.City)
                .WithMany()
                .HasForeignKey(p => p.CityId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}

