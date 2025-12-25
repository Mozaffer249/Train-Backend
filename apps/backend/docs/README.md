# Sudan Train Backend - Documentation

Backend-specific documentation for the .NET 8 API.

## 📁 Documentation Structure

```
apps/backend/docs/
├── architecture/       # System design, messaging, email service
├── authentication/     # Auth system, 2FA, security modules
├── configuration/      # AppSettings, environment configuration
├── database/           # Schema, migrations, geographic data
├── development/        # Implementation guides, best practices
├── localization/       # Multi-language support
└── testing/            # Postman collections, API testing
```

## 📚 Documentation Index

### 🔐 [Authentication](./authentication)

Complete authentication system documentation.

| Document | Description |
|----------|-------------|
| [Module 1: Core Security](./authentication/module-1-core-security-fixes.md) | Account lockout, email confirmation, logout |
| [Module 2: Two-Factor Auth](./authentication/module-2-two-factor-authentication.md) | TOTP-based 2FA |
| [Authentication Status](./authentication/authentication-final-status.md) | Implementation status |
| [OTP System](./authentication/complete-otp-system-summary.md) | OTP implementation |
| [Security Integration](./authentication/security-modules-integration-summary.md) | Security modules overview |

### 🏗️ [Architecture](./architecture)

System architecture and implementation details.

| Document | Description |
|----------|-------------|
| [Messaging API](./architecture/messaging-api.md) | Email, SMS, Push microservice |
| [Email Service](./architecture/email-service.md) | Email implementation |
| [Email Strategy](./architecture/email-strategy.md) | Direct, Queued, Fallback strategies |

### ⚙️ [Configuration](./configuration)

Application configuration and settings.

| Document | Description |
|----------|-------------|
| [AppSettings Guide](./configuration/appsettings-guide.md) | All configuration options |
| [Configuration Overview](./configuration/configuration.md) | Best practices |

### 🗄️ [Database](./database)

Database setup, migrations, and schema.

| Document | Description |
|----------|-------------|
| [Database Setup](./database/database-setup.md) | Configuration and initialization |
| [Migration Guide](./database/migration-guide.md) | Running migrations |
| [Quick Reference](./database/quick-reference.md) | Common database commands |
| [Entity Relationships](./database/entity-relationship-diagram.md) | Schema diagram |
| [Geographic Data](./database/sudanese-geographic-data-implementation.md) | Sudanese regions |

### 💻 [Development](./development)

Development guides and implementation summaries.

| Document | Description |
|----------|-------------|
| [Implementation Progress](./development/implementation-progress.md) | Project progress |
| [Next Steps Guide](./development/next-steps-guide.md) | Recommended actions |
| [Register Handler Refactoring](./development/register-handler-refactoring.md) | Clean code example |
| [Localization Refactoring](./development/localization-refactoring.md) | i18n improvements |
| [Module 6-8 Summary](./development/module-6-8-implementation-summary.md) | Account & notifications |

### 🌍 [Localization](./localization)

Internationalization and localization.

| Document | Description |
|----------|-------------|
| [Validator Localization](./localization/validator-localization.md) | Validation messages |

### 🧪 [Testing](./testing)

Testing guides and Postman collections. **[📦 Postman Collection](../postman/)** files are ready to import.

| Document | Description |
|----------|-------------|
| [Testing Overview](./testing/README.md) | Complete testing guide with Postman setup |
| [Postman Testing Guide](./testing/postman-testing-guide.md) | Step-by-step testing workflow |
| [Postman Collection Summary](./testing/postman-collection-summary.md) | Collection structure details |
| [Testing Quick Reference](./testing/testing-quick-reference.md) | Common testing commands |

## 🎯 Quick Links

### Getting Started

1. [Backend README](../README.md) - Project overview and setup
2. [Database Setup](./database/database-setup.md) - Initialize the database
3. [Configuration Guide](./configuration/appsettings-guide.md) - Configure your environment
4. [Testing Guide](./testing/postman-testing-guide.md) - Test the APIs

### Key Topics

- **Authentication** - [Authentication Docs](./authentication)
- **Microservices** - [Messaging API](./architecture/messaging-api.md)
- **Multi-language** - [Localization Guide](./localization/validator-localization.md)
- **Clean Code** - [Refactoring Examples](./development/register-handler-refactoring.md)

## 🔗 Related Documentation

| Location | Description |
|----------|-------------|
| [Platform Docs](../../../docs) | Deployment, Docker setup |
| [Frontend Docs](../../frontend/README.md) | React application |
| [Migration Guide](../../../MIGRATION_GUIDE.md) | App migration |

## 📝 Contributing

When adding new documentation:

1. Place it in the appropriate domain folder
2. Update the folder's README.md
3. Add a link in this main README.md
4. Follow kebab-case naming convention
