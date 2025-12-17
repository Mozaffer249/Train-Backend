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

            builder.Property(t => t.Status)
                .HasMaxLength(20)
                .HasDefaultValue("Issued");

            builder.HasOne(t => t.BookingPassenger)
                .WithOne(bp => bp.Ticket)
                .HasForeignKey<Ticket>(t => t.BookingPassengerId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}

