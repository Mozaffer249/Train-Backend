# 📚 Documentation Index

Complete index of all documentation files in the Sudan Train Backend project.

## 📖 Navigation Guide

### Main Documentation Hub
- **[Documentation Home](./README.md)** - Start here for documentation overview

---

## 🚀 Deployment (3 documents)

Deploy and run the application in different environments.

| Document | Description |
|----------|-------------|
| [Docker Setup](./deployment/docker-setup.md) | Complete Docker and Docker Compose configuration guide |
| [Quickstart](./deployment/quickstart.md) | Get the application running in 5 minutes |
| [Deployment Guide](./deployment/deployment-guide.md) | Production deployment best practices |

**Quick Access**: [Deployment Folder](./deployment)

---

## ⚙️ Configuration (2 documents)

Configure the application for different environments.

| Document | Description |
|----------|-------------|
| [AppSettings Guide](./configuration/appsettings-guide.md) | Comprehensive guide to all configuration options |
| [Configuration Overview](./configuration/configuration.md) | Configuration hierarchy and best practices |

**Quick Access**: [Configuration Folder](./configuration)

---

## 🏗️ Architecture (3 documents)

Understand the system architecture and design patterns.

| Document | Description |
|----------|-------------|
| [Messaging API](./architecture/messaging-api.md) | Microservice for Email, SMS, and Push notifications |
| [Email Service](./architecture/email-service.md) | Email service implementation details |
| [Email Strategy](./architecture/email-strategy.md) | Direct, Queued, and Fallback sending strategies |

**Quick Access**: [Architecture Folder](./architecture)

---

## 💻 Development (4 documents)

Development guidelines, refactoring examples, and fixes.

| Document | Description |
|----------|-------------|
| [Register Handler Refactoring](./development/register-handler-refactoring.md) | Clean code refactoring example with metrics |
| [Localization Refactoring](./development/localization-refactoring.md) | Multi-language support implementation |
| [Property Name Fix](./development/property-name-fix.md) | Property naming convention fixes |
| [Missing Fields Fix](./development/missing-fields-fix.md) | Database schema and field fixes |

**Quick Access**: [Development Folder](./development)

---

## 🗄️ Database (1 document)

Database setup, migrations, and schema documentation.

| Document | Description |
|----------|-------------|
| [Database Setup](./database/database-setup.md) | SQL Server setup, migrations, and seeding |

**Quick Access**: [Database Folder](./database)

---

## 🌍 Localization (1 document)

Internationalization and multi-language support.

| Document | Description |
|----------|-------------|
| [Validator Localization](./localization/validator-localization.md) | Validation message localization guide |

**Quick Access**: [Localization Folder](./localization)

---

## 📊 Documentation Statistics

- **Total Documents**: 14 markdown files
- **Total Folders**: 6 domain-specific folders
- **README Files**: 7 (1 main + 6 folder READMEs)
- **Documentation Files**: 14 content documents

## 🎯 Quick References by Task

### I want to...

**Deploy the application**
→ [Docker Quickstart](./deployment/quickstart.md)

**Configure email settings**
→ [AppSettings Guide](./configuration/appsettings-guide.md) → Email Settings

**Understand the messaging system**
→ [Messaging API](./architecture/messaging-api.md)

**Learn clean code practices**
→ [Register Handler Refactoring](./development/register-handler-refactoring.md)

**Setup the database**
→ [Database Setup](./database/database-setup.md)

**Add multi-language support**
→ [Validator Localization](./localization/validator-localization.md)

**Configure Docker**
→ [Docker Setup](./deployment/docker-setup.md)

**Understand email strategies**
→ [Email Strategy](./architecture/email-strategy.md)

---

## 📝 Document Naming Convention

All documents follow **kebab-case** naming:
- ✅ `docker-setup.md`
- ✅ `email-strategy.md`
- ✅ `register-handler-refactoring.md`
- ❌ ~~`DockerSetup.md`~~
- ❌ ~~`Email_Strategy.md`~~

## 🔄 Keeping Documentation Updated

When adding new documentation:
1. Create the file in the appropriate domain folder
2. Use kebab-case naming
3. Add entry to folder's README.md
4. Add entry to main [docs/README.md](./README.md)
5. Update this index if it's a new category

## 🔍 Search Tips

To find documentation quickly:

```bash
# Search all docs for a keyword
grep -r "keyword" docs/

# List all markdown files
find docs/ -name "*.md"

# Search by category
ls docs/deployment/
ls docs/architecture/
```

---

**Last Updated**: December 11, 2025
**Documentation Version**: 1.0
