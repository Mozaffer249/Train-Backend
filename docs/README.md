# Sudan Train Platform - Documentation

Platform-level documentation for the Sudan Train monorepo.

## 📁 Documentation Structure

```
Sudan-Train-Platform/
├── docs/                         # Platform/shared documentation (you are here)
│   └── deployment/               # Docker, deployment guides
│
├── apps/
│   ├── backend/docs/             # Backend-specific documentation
│   │   ├── architecture/         # System design, messaging, email
│   │   ├── authentication/       # Auth system, 2FA, security
│   │   ├── configuration/        # AppSettings, environment
│   │   ├── database/             # Schema, migrations
│   │   ├── development/          # Implementation guides
│   │   ├── localization/         # Multi-language support
│   │   └── testing/              # API testing, Postman
│   │
│   └── frontend/                 # Frontend documentation in app folder
│       └── README.md
```

## 🚀 Deployment Documentation

Everything you need to deploy and run the platform.

| Document | Description |
|----------|-------------|
| [Docker Setup](./deployment/docker-setup.md) | Complete Docker configuration |
| [Quickstart Guide](./deployment/quickstart.md) | Get up and running quickly |
| [Deployment Guide](./deployment/deployment-guide.md) | Production deployment |
| [Localhost Setup](./deployment/localhost-setup-guide.md) | Local development setup |
| [Localhost vs Production](./deployment/localhost-vs-production-url.md) | URL configuration |

## 📚 App-Specific Documentation

### Backend (.NET 8)

Backend documentation is located in [`apps/backend/docs/`](../apps/backend/docs/):

- 🔐 **[Authentication](../apps/backend/docs/authentication)** - JWT, 2FA, security
- 🏗️ **[Architecture](../apps/backend/docs/architecture)** - Messaging API, email service
- ⚙️ **[Configuration](../apps/backend/docs/configuration)** - AppSettings guide
- 🗄️ **[Database](../apps/backend/docs/database)** - Schema, migrations
- 💻 **[Development](../apps/backend/docs/development)** - Implementation guides
- 🌍 **[Localization](../apps/backend/docs/localization)** - Multi-language support
- 🧪 **[Testing](../apps/backend/docs/testing)** - Postman, API testing

### Frontend (React)

Frontend documentation is in [`apps/frontend/README.md`](../apps/frontend/README.md).

## 🎯 Quick Links

### Getting Started

1. [Docker Quickstart](./deployment/quickstart.md) - Fastest way to run everything
2. [Backend Setup](../apps/backend/README.md) - Backend development
3. [Frontend Setup](../apps/frontend/README.md) - Frontend development
4. [Migration Guide](../MIGRATION_GUIDE.md) - Moving apps into the monorepo

### Common Tasks

| Task | Guide |
|------|-------|
| Run with Docker | [Quickstart](./deployment/quickstart.md) |
| Configure Backend | [AppSettings Guide](../apps/backend/docs/configuration/appsettings-guide.md) |
| Setup Database | [Database Setup](../apps/backend/docs/database/database-setup.md) |
| Test APIs | [Postman Guide](../apps/backend/docs/testing/postman-testing-guide.md) |
| Deploy to Production | [Deployment Guide](./deployment/deployment-guide.md) |

## 🔗 Related Files

| File | Location | Description |
|------|----------|-------------|
| Root README | [`../README.md`](../README.md) | Monorepo overview |
| Backend README | [`../apps/backend/README.md`](../apps/backend/README.md) | Backend setup |
| Frontend README | [`../apps/frontend/README.md`](../apps/frontend/README.md) | Frontend setup |
| Docker Compose | [`../docker-compose.yml`](../docker-compose.yml) | Service orchestration |
| Migration Guide | [`../MIGRATION_GUIDE.md`](../MIGRATION_GUIDE.md) | App migration |
