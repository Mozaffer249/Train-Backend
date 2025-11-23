using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sudan_Train.Data.Entity;

namespace Sudan_Train.Infrastructure.Configurations
{
    public class StationConfiguration : IEntityTypeConfiguration<Station>
    {
        public void Configure(EntityTypeBuilder<Station> builder)
        {
            builder.HasKey(s => s.Id);

            builder.Property(s => s.Code)
                .IsRequired()
                .HasMaxLength(20);

            builder.HasIndex(s => s.Code)
                .IsUnique();

            builder.Property(s => s.NameEn)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(s => s.NameAr)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(s => s.AddressEn)
                .HasMaxLength(500);

            builder.Property(s => s.AddressAr)
                .HasMaxLength(500);

            builder.HasOne(s => s.City)
                .WithMany(c => c.Stations)
                .HasForeignKey(s => s.CityId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(s => s.RouteStations)
                .WithOne(rs => rs.Station)
                .HasForeignKey(rs => rs.StationId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}

