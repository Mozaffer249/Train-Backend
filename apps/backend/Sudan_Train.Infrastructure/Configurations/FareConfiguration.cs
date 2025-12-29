using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sudan_Train.Data.Entity;

namespace Sudan_Train.Infrastructure.Configurations
{
    public class FareConfiguration : IEntityTypeConfiguration<Fare>
    {
        public void Configure(EntityTypeBuilder<Fare> builder)
        {
            builder.HasKey(f => f.Id);

            builder.Property(f => f.BasePrice)
                .HasColumnType("decimal(18,2)");

            builder.Property(f => f.PricePerKm)
                .HasColumnType("decimal(18,2)");

            builder.Property(f => f.VatRate)
                .HasColumnType("decimal(5,2)")
                .HasDefaultValue(0.15m);

            builder.Property(f => f.DiscountPercent)
                .HasColumnType("decimal(5,2)");

            builder.Property(f => f.Currency)
                .HasMaxLength(3)
                .HasDefaultValue("SDG");

            builder.Property(f => f.CoachClass)
                .HasConversion<int>();

            builder.HasOne(f => f.Route)
                .WithMany()
                .HasForeignKey(f => f.RouteId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasOne(f => f.OriginStation)
                .WithMany()
                .HasForeignKey(f => f.OriginStationId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(f => f.DestinationStation)
                .WithMany()
                .HasForeignKey(f => f.DestinationStationId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(f => f.Trip)
                .WithMany()
                .HasForeignKey(f => f.TripId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}

