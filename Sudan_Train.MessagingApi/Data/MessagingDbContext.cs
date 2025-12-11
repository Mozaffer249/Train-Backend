using Microsoft.EntityFrameworkCore;

namespace Sudan_Train.MessagingApi.Data
{
    public class MessagingDbContext : DbContext
    {
        public MessagingDbContext(DbContextOptions<MessagingDbContext> options) : base(options)
        {
        }

        public DbSet<MessageLog> MessageLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<MessageLog>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.HasIndex(e => e.MessageId)
                    .IsUnique();

                entity.HasIndex(e => e.QueuedAt);

                entity.HasIndex(e => e.Status);

                entity.Property(e => e.MessageId)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.Recipient)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.Subject)
                    .HasMaxLength(500);

                entity.Property(e => e.ErrorMessage)
                    .HasMaxLength(2000);
            });
        }
    }
}
