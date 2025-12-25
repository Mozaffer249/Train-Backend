using EntityFrameworkCore.EncryptColumn.Extension;
using EntityFrameworkCore.EncryptColumn.Interfaces;
using EntityFrameworkCore.EncryptColumn.Util;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Sudan_Train.Data.Commons;
using Sudan_Train.Data.Entity;
using Sudan_Train.Data.Entity.Identity;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Sudan_Train.Infrastructure.context
{
    public class ApplicationDBContext : IdentityDbContext<User, Role, int, IdentityUserClaim<int>,
          IdentityUserRole<int>, IdentityUserLogin<int>, IdentityRoleClaim<int>,
          IdentityUserToken<int>>
    {
        private readonly IEncryptionProvider _encryptionProvider;
        //public ApplicationDBContext()
        //{
        //
        //}
        public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options) : base(options)
        {
            _encryptionProvider = new GenerateEncryptionProvider("8a4dcaaec64d412380fe4b02193cd26f");
        }
        // Existing DbSets
        public DbSet<Train> Trains { get; set; }
        public DbSet<Coach> Coaches { get; set; }
        public DbSet<Seat> Seats { get; set; }
        public DbSet<Trip> Trip { get; set; }
        public DbSet<TripSeat> TripSeats { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<BookingPassenger> BookingPassengers { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<Fare> Fares { get; set; }
        public DbSet<Passenger> Passengers { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Route> Routes { get; set; }
        public DbSet<RouteStation> RouteStations { get; set; }
        public DbSet<Station> Stations { get; set; }
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<UserRefreshToken> UserRefreshTokens { get; set; }

        // New DbSets for enhanced functionality
        public DbSet<Refund> Refunds { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<TrainSchedule> TrainSchedules { get; set; }
        public DbSet<Promotion> Promotions { get; set; }
        public DbSet<PromotionUsage> PromotionUsages { get; set; }

        // Authentication & Security DbSets
        public DbSet<LoginSession> LoginSessions { get; set; }
        public DbSet<TrustedDevice> TrustedDevices { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }
        public DbSet<SecurityEvent> SecurityEvents { get; set; }
        public DbSet<PasswordHistory> PasswordHistories { get; set; }
        public DbSet<TwoFactorRecoveryCode> TwoFactorRecoveryCodes { get; set; }
        public DbSet<EmailConfirmationOtp> EmailConfirmationOtps { get; set; }
        public DbSet<PasswordResetOtp> PasswordResetOtps { get; set; }
        public DbSet<IpLoginAttempt> IpLoginAttempts { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Apply all entity configurations from the Infrastructure project
            builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            // Identity table mappings
            builder.Entity<User>().ToTable("Users", "security");
            builder.Entity<Role>().ToTable("Roles", "security");
            builder.Entity<IdentityUserRole<int>>().ToTable("UserRoles", "security");
            builder.Entity<IdentityUserClaim<int>>().ToTable("UserClaims", "security");
            builder.Entity<IdentityUserLogin<int>>().ToTable("UserLogins", "security");
            builder.Entity<IdentityRoleClaim<int>>().ToTable("RoleClaims", "security");
            builder.Entity<IdentityUserToken<int>>().ToTable("UserTokens", "security");

            // Apply encryption
            builder.UseEncryption(_encryptionProvider);

            // Apply global query filters for soft delete
            ApplySoftDeleteQueryFilters(builder);
        }

        private void ApplySoftDeleteQueryFilters(ModelBuilder builder)
        {
            foreach (var entityType in builder.Model.GetEntityTypes())
            {
                if (typeof(SoftDeletableEntity).IsAssignableFrom(entityType.ClrType))
                {
                    var parameter = Expression.Parameter(entityType.ClrType, "e");
                    var property = Expression.Property(parameter, nameof(SoftDeletableEntity.IsDeleted));
                    var filter = Expression.Lambda(Expression.Equal(property, Expression.Constant(false)), parameter);

                    entityType.SetQueryFilter(filter);
                }
            }
        }
    }
}
