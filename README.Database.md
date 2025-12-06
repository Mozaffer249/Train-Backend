# Database Auto-Initialization & Seeding

## 🎯 Overview

Your Sudan Train Backend now includes **automatic database initialization and seeding** that runs on application startup. This ensures:

✅ Database is created automatically if it doesn't exist
✅ Tables are created from your Entity Framework models
✅ Pending migrations are applied automatically
✅ Default roles and admin user are seeded

## 🔧 What Was Added

### 1. **DatabaseSeeder.cs** - Auto Database Creation

**Location**: `Sudan_Train.Infrastructure/Seeder/DatabaseSeeder.cs`

**Features**:
- Checks if database exists, creates if missing
- Checks if tables exist, creates if missing
- Applies pending migrations automatically
- Comprehensive logging for each step

**Code**:
```csharp
var databaseCreator = _context.GetService<IRelationalDatabaseCreator>();

if (!databaseCreator.CanConnect())
{
    databaseCreator.Create();  // Creates the database
}

if (!databaseCreator.HasTables())
{
    databaseCreator.CreateTables();  // Creates all tables
}

await _context.Database.MigrateAsync();  // Applies migrations
```

### 2. **RoleSeeder.cs** - Default Roles

**Location**: `Sudan_Train.Infrastructure/Seeder/RoleSeeder.cs`

**Seeds**:
- `Admin` role
- `User` role

**Code**:
```csharp
await _roleManager.CreateAsync(new Role()
{
    Name = "Admin",
    NormalizedName = "ADMIN"
});
```

### 3. **UserSeeder.cs** - Default Admin User

**Location**: `Sudan_Train.Infrastructure/Seeder/UserSeeder.cs`

**Creates**:
- **Username**: `admin`
- **Email**: `admin@project.com`
- **Password**: `Admin@123`
- **Role**: Admin
- **Status**: Active, Email Confirmed

**Default Credentials**:
```
Username: admin
Password: Admin@123
Email: admin@project.com
```

### 4. **Program.cs** - Auto Initialization on Startup

**Location**: `Sudan_Train/Program.cs`

**Added Code**:
```csharp
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    
    var databaseSeeder = services.GetRequiredService<DatabaseSeeder>();
    await databaseSeeder.InitializeAsync();
    
    var roleSeeder = services.GetRequiredService<RoleSeeder>();
    await roleSeeder.SeedAsync();
    
    var userSeeder = services.GetRequiredService<UserSeeder>();
    await userSeeder.SeedAsync();
}
```

## 🚀 How It Works

### Application Startup Sequence:

1. **Application Starts** → `Program.cs` executes
2. **Database Check** → `DatabaseSeeder` checks if database exists
3. **Database Creation** → Creates database if missing
4. **Table Creation** → Creates all tables from EF Core models
5. **Migration Application** → Applies any pending migrations
6. **Role Seeding** → Seeds Admin and User roles (if not exist)
7. **User Seeding** → Creates default admin user (if not exist)
8. **Application Ready** → API starts accepting requests

### Startup Logs Example:

```
[INF] Creating database...
[INF] Database created successfully.
[INF] Creating database tables...
[INF] Database tables created successfully.
[INF] Applying pending migrations...
[INF] Migrations applied successfully.
[INF] Seeding roles...
[INF] Roles seeded successfully.
[INF] Seeding default admin user...
[INF] Default admin user created successfully.
[INF] Username: admin, Password: Admin@123
[INF] Database initialization and seeding completed successfully.
```

## 📝 Usage

### First Time Setup

1. **Update Connection String** in `appsettings.json`:
   ```json
   "ConnectionStrings": {
     "dbcontext": "Server=YOUR_SERVER;Database=TrainsDb;Trusted_Connection=True;TrustServerCertificate=True;"
   }
   ```

2. **Run the Application**:
   ```bash
   dotnet run --project Sudan_Train
   ```

3. **Database Auto-Created** - Check the logs!

### Docker Setup

When using Docker, the database is automatically created:

```bash
docker-compose up -d
```

The SQL Server container starts, and the API:
- Waits for SQL Server to be ready
- Creates the `TrainsDb` database
- Creates all tables
- Seeds roles and admin user

### Manual Migration (Optional)

If you prefer manual control:

```bash
# Create a new migration
dotnet ef migrations add MigrationName --project Sudan_Train.Infrastructure --startup-project Sudan_Train

# Apply migrations manually
dotnet ef database update --project Sudan_Train.Infrastructure --startup-project Sudan_Train

# Remove last migration
dotnet ef migrations remove --project Sudan_Train.Infrastructure --startup-project Sudan_Train
```

## 🔐 Default Admin Credentials

```
Username: admin
Email: admin@project.com
Password: Admin@123
Role: Admin
```

**⚠️ Important**: Change the default admin password after first login in production!

## 🎨 Customization

### Add More Roles

Edit `Sudan_Train.Infrastructure/Seeder/RoleSeeder.cs`:

```csharp
await _roleManager.CreateAsync(new Role()
{
    Name = "Manager",
    NormalizedName = "MANAGER"
});

await _roleManager.CreateAsync(new Role()
{
    Name = "Staff",
    NormalizedName = "STAFF"
});
```

### Add More Default Users

Edit `Sudan_Train.Infrastructure/Seeder/UserSeeder.cs`:

```csharp
var testUser = new User()
{
    UserName = "testuser",
    Email = "test@project.com",
    FirstName = "Test",
    LastName = "User",
    // ... other properties
};
await _userManager.CreateAsync(testUser, "Test@123");
await _userManager.AddToRoleAsync(testUser, "User");
```

### Seed Sample Data (States, Cities, Stations)

Create `Sudan_Train.Infrastructure/Seeder/LocationSeeder.cs`:

```csharp
public class LocationSeeder
{
    private readonly ApplicationDBContext _context;
    
    public async Task SeedAsync()
    {
        if (!_context.States.Any())
        {
            var khartoum = new State 
            { 
                NameEn = "Khartoum", 
                NameAr = "الخرطوم" 
            };
            await _context.States.AddAsync(khartoum);
            await _context.SaveChangesAsync();
        }
    }
}
```

Register in `ModuleInfrastructureDependencies.cs`:
```csharp
services.AddTransient<LocationSeeder>();
```

Call in `Program.cs`:
```csharp
var locationSeeder = services.GetRequiredService<LocationSeeder>();
await locationSeeder.SeedAsync();
```

## 🔍 Troubleshooting

### Database Already Exists Error

**Symptom**: Error says database exists but you want to recreate

**Solution**:
```bash
# Drop the database
dotnet ef database drop --project Sudan_Train.Infrastructure --startup-project Sudan_Train

# Restart the application (auto-creates)
dotnet run --project Sudan_Train
```

### Migration Errors

**Symptom**: Migration fails to apply

**Solution**:
```bash
# Remove migrations folder
rm -rf Sudan_Train.Infrastructure/Migrations/*

# Create new initial migration
dotnet ef migrations add InitialCreate --project Sudan_Train.Infrastructure --startup-project Sudan_Train

# Restart application
dotnet run --project Sudan_Train
```

### Connection String Issues

**Symptom**: Cannot connect to database

**Solutions**:
- Check SQL Server is running
- Verify connection string in `appsettings.json`
- For Docker: use `Server=sqlserver,1433` (service name)
- For local: use `Server=localhost,1433` or `Server=.`

### Roles/Users Not Seeding

**Symptom**: Roles or users don't appear

**Check**:
```csharp
// The seeders only run if tables are empty
if (rolesCount <= 0)  // Only seeds if no roles exist
```

**Solution**: If you want to re-seed, delete the roles/users first

## 📊 Database Schema

After auto-initialization, your database will have:

**Security Schema** (ASP.NET Identity):
- `security.Users`
- `security.Roles`
- `security.UserRoles`
- `security.UserClaims`
- `security.RoleClaims`
- `security.UserLogins`
- `security.UserTokens`

**Application Schema** (dbo):
- `States`, `Cities`, `Stations`
- `Trains`, `Coaches`, `Seats`
- `Routes`, `RouteStations`
- `Trips`, `TripSeats`
- `Bookings`, `BookingPassengers`
- `Passengers`, `Payments`, `Tickets`, `Fares`
- `UserRefreshTokens`

## ✅ Benefits

1. **Zero Manual Setup** - No need to run migrations manually
2. **Docker Friendly** - Works perfectly in containers
3. **Development Speed** - Fresh database in seconds
4. **CI/CD Ready** - Automated deployment
5. **Consistent State** - Always has default data
6. **Error Handling** - Comprehensive logging
7. **Idempotent** - Safe to run multiple times

## 🎉 Summary

Your database now **auto-initializes on every startup**:

✅ No manual `dotnet ef database update` needed
✅ No manual role creation
✅ No manual admin user setup
✅ Works in Docker, Development, and Production
✅ Comprehensive logging for debugging
✅ Safe and idempotent (won't duplicate data)

**Just run the app and everything is ready!** 🚀

