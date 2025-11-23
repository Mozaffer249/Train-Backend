using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sudan_Train.Data.Entity;

namespace Sudan_Train.Infrastructure.Configurations
{
    public class RouteStationConfiguration : IEntityTypeConfiguration<RouteStation>
    {
        public void Configure(EntityTypeBuilder<RouteStation> builder)
        {
            builder.HasKey(rs => rs.Id);

            builder.HasIndex(rs => new { rs.RouteId, rs.StopOrder })
                .IsUnique();

            builder.HasIndex(rs => new { rs.RouteId, rs.StationId });

            builder.Property(rs => rs.ArrivalOffset)
                .HasColumnType("time");

            builder.Property(rs => rs.DepartureOffset)
                .HasColumnType("time");

            builder.HasOne(rs => rs.Route)
                .WithMany(r => r.RouteStations)
                .HasForeignKey(rs => rs.RouteId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(rs => rs.Station)
                .WithMany(s => s.RouteStations)
                .HasForeignKey(rs => rs.StationId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}

