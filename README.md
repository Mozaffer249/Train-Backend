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
# Clone the repository
git clone <repository-url>
cd Train-Backend

# Start all services
docker-compose up -d

# View logs
docker-compose logs -f train-api

# Access the API
open http://localhost:8080/swagger
```

### Services URLs
- **Main API**: http://localhost:8080/swagger
- **Messaging API**: http://localhost:5001
- **RabbitMQ Management**: http://localhost:15672 (guest/guest)
- **SQL Server**: localhost:1433

## 📚 Documentation

Comprehensive documentation is organized by domain in the [`/docs`](./docs) folder:

### 🚀 [Deployment](./docs/deployment)
- [Docker Setup](./docs/deployment/docker-setup.md) - Complete Docker guide
- [Quickstart](./docs/deployment/quickstart.md) - Get running in 5 minutes
- [Deployment Guide](./docs/deployment/deployment-guide.md) - Production deployment

### ⚙️ [Configuration](./docs/configuration)
- [AppSettings Guide](./docs/configuration/appsettings-guide.md) - All configuration options
- [Configuration Overview](./docs/configuration/configuration.md) - Configuration best practices

### 🏗️ [Architecture](./docs/architecture)
- [Messaging API](./docs/architecture/messaging-api.md) - Microservice documentation
- [Email Service](./docs/architecture/email-service.md) - Email implementation
- [Email Strategy](./docs/architecture/email-strategy.md) - Sending strategies

### 💻 [Development](./docs/development)
- [Clean Code Refactoring](./docs/development/register-handler-refactoring.md) - Best practices
- [Localization Guide](./docs/development/localization-refactoring.md) - i18n implementation
- [Fix Summaries](./docs/development) - Common fixes and solutions

### 🗄️ [Database](./docs/database)
- [Database Setup](./docs/database/database-setup.md) - Schema and migrations

### 🌍 [Localization](./docs/localization)
- [Validator Localization](./docs/localization/validator-localization.md) - Multi-language support

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
Train-Backend/
├── Sudan_Train/              # Main API project
├── Sudan_Train.Core/         # Business logic & MediatR handlers
├── Sudan_Train.Data/         # Data entities
├── Sudan_Train.Infrastructure/ # Infrastructure layer
├── Sudan_Train.Service/      # Service layer
├── Sudan_Train.MessagingApi/ # Messaging microservice
├── docs/                     # Documentation
└── docker-compose.yml        # Docker orchestration
```

## 🔐 Environment Variables

Create a `.env` file in the root directory:

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

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

### Development Guidelines
- Follow [Clean Code Principles](./docs/development/register-handler-refactoring.md)
- Write unit tests for new features
- Update documentation as needed
- Use conventional commit messages

## 📝 License

[Add your license here]

## 👥 Authors

- **Muzafar Ragab** - Initial work

## 🙏 Acknowledgments

- .NET Team for the amazing framework
- Community contributors
- Open source libraries used in this project

## 📞 Support

For issues and questions:
- 📖 Check the [Documentation](./docs)
- 🐛 Report bugs via [Issues](../../issues)
- 💬 Ask questions in [Discussions](../../discussions)

---

**Built with ❤️ using .NET 8**
