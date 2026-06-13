using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sudan_Train.Data.Entity;

namespace Sudan_Train.Infrastructure.Configurations
{
    public class TicketConfiguration : IEntityTypeConfiguration<Ticket>
    {
        public void Configure(EntityTypeBuilder<Ticket> builder)
        {
            builder.HasKey(t => t.Id);

            builder.Property(t => t.TicketNumber)
                .IsRequired()
                .HasMaxLength(50);

            builder.HasIndex(t => t.TicketNumber)
                .IsUnique();

            // Status is now an enum stored as int. Existing column was
            // nvarchar(20); the AddStaffStationsAndOpsEnums migration converts it.
            builder.Property(t => t.Status)
                .HasConversion<int>()
                .HasDefaultValue(TicketStatus.Issued);

            builder.HasOne(t => t.BookingPassenger)
                .WithOne(bp => bp.Ticket)
                .HasForeignKey<Ticket>(t => t.BookingPassengerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(t => t.BoardedByUser)
                .WithMany()
                .HasForeignKey(t => t.BoardedByUserId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
