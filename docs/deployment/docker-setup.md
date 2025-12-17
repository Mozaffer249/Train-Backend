# Docker Setup for Sudan Train Platform

This guide covers Docker setup for the full-stack monorepo including backend and frontend services.

## 🐳 Quick Start

### Build and Run with Docker Compose

```bash
# Navigate to project root
cd Sudan-Train-Platform

# Build and start all services
docker-compose up -d --build

# View logs
docker-compose logs -f

# Stop all services
docker-compose down

# Remove everything including volumes
docker-compose down -v
```

### Access the Applications

| Service | URL | Description |
|---------|-----|-------------|
| **Frontend** | http://localhost:3000 | React web application |
| **Backend API** | http://localhost:8080/swagger | .NET API with Swagger docs |
| **Messaging API** | http://localhost:5001 | Email/SMS/Push microservice |
| **RabbitMQ** | http://localhost:15672 | Message queue (guest/guest) |
| **SQL Server** | localhost:1433 | Database (sa/YourStrong@Passw0rd) |

> **Note**: Port 8080 is used instead of 5000 to avoid conflicts with macOS AirPlay Receiver.

## 🏗️ Monorepo Structure

```
Sudan-Train-Platform/
├── apps/
│   ├── backend/              # .NET 8 API
│   │   ├── Dockerfile        # Backend Docker config
│   │   └── Sudan_Train.MessagingApi/
│   │       └── Dockerfile    # Messaging API Docker config
│   └── frontend/             # React app
│       └── Dockerfile        # Frontend Docker config
├── docker-compose.yml        # Orchestrates all services
└── docs/                     # Documentation
```

## 🔨 Manual Docker Commands

### Build Individual Services

```bash
# Build backend API
docker build -t sudan-train-api:latest ./apps/backend

# Build messaging API
docker build -t sudan-messaging-api:latest -f ./apps/backend/Sudan_Train.MessagingApi/Dockerfile ./apps/backend

# Build frontend
docker build -t sudan-train-frontend:latest ./apps/frontend
``` .      

### Run Backend Container Manually

```bash
docker run -d \
  --name train-api \
  -p 8080:80 \
  -e ASPNETCORE_ENVIRONMENT=Development \
  -e ConnectionStrings__dbcontext="Server=host.docker.internal,1433;Database=TrainsDb;User Id=sa;Password=YourStrong@Passw0rd;TrustServerCertificate=True;" \
  sudan-train-api:latest
```

### View Logs

```bash
# All services
docker-compose logs -f

# Individual services
docker logs -f train-api
docker logs -f train-frontend
docker logs -f train-messaging-api
```

### Stop and Remove Containers

```bash
docker stop train-api train-frontend train-messaging-api
docker rm train-api train-frontend train-messaging-api
```

## 🗄️ Database Migration

### Run migrations inside the container

```bash
# Access the container
docker exec -it train-api bash

# Navigate to backend directory
cd /app

# Run migrations (if EF tools are installed)
dotnet ef database update --project Sudan_Train.Infrastructure

# Exit container
exit
```

### Or run from host

```bash
# Navigate to backend directory
cd apps/backend

# Update connection string in appsettings.json to point to localhost:1433
dotnet ef database update --project Sudan_Train.Infrastructure --startup-project Sudan_Train
```

## 🔧 Configuration

### Environment Variables

Override settings using environment variables in `docker-compose.yml` or a `.env` file:

```bash
# Create .env file in project root
cat > .env << EOF
# Database
SQL_PASSWORD=YourStrong@Passw0rd

# JWT
JWT_SECRET=YourSecretKeyHere

# Email
EMAIL_FROM=your-email@gmail.com
EMAIL_PASSWORD=your-app-password

# Twilio (Optional)
TWILIO_ACCOUNT_SID=
TWILIO_AUTH_TOKEN=
TWILIO_FROM_NUMBER=
EOF
```

### Custom appsettings for Production

Create `appsettings.Production.json` in `apps/backend/Sudan_Train/`:

```json
{
  "ConnectionStrings": {
    "dbcontext": "Server=sqlserver,1433;Database=TrainsDb;User Id=sa;Password=YourStrong@Passw0rd;TrustServerCertificate=True;"
  },
  "jwtSettings": {
    "Secret": "YOUR-PRODUCTION-SECRET-KEY-MUST-BE-VERY-LONG-AND-SECURE",
    "Issuer": "TrainProject",
    "Audience": "TrainProjectUsers"
  }
}
```

### Frontend Environment

Create `.env.local` in `apps/frontend/`:

```env
VITE_API_URL=http://localhost:8080
VITE_MESSAGING_API_URL=http://localhost:5001
```

## 🚀 Deployment

### Push to Docker Hub

```bash
# Tag the images
docker tag sudan-train-api:latest yourusername/sudan-train-api:latest
docker tag sudan-train-frontend:latest yourusername/sudan-train-frontend:latest

# Login to Docker Hub
docker login

# Push the images
docker push yourusername/sudan-train-api:latest
docker push yourusername/sudan-train-frontend:latest
```

### Pull and Run on Server

```bash
# Pull the images
docker pull yourusername/sudan-train-api:latest
docker pull yourusername/sudan-train-frontend:latest

# Run with docker-compose
docker-compose up -d
```

## 📝 Notes

- SQL Server requires at least **2GB of RAM** to run properly
- Change the default `SA_PASSWORD` in production
- Use Docker secrets or environment variables for sensitive data
- Enable HTTPS in production by mounting SSL certificates
- The `.dockerignore` files in each app prevent unnecessary files from being copied

## 🔍 Troubleshooting

### Container won't start

```bash
# Check logs
docker logs train-api
docker logs train-frontend

# Check if port is already in use (macOS)
lsof -i :8080
lsof -i :3000

# Or on Linux
netstat -an | grep -E "8080|3000"
```

### Database connection fails

```bash
# Check if SQL Server is running
docker ps | grep sqlserver

# Test SQL Server connection
docker exec -it train-sqlserver /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "YourStrong@Passw0rd" -C
```

### Frontend can't connect to API

- Check CORS settings in backend allow frontend origin
- Ensure `VITE_API_URL` environment variable is set correctly
- Verify both containers are on the same Docker network

### Cannot connect to SQL Server from API

- Make sure both containers are on the same network
- Use the service name `sqlserver` instead of `localhost` in connection string
- Wait for SQL Server to fully start (can take 10-15 seconds)

### Clean rebuild

```bash
# Remove all containers, images, and volumes
docker-compose down -v
docker system prune -a

# Rebuild from scratch
docker-compose up -d --build
```

## 🔗 Related Documentation

- [Quickstart Guide](./quickstart.md) - Quick commands reference
- [Deployment Guide](./deployment-guide.md) - Production deployment
- [Configuration Guide](../../apps/backend/docs/configuration/appsettings-guide.md) - All configuration options
- [Backend README](../../apps/backend/README.md) - Backend setup
- [Frontend README](../../apps/frontend/README.md) - Frontend setup
