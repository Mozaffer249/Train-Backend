using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sudan_Train.Data.Entity;

namespace Sudan_Train.Infrastructure.Configurations
{
    public class TrainScheduleConfiguration : IEntityTypeConfiguration<TrainSchedule>
    {
        public void Configure(EntityTypeBuilder<TrainSchedule> builder)
        {
            builder.HasKey(ts => ts.Id);

            builder.Property(ts => ts.Name)
                .IsRequired()
                .HasMaxLength(100);

            // Add indexes for schedule queries
            builder.HasIndex(ts => ts.TrainId);
            builder.HasIndex(ts => ts.RouteId);
            builder.HasIndex(ts => new { ts.IsActive, ts.EffectiveFrom, ts.EffectiveTo });

            builder.Property(ts => ts.RecurrenceType)
                .HasConversion<int>();

            builder.Property(ts => ts.DaysOfWeek)
                .HasMaxLength(100);

            builder.HasOne(ts => ts.Train)
                .WithMany(t => t.TrainSchedules)
                .HasForeignKey(ts => ts.TrainId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(ts => ts.Route)
                .WithMany(r => r.TrainSchedules)
                .HasForeignKey(ts => ts.RouteId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
