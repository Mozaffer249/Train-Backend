# Docker Setup for Sudan Train Backend

## 🐳 Quick Start

### Build and Run with Docker Compose

```bash
# Build and start all services
docker-compose up -d --build

# View logs
docker-compose logs -f train-api

# Stop all services
docker-compose down

# Remove everything including volumes
docker-compose down -v
```

### Access the Application

- **API**: http://localhost:8080
- **Swagger UI**: http://localhost:8080/swagger
- **SQL Server**: localhost:1433
  - Username: `sa`
  - Password: `YourStrong@Passw0rd`

> **Note**: Port 8080 is used instead of 5000 to avoid conflicts with macOS AirPlay Receiver.

## 🔨 Manual Docker Commands

### Build the Docker Image

```bash
docker build -t sudan-train-api:latest .
```

### Run the Container

```bash
# Run with environment variables
docker run -d \
  --name train-api \
  -p 8080:80 \
  -e ASPNETCORE_ENVIRONMENT=Development \
  -e ConnectionStrings__dbcontext="Server=host.docker.internal,1433;Database=TrainsDb;User Id=sa;Password=YourStrong@Passw0rd;TrustServerCertificate=True;" \
  sudan-train-api:latest
```

### View Logs

```bash
docker logs -f train-api
```

### Stop and Remove Container

```bash
docker stop train-api
docker rm train-api
```

## 🗄️ Database Migration

### Run migrations inside the container

```bash
# Access the container
docker exec -it train-api bash

# Run migrations
dotnet ef database update --project Sudan_Train.Infrastructure

# Exit container
exit
```

### Or run from host (if EF tools installed)

```bash
# Update connection string in appsettings.json to point to localhost:1433
dotnet ef database update --project Sudan_Train.Infrastructure --startup-project Sudan_Train
```

## 🔧 Configuration

### Environment Variables

You can override settings using environment variables:

```bash
docker run -d \
  --name train-api \
  -p 8080:80 \
  -e ASPNETCORE_ENVIRONMENT=Production \
  -e ConnectionStrings__dbcontext="Your-Connection-String" \
  -e jwtSettings__Secret="Your-JWT-Secret-Key" \
  -e emailSettings__FromEmail="your-email@example.com" \
  -e emailSettings__Password="your-password" \
  sudan-train-api:latest
```

### Custom appsettings for Production

Create `appsettings.Production.json` in Sudan_Train folder:

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

## 🚀 Deployment

### Push to Docker Hub

```bash
# Tag the image
docker tag sudan-train-api:latest yourusername/sudan-train-api:latest

# Login to Docker Hub
docker login

# Push the image
docker push yourusername/sudan-train-api:latest
```

### Pull and Run on Server

```bash
# Pull the image
docker pull yourusername/sudan-train-api:latest

# Run with docker-compose
docker-compose up -d
```

## 📝 Notes

- SQL Server requires at least **2GB of RAM** to run properly
- Change the default `SA_PASSWORD` in production
- Use Docker secrets or environment variables for sensitive data
- Enable HTTPS in production by mounting SSL certificates
- The `.dockerignore` file prevents unnecessary files from being copied into the image

## 🔍 Troubleshooting

### Container won't start

```bash
# Check logs
docker logs train-api

# Check if port is already in use (macOS)
lsof -i :8080

# Or on Linux
netstat -an | grep 8080
```

### Database connection fails

```bash
# Check if SQL Server is running
docker ps | grep sqlserver

# Test SQL Server connection (SQL Server 2022 uses mssql-tools18)
docker exec -it train-sqlserver /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "YourStrong@Passw0rd" -C
```

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

