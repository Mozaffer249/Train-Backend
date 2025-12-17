# Sudan Train Backend - Documentation

Welcome to the Sudan Train Backend documentation. This documentation is organized by domain to help you find what you need quickly.

## 📁 Documentation Structure

### 🔐 [Authentication](./authentication)
Complete authentication system documentation.
- **[Module 1: Core Security](./authentication/module-1-core-security-fixes.md)** - Account lockout, email confirmation, logout
- **[Module 2: Two-Factor Auth](./authentication/module-2-two-factor-authentication.md)** - TOTP-based 2FA
- **[Authentication Status](./authentication/authentication-final-status.md)** - Complete implementation status
- **[OTP System](./authentication/complete-otp-system-summary.md)** - OTP implementation details

### 🚀 [Deployment](./deployment)
Everything you need to deploy and run the application.
- **[Docker Setup](./deployment/docker-setup.md)** - Complete Docker configuration and setup
- **[Quickstart Guide](./deployment/quickstart.md)** - Get up and running quickly
- **[Deployment Guide](./deployment/deployment-guide.md)** - Production deployment instructions
- **[Localhost Setup](./deployment/localhost-setup-guide.md)** - Local development setup
- **[Localhost vs Production](./deployment/localhost-vs-production-url.md)** - URL configuration guide

### 🧪 [Testing](./testing)
Testing guides and resources.
- **[Postman Testing Guide](./testing/postman-testing-guide.md)** - Complete Postman testing workflow
- **[Postman Collection Summary](./testing/postman-collection-summary.md)** - Collection structure overview
- **[Testing Quick Reference](./testing/testing-quick-reference.md)** - Common testing tasks

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
Development guides, implementation summaries, and fixes.
- **[Implementation Progress](./development/implementation-progress.md)** - Overall project progress
- **[Next Steps Guide](./development/next-steps-guide.md)** - Recommended next steps
- **[Module 6-8 Summary](./development/module-6-8-implementation-summary.md)** - Account management & notifications
- **[Register Handler Refactoring](./development/register-handler-refactoring.md)** - Clean code refactoring example
- **[Localization Refactoring](./development/localization-refactoring.md)** - Localization improvements

### 🗄️ [Database](./database)
Database setup, migrations, and schema documentation.
- **[Database Setup](./database/database-setup.md)** - Database configuration and initialization
- **[Migration Guide](./database/migration-guide.md)** - Database migrations
- **[Database Improvements](./database/database-improvement-summary.md)** - Schema improvements
- **[Geographic Data](./database/sudanese-geographic-data-implementation.md)** - Sudanese geographic data
- **[Region Entity](./database/region-entity-implementation-summary.md)** - Regional hierarchy

### 🌍 [Localization](./localization)
Internationalization and localization documentation.
- **[Validator Localization](./localization/validator-localization.md)** - Validation message localization

## 🎯 Quick Links

### Getting Started
1. [Docker Quickstart](./deployment/quickstart.md) - Fastest way to run the project
2. [Localhost Setup](./deployment/localhost-setup-guide.md) - Set up local development
3. [Database Setup](./database/database-setup.md) - Initialize the database
4. [Configuration Guide](./configuration/appsettings-guide.md) - Configure your environment

### Authentication
- [Authentication Status](./authentication/authentication-final-status.md) - Current implementation status
- [Two-Factor Setup](./authentication/module-2-two-factor-authentication.md) - Enable 2FA
- [Testing Auth APIs](./testing/postman-testing-guide.md) - Test with Postman

### Key Features
- **Authentication System** - [Authentication Documentation](./authentication)
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

- **Authentication Issues?** Check [Authentication Docs](./authentication)
- **Deployment Issues?** Check [Deployment Guide](./deployment/deployment-guide.md)
- **Testing Questions?** See [Testing Guide](./testing/postman-testing-guide.md)
- **Configuration Problems?** See [AppSettings Guide](./configuration/appsettings-guide.md)
- **Architecture Questions?** Browse [Architecture Docs](./architecture)
- **Development Guidelines?** Review [Development Docs](./development)
