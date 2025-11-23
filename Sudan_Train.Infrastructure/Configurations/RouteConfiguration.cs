using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sudan_Train.Data.Entity;

namespace Sudan_Train.Infrastructure.Configurations
{
    public class RouteConfiguration : IEntityTypeConfiguration<Route>
    {
        public void Configure(EntityTypeBuilder<Route> builder)
        {
            builder.HasKey(r => r.Id);

            builder.Property(r => r.NameEn)
                .HasMaxLength(200);

            builder.Property(r => r.NameAr)
                .HasMaxLength(200);

            builder.Property(r => r.DistanceKm)
                .HasColumnType("decimal(18,2)");

            builder.HasOne(r => r.OriginStation)
                .WithMany()
                .HasForeignKey(r => r.OriginStationId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(r => r.DestinationStation)
                .WithMany()
                .HasForeignKey(r => r.DestinationStationId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(r => r.RouteStations)
                .WithOne(rs => rs.Route)
                .HasForeignKey(rs => rs.RouteId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}

