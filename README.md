# 🚂 Sudan Train Platform

Full-stack train booking platform with .NET backend and React frontend.

## 📁 Project Structure

```
Sudan-Train-Platform/
├── apps/
│   ├── backend/                  # .NET 8 API and microservices
│   │   ├── Sudan_Train/          # Main API project
│   │   ├── Sudan_Train.Core/     # Business logic
│   │   ├── Sudan_Train.Data/     # Data entities
│   │   ├── Sudan_Train.Infrastructure/
│   │   ├── Sudan_Train.Service/
│   │   ├── Sudan_Train.MessagingApi/
│   │   ├── docs/                 # Backend documentation
│   │   ├── _Trains.sln
│   │   └── Dockerfile
│   │
│   └── frontend/                 # Web applications
│       ├── customer/             # Public-facing booking site
│       │   ├── src/
│       │   ├── public/
│       │   ├── package.json
│       │   └── Dockerfile
│       │
│       └── admin/                # Admin dashboard
│           ├── src/
│           ├── public/
│           ├── package.json
│           └── Dockerfile
│
├── docs/                         # Platform/deployment documentation
│   └── deployment/               # Docker, deployment guides
├── docker-compose.yml
└── README.md
```

## 🚀 Quick Start

### Prerequisites
- Docker & Docker Compose
- .NET 8 SDK (for backend development)
- Node.js 18+ (for frontend development)

### Run Everything with Docker (Recommended)

```bash
# Clone the repository
git clone <repository-url>
cd Sudan-Train-Platform

# Start all services
docker-compose up -d

# View logs
docker-compose logs -f
```

### Access the Applications

| Service | URL | Description |
|---------|-----|-------------|
| **Customer App** | http://localhost:3000 | Public-facing booking site |
| **Admin Dashboard** | http://localhost:3001 | Admin management portal |
| **Backend API** | http://localhost:8080/swagger | .NET API with Swagger docs |
| **Messaging API** | http://localhost:5001 | Email/SMS/Push microservice |
| **RabbitMQ** | http://localhost:15672 | Message queue management (guest/guest) |
| **SQL Server** | localhost:1433 | Database |

## 🛠️ Development

### Backend Development

```bash
cd apps/backend

# Restore dependencies
dotnet restore _Trains.sln

# Build the solution
dotnet build _Trains.sln

# Run the API
dotnet run --project Sudan_Train
```

See [Backend README](./apps/backend/README.md) for detailed instructions.

### Frontend Development

```bash
cd apps/frontend/customer

# Install dependencies
npm install

# Start development server (runs on port 5173)
npm run dev
```

See [Customer Frontend README](./apps/frontend/customer/README.md) for detailed instructions.

### Admin Dashboard Development

```bash
cd apps/frontend/admin

# Install dependencies
npm install

# Start development server (runs on port 3001)
npm run dev
```

See [Admin README](./apps/frontend/admin/README.md) for detailed instructions.

## 📚 Documentation

Documentation is organized by scope:

### Platform Documentation (`/docs`)

Shared documentation for deployment and Docker:

- 🚀 **[Deployment](./docs/deployment)** - Docker setup, quickstart, production deployment

### Backend Documentation (`/apps/backend/docs`)

Backend-specific documentation:

- 🔐 **[Authentication](./apps/backend/docs/authentication)** - Auth system, 2FA, security
- 🏗️ **[Architecture](./apps/backend/docs/architecture)** - System design, messaging API
- ⚙️ **[Configuration](./apps/backend/docs/configuration)** - AppSettings, environment
- 🗄️ **[Database](./apps/backend/docs/database)** - Schema, migrations
- 💻 **[Development](./apps/backend/docs/development)** - Implementation guides
- 🌍 **[Localization](./apps/backend/docs/localization)** - Multi-language support
- 🧪 **[Testing](./apps/backend/docs/testing)** - Postman, API testing

### Frontend Documentation

- [Frontend Overview](./apps/frontend/README.md) - Web applications overview
- [Customer App](./apps/frontend/customer/README.md) - Public booking site
- [Admin App](./apps/frontend/admin/README.md) - Admin dashboard

## 🌟 Features

### Backend (.NET 8)
- **RESTful API** - Clean Architecture with CQRS pattern
- **Microservices** - Standalone Messaging API for Email/SMS/Push notifications
- **Multi-Language Support** - English and Arabic localization
- **Authentication & Authorization** - JWT-based with role management
- **Message Queue** - RabbitMQ for async processing
- **Email Service** - Multiple sending strategies (Direct, Queued, Fallback)

### Frontend (React + Vite)
- **Modern UI** - React with TypeScript
- **Responsive Design** - Tailwind CSS styling
- **Internationalization** - Arabic and English support
- **API Integration** - Connected to backend services

### Admin Dashboard (React + Vite)
- **Management Portal** - Separate admin interface
- **Role-Based Access** - Admin/Staff authentication
- **User Management** - User CRUD operations
- **Booking Management** - View and manage bookings
- **Train & Trip Management** - Fleet and schedule management

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

# Firebase (Optional)
FIREBASE_KEY_PATH=
```

## 🧪 Testing

### Backend Tests

```bash
cd apps/backend
dotnet test
```

### Frontend Tests

```bash
# Customer app tests
cd apps/frontend/customer
npm test

# Admin app tests
cd apps/frontend/admin
npm test
```

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

### Development Guidelines
- Follow Clean Code principles
- Write tests for new features
- Update documentation as needed
- Use conventional commit messages

## 📝 License

[Add your license here]

## 👥 Authors

- **Muzafar Ragab** - Initial work

---

**Built with ❤️ using .NET 8 and React**
