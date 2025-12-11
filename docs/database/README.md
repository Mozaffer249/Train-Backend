# Database Documentation

Database setup, migrations, schema, and maintenance documentation.

## 📄 Documents

### [Database Setup](./database-setup.md)
Complete database setup guide:
- SQL Server installation
- Database creation
- Connection string configuration
- Initial migration
- Seeding data
- Docker database setup

## 🗄️ Database Information

### Database Provider
- **SQL Server 2022** (Production)
- **Azure SQL Edge** (Development - ARM64 compatible)

### Entity Framework Core
- Code-First approach
- Automatic migrations
- Database seeding

### Schema Organization
- `security` - User, roles, authentication
- `dbo` - Main application tables

## 🔄 Migrations

### Create Migration
```bash
dotnet ef migrations add MigrationName --project Sudan_Train.Infrastructure
```

### Apply Migration
```bash
dotnet ef database update --project Sudan_Train.Infrastructure
```

### Rollback Migration
```bash
dotnet ef database update PreviousMigrationName --project Sudan_Train.Infrastructure
```

## 🌱 Data Seeding

The application includes seeders for:
- **DatabaseSeeder** - Creates database if not exists
- **RoleSeeder** - Seeds default roles (Admin, User)
- **UserSeeder** - Seeds default admin user

## 🔒 Security

- Use parameterized queries (EF Core handles this)
- Never store plain text passwords
- Use connection string secrets management
- Implement row-level security where needed

## 🔗 Related Documentation

- [Configuration](../configuration/appsettings-guide.md) - Connection strings
- [Deployment](../deployment/docker-setup.md) - Database container setup
