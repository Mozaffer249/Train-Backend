using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sudan_Train.Data.Entity.Identity;

namespace Sudan_Train.Infrastructure.Configurations
{
    public class StaffStationConfiguration : IEntityTypeConfiguration<StaffStation>
    {
        public void Configure(EntityTypeBuilder<StaffStation> builder)
        {
            builder.HasKey(s => s.Id);

            // One assignment per (user, station) pair.
            builder.HasIndex(s => new { s.UserId, s.StationId }).IsUnique();

            builder.HasOne(s => s.User)
                .WithMany(u => u.StaffStations)
                .HasForeignKey(s => s.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(s => s.Station)
                .WithMany()
                .HasForeignKey(s => s.StationId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
