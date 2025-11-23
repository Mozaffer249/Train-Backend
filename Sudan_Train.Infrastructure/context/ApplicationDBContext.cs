using System.Reflection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Sudan_Train.Data.Entity;
using Sudan_Train.Data.Entity.Identity;

namespace Sudan_Train.Infrastructure.context
{
    public class ApplicationDBContext : IdentityDbContext<User, Role, int, IdentityUserClaim<int>,
          IdentityUserRole<int>, IdentityUserLogin<int>, IdentityRoleClaim<int>,
          IdentityUserToken<int>>
    {
        //private readonly IEncryptionProvider _encryptionProvider;
        //public ApplicationDBContext()
        //{

        //}
        public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options) : base(options)
        {
            //_encryptionProvider = new GenerateEncryptionProvider("8a4dcaaec64d412380fe4b02193cd26f");
        }
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
        public DbSet<State> States { get; set; }
        public DbSet<Station> Stations { get; set; }
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<UserRefreshToken> UserRefreshTokens { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Apply all entity configurations from the Infrastructure project
            builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            // Identity table mappings
            builder.Entity<IdentityRole>().ToTable("Roles", "security");
            builder.Entity<IdentityUserRole<int>>().ToTable("UserRoles", "security");
            builder.Entity<IdentityUserClaim<int>>().ToTable("UserClaims", "security");
            builder.Entity<IdentityUserLogin<int>>().ToTable("UserLogins", "security");
            builder.Entity<IdentityRoleClaim<int>>().ToTable("RoleClaims", "security");
            builder.Entity<IdentityUserToken<int>>().ToTable("UserTokens", "security");

            //modelBuilder.UseEncryption(_encryptionProvider);
        }
    }
}
