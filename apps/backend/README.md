# 🚂 Sudan Train Backend

A comprehensive train booking system backend built with .NET 8, featuring microservices architecture, multi-language support, and real-time messaging capabilities.

## 🌟 Features

- **RESTful API** - Clean Architecture with CQRS pattern
- **Microservices** - Standalone Messaging API for Email/SMS/Push notifications
- **Multi-Language Support** - English and Arabic localization
- **Authentication & Authorization** - JWT-based with role management
- **Message Queue** - RabbitMQ for async processing
- **Email Service** - Multiple sending strategies (Direct, Queued, Fallback)
- **Docker Support** - Containerized deployment with Docker Compose
- **Clean Code** - SOLID principles and best practices

## 🚀 Quick Start

### Prerequisites
- Docker & Docker Compose
- .NET 8 SDK (for local development)
- SQL Server 2022 or Azure SQL Edge

### Run with Docker (Recommended)

```bash
# From the monorepo root
cd ../..
docker-compose up -d

# View logs
docker-compose logs -f train-api

# Access the API
open http://localhost:8080/swagger
```

### Run Locally

```bash
# From this directory (apps/backend)
dotnet restore _Trains.sln
dotnet run --project Sudan_Train
```

### Services URLs
- **Main API**: http://localhost:8080/swagger
- **Messaging API**: http://localhost:5001
- **RabbitMQ Management**: http://localhost:15672 (guest/guest)
- **SQL Server**: localhost:1433

## 📚 Documentation

Backend documentation is organized by domain in the [`docs/`](./docs) folder:

### 🔐 [Authentication](./docs/authentication)
- [Module 1: Core Security](./docs/authentication/module-1-core-security-fixes.md) - Account lockout, email confirmation
- [Module 2: Two-Factor Auth](./docs/authentication/module-2-two-factor-authentication.md) - TOTP-based 2FA
- [Authentication Status](./docs/authentication/authentication-final-status.md) - Implementation status

### 🏗️ [Architecture](./docs/architecture)
- [Messaging API](./docs/architecture/messaging-api.md) - Microservice documentation
- [Email Service](./docs/architecture/email-service.md) - Email implementation
- [Email Strategy](./docs/architecture/email-strategy.md) - Sending strategies

### ⚙️ [Configuration](./docs/configuration)
- [AppSettings Guide](./docs/configuration/appsettings-guide.md) - All configuration options
- [Configuration Overview](./docs/configuration/configuration.md) - Best practices

### 🗄️ [Database](./docs/database)
- [Database Setup](./docs/database/database-setup.md) - Schema and migrations
- [Migration Guide](./docs/database/migration-guide.md) - Running migrations

### 💻 [Development](./docs/development)
- [Clean Code Refactoring](./docs/development/register-handler-refactoring.md) - Best practices
- [Localization Guide](./docs/development/localization-refactoring.md) - i18n implementation

### 🌍 [Localization](./docs/localization)
- [Validator Localization](./docs/localization/validator-localization.md) - Multi-language support

### 🧪 [Testing](./docs/testing)
- [Postman Testing Guide](./docs/testing/postman-testing-guide.md) - API testing

## 🛠️ Tech Stack

### Backend
- **.NET 8** - Latest .NET framework
- **ASP.NET Core** - Web API framework
- **Entity Framework Core** - ORM for database access
- **MediatR** - CQRS pattern implementation
- **FluentValidation** - Input validation
- **Serilog** - Structured logging

### Infrastructure
- **SQL Server 2022** - Primary database
- **RabbitMQ** - Message queue
- **Docker & Docker Compose** - Containerization
- **MailKit** - Email sending
- **Twilio** - SMS service
- **Firebase** - Push notifications

## 📁 Project Structure

```
apps/backend/
├── Sudan_Train/              # Main API project
├── Sudan_Train.Core/         # Business logic & MediatR handlers
├── Sudan_Train.Data/         # Data entities
├── Sudan_Train.Infrastructure/ # Infrastructure layer
├── Sudan_Train.Service/      # Service layer
├── Sudan_Train.MessagingApi/ # Messaging microservice
├── docs/                     # Backend documentation
├── _Trains.sln               # Solution file
└── Dockerfile                # Docker configuration
```

## 🔐 Environment Variables

Create a `.env` file in the monorepo root:

```env
# Database
SQL_PASSWORD=YourStrong@Passw0rd

# JWT
JWT_SECRET=YourSecretKeyHere

# Email (Gmail)
EMAIL_FROM=your-email@gmail.com
EMAIL_PASSWORD=your-app-password

# Twilio (Optional)
TWILIO_ACCOUNT_SID=
TWILIO_AUTH_TOKEN=
TWILIO_FROM_NUMBER=
```

## 🧪 Testing

```bash
# Run unit tests
dotnet test

# Run with coverage
dotnet test /p:CollectCoverage=true
```

## 📊 API Documentation

Once running, access the interactive API documentation:
- **Swagger UI**: http://localhost:8080/swagger
- **Messaging API Docs**: http://localhost:5001

## 🔗 Related Documentation

| Location | Description |
|----------|-------------|
| [Platform Docs](../../docs) | Deployment, Docker setup |
| [Frontend](../frontend/README.md) | React application |
| [Migration Guide](../../MIGRATION_GUIDE.md) | App migration |

---

**Built with ❤️ using .NET 8**
