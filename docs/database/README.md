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

### [Migration Guide](./migration-guide.md)
Comprehensive guide for applying the database improvements migration:
- Pre-migration checklist and backup procedures
- Migration execution steps
- Post-migration verification
- Breaking changes and their impacts
- Rollback procedures
- Troubleshooting common issues

### [Entity Relationship Diagram](./entity-relationship-diagram.md)
Visual and detailed documentation of the complete database schema:
- Entity relationship diagrams
- Entity categorization
- Relationship explanations
- Common query patterns
- Performance considerations

### [Quick Reference Guide](./quick-reference.md)
Concise reference for developers:
- Quick commands for migrations
- Code examples for new entities
- Common query patterns
- Performance tips
- Troubleshooting guide
- Enums cheat sheet

### [Sudanese Geographic Data Seeding](./sudanese-geographic-data-seeding.md)
Comprehensive guide for the geographic hierarchy seeding system:
- 6 Sudanese regions with codes (KRT, EST, NTH, CNT, KRD, DRF)
- 18 states organized by region with Arabic names
- 145+ cities with bilingual support
- Three-level hierarchy: Region → State → City
- Automatic seeding on application startup
- Testing and verification queries
- Data quality and maintenance guidelines

### [Regional Hierarchy Usage](./regional-hierarchy-usage.md)
Quick reference for using the regional structure:
- Query patterns for regions, states, and cities
- Cascading dropdown implementation
- Regional analytics examples
- Region codes reference
- Performance tips

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

### Current Migration
**ComprehensiveDatabaseImprovement** (December 11, 2025)
- Added 5 new entities (Refund, Notification, TrainSchedule, Promotion, PromotionUsage)
- Added audit trails to core entities
- Enhanced security with encryption
- Performance improvements with 40+ indexes
- Breaking changes: Removed redundant columns

**⚠️ See [Migration Guide](./migration-guide.md) for detailed instructions**

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
