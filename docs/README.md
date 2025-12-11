# Sudan Train Backend - Documentation

Welcome to the Sudan Train Backend documentation. This documentation is organized by domain to help you find what you need quickly.

## 📁 Documentation Structure

### 🚀 [Deployment](./deployment)
Everything you need to deploy and run the application.
- **[Docker Setup](./deployment/docker-setup.md)** - Complete Docker configuration and setup
- **[Quickstart Guide](./deployment/quickstart.md)** - Get up and running quickly
- **[Deployment Guide](./deployment/deployment-guide.md)** - Production deployment instructions

### ⚙️ [Configuration](./configuration)
Application configuration and settings guides.
- **[AppSettings Guide](./configuration/appsettings-guide.md)** - Configuration file documentation
- **[Configuration Overview](./configuration/configuration.md)** - General configuration guidelines

### 🏗️ [Architecture](./architecture)
System architecture and implementation details.
- **[Messaging API](./architecture/messaging-api.md)** - Microservice for Email, SMS, and Push notifications
- **[Email Service](./architecture/email-service.md)** - Email service implementation details
- **[Email Strategy](./architecture/email-strategy.md)** - Email sending strategies (Direct, Queued, Fallback)

### 💻 [Development](./development)
Development guides, refactoring summaries, and fixes.
- **[Register Handler Refactoring](./development/register-handler-refactoring.md)** - Clean code refactoring example
- **[Localization Refactoring](./development/localization-refactoring.md)** - Localization improvements
- **[Property Name Fix](./development/property-name-fix.md)** - Property naming convention fixes
- **[Missing Fields Fix](./development/missing-fields-fix.md)** - Schema fixes

### 🗄️ [Database](./database)
Database setup, migrations, and schema documentation.
- **[Database Setup](./database/database-setup.md)** - Database configuration and initialization

### 🌍 [Localization](./localization)
Internationalization and localization documentation.
- **[Validator Localization](./localization/validator-localization.md)** - Validation message localization

## 🎯 Quick Links

### Getting Started
1. [Docker Quickstart](./deployment/quickstart.md) - Fastest way to run the project
2. [Database Setup](./database/database-setup.md) - Initialize the database
3. [Configuration Guide](./configuration/appsettings-guide.md) - Configure your environment

### Key Features
- **Microservices Architecture** - [Messaging API Documentation](./architecture/messaging-api.md)
- **Multi-language Support** - [Localization Guide](./localization/validator-localization.md)
- **Clean Code Practices** - [Refactoring Examples](./development/register-handler-refactoring.md)

## 📝 Contributing

When adding new documentation:
1. Place it in the appropriate domain folder
2. Update the folder's README.md
3. Add a link in this main README.md
4. Follow the existing naming conventions (kebab-case)

## 🔍 Need Help?

- **Deployment Issues?** Check [Deployment Guide](./deployment/deployment-guide.md)
- **Configuration Problems?** See [AppSettings Guide](./configuration/appsettings-guide.md)
- **Architecture Questions?** Browse [Architecture Docs](./architecture)
- **Development Guidelines?** Review [Development Docs](./development)
