# Train Backend - Docker Deployment Guide

This guide explains how to build and deploy the Train Backend application using Docker and Docker Compose.

## 📋 Table of Contents

- [Prerequisites](#prerequisites)
- [Quick Start](#quick-start)
- [Architecture Overview](#architecture-overview)
- [Port Configuration](#port-configuration)
- [Environment Variables](#environment-variables)
- [Common Commands](#common-commands)
- [Troubleshooting](#troubleshooting)
- [API Access](#api-access)

## 🔧 Prerequisites

Before you begin, ensure you have the following installed:

- **Docker Desktop** (v20.10 or higher)
- **Docker Compose** (v2.0 or higher)
- At least **4GB of RAM** available for Docker
- **Ports available**: 8080, 8443, 1433

## 🚀 Quick Start

### 1. Clone and Navigate to Project

```bash
cd /path/to/Train-Backend
```

### 2. Build and Start All Services

```bash
docker-compose up --build -d
```

This command will:
- Build the Train API Docker image
- Pull the SQL Server 2022 image
- Create a network for the services
- Start SQL Server first (with healthcheck)
- Start the API after SQL Server is healthy
- Run migrations and seed the database

### 3. Verify Deployment

```bash
docker-compose ps
```

You should see:
```
NAME              STATUS                    PORTS
train-api         Up                        0.0.0.0:8080->80/tcp, 0.0.0.0:8443->443/tcp
train-sqlserver   Up (healthy)              0.0.0.0:1433->1433/tcp
```

### 4. Access the API

Open your browser and navigate to:
- **Swagger UI**: http://localhost:8080/swagger
- **API Base**: http://localhost:8080

## 🏗️ Architecture Overview

### Multi-Stage Docker Build

The application uses a multi-stage Dockerfile for optimization:

```
┌─────────────────────────────────────────┐
│  Stage 1: Build (SDK 8.0)              │
│  - Restore NuGet packages               │
│  - Compile C# code                      │
│  - Publish release build                │
└─────────────────┬───────────────────────┘
                  │
                  ↓
┌─────────────────────────────────────────┐
│  Stage 2: Runtime (ASP.NET 8.0)        │
│  - Copy compiled binaries only          │
│  - Lightweight production image         │
└─────────────────────────────────────────┘
```

### Service Dependencies

```
┌─────────────────────────────────────────┐
│  train-api (ASP.NET Core 8.0)          │
│  Container: train-api                   │
│  Ports: 8080 (HTTP), 8443 (HTTPS)      │
│  Environment: Development               │
└─────────────────┬───────────────────────┘
                  │
                  │ Waits for healthy status
                  ↓
┌─────────────────────────────────────────┐
│  train-sqlserver (SQL Server 2022)     │
│  Container: train-sqlserver             │
│  Port: 1433                             │
│  Database: TrainsDb                     │
│  Healthcheck: Every 10s                 │
└─────────────────────────────────────────┘
```

### Networking

- **Network Name**: `train-backend_train-network`
- **Driver**: Bridge (isolated network)
- **DNS**: Containers can communicate using service names (e.g., `sqlserver`)

### Data Persistence

- **SQL Server Data**: Stored in Docker volume `sqlserver_data`
- **Application Logs**: Mounted from host `./Logs` directory
- Data persists even when containers are stopped

## 🔌 Port Configuration

| Service        | Container Port | Host Port | Protocol | Purpose                    |
|----------------|----------------|-----------|----------|----------------------------|
| train-api      | 80             | 8080      | HTTP     | API endpoints & Swagger    |
| train-api      | 443            | 8443      | HTTPS    | Secure API access          |
| train-sqlserver| 1433           | 1433      | TCP      | SQL Server connections     |

### Why Port 8080 Instead of 5000?

macOS uses port 5000 for AirPlay Receiver (Control Center). To avoid conflicts, we use port 8080.

**To change ports**, edit `docker-compose.yml`:

```yaml
train-api:
  ports:
    - "YOUR_PORT:80"      # Change YOUR_PORT to desired port
    - "YOUR_HTTPS:443"    # Change YOUR_HTTPS to desired HTTPS port
```

## 🌍 Environment Variables

The application uses environment variables for configuration. You can customize them in `.env` file or directly in `docker-compose.yml`.

### Default Values

| Variable                      | Default Value                                    | Description                    |
|-------------------------------|--------------------------------------------------|--------------------------------|
| `ASPNETCORE_ENVIRONMENT`      | `Development`                                    | Application environment        |
| `SQL_PASSWORD`                | `YourStrong@Passw0rd`                           | SQL Server SA password         |
| `JWT_SECRET`                  | `TrainProjectSecretKey...`                      | JWT signing key                |
| `EMAIL_FROM`                  | `your-email@gmail.com`                          | Email sender address           |
| `EMAIL_PASSWORD`              | `your-app-password`                             | Email app password             |

### Creating a .env File

Create a `.env` file in the project root:

```bash
# Database
SQL_PASSWORD=YourStrongPassword123!

# Application
ASPNETCORE_ENVIRONMENT=Production

# JWT
JWT_SECRET=YourVeryLongSecretKeyHere

# Email (optional)
EMAIL_FROM=noreply@yourdomain.com
EMAIL_PASSWORD=your-app-specific-password
```

**⚠️ Security Warning**: Never commit `.env` files with real credentials to version control!

## 📝 Common Commands

### Build & Start

```bash
# Build and start in detached mode
docker-compose up --build -d

# Build only (no start)
docker-compose build

# Start without rebuilding
docker-compose up -d

# Start with logs visible
docker-compose up
```

### View Logs

```bash
# View all logs
docker-compose logs

# Follow logs in real-time
docker-compose logs -f

# View specific service logs
docker logs train-api -f
docker logs train-sqlserver -f

# View last 100 lines
docker logs train-api --tail 100
```

### Stop & Remove

```bash
# Stop containers (keeps data)
docker-compose stop

# Stop and remove containers (keeps volumes)
docker-compose down

# Remove everything including volumes (⚠️ DATA LOSS)
docker-compose down -v

# Remove and rebuild from scratch
docker-compose down -v && docker-compose up --build -d
```

### Container Management

```bash
# View running containers
docker-compose ps

# Restart a service
docker-compose restart train-api

# Execute command in container
docker exec -it train-api bash

# Connect to SQL Server
docker exec -it train-sqlserver /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "YourStrong@Passw0rd" -C
```

### Health & Diagnostics

```bash
# Check health status
docker inspect train-sqlserver | grep -A 10 Health

# View resource usage
docker stats

# View networks
docker network ls

# View volumes
docker volume ls
```

## 🔍 Troubleshooting

### SQL Server Won't Start

**Problem**: Container shows as "unhealthy"

**Solution**:
```bash
# Check logs
docker logs train-sqlserver

# Verify healthcheck command works
docker exec train-sqlserver /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "YourStrong@Passw0rd" -Q "SELECT 1" -C

# Restart container
docker-compose restart train-sqlserver
```

### Port Already in Use

**Problem**: Error: `bind: address already in use`

**Solution**:
```bash
# Find what's using the port
lsof -i :8080

# Kill the process (replace PID)
kill -9 PID

# Or change port in docker-compose.yml
```

### API Can't Connect to Database

**Problem**: Connection refused or timeout errors

**Solution**:
```bash
# Ensure SQL Server is healthy
docker-compose ps

# Check network connectivity
docker exec train-api ping sqlserver

# Verify connection string
docker exec train-api env | grep ConnectionStrings
```

### Migration Errors

**Problem**: Database migration fails on startup

**Solution**:
```bash
# Reset database (⚠️ deletes all data)
docker-compose down -v
docker-compose up -d

# Or manually run migrations
docker exec train-api dotnet ef database update
```

### Out of Memory

**Problem**: Containers crash or won't start

**Solution**:
- Increase Docker Desktop memory allocation (Settings → Resources)
- Recommended: At least 4GB for both services

### SQL Server on ARM64/M1 Mac

**Warning**: SQL Server 2022 runs in emulation mode on Apple Silicon Macs (slower performance)

```bash
# You may see this warning (it's normal):
# "The requested image's platform (linux/amd64) does not match the detected host platform (linux/arm64/v8)"
```

## 🌐 API Access

### Swagger UI

Access interactive API documentation:
- **URL**: http://localhost:8080/swagger
- **Features**:
  - Browse all endpoints
  - Test API calls directly from browser
  - View request/response schemas
  - Generate example requests

### Direct API Calls

```bash
# Health check
curl http://localhost:8080/api/health

# Authentication example
curl -X POST http://localhost:8080/api/authentication/login \
  -H "Content-Type: application/json" \
  -d '{"username":"admin","password":"password"}'

# With authentication token
curl http://localhost:8080/api/users \
  -H "Authorization: Bearer YOUR_JWT_TOKEN"
```

### Database Access

Connect to SQL Server from your local machine:

**Using Azure Data Studio or SQL Server Management Studio:**
```
Server: localhost,1433
Authentication: SQL Server Authentication
Login: sa
Password: YourStrong@Passw0rd (or your SQL_PASSWORD)
Database: TrainsDb
Trust Server Certificate: Yes
```

**Using sqlcmd:**
```bash
docker exec -it train-sqlserver /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "YourStrong@Passw0rd" -C

# Once connected:
1> SELECT name FROM sys.databases;
2> GO
```

## 📊 Monitoring

### Container Health

```bash
# Real-time resource usage
docker stats train-api train-sqlserver

# Health check results
docker inspect train-sqlserver --format='{{json .State.Health}}' | jq
```

### Application Logs

Logs are stored in `./Logs` directory:
```bash
# View today's log
cat Logs/log-$(date +%Y%m%d).txt

# Follow log in real-time
tail -f Logs/log-$(date +%Y%m%d).txt
```

## 🔒 Security Best Practices

1. **Change Default Passwords**: Never use default SA password in production
2. **Use Environment Variables**: Store secrets in `.env` file (not in `docker-compose.yml`)
3. **Enable HTTPS**: Configure SSL certificates for production
4. **Restrict Ports**: Don't expose database port (1433) to public in production
5. **Use Secrets Management**: Consider Docker secrets or Azure Key Vault for production
6. **Regular Updates**: Keep base images updated for security patches

## 🚀 Production Deployment

For production deployment, consider:

1. **Use Production Configuration**:
   ```bash
   ASPNETCORE_ENVIRONMENT=Production docker-compose up -d
   ```

2. **Enable HTTPS with Real Certificates**
3. **Use External Database**: Don't run SQL Server in Docker for production
4. **Implement Logging**: Use centralized logging (ELK stack, Azure Monitor)
5. **Add Monitoring**: Use Prometheus, Grafana, or Application Insights
6. **Configure Reverse Proxy**: Use Nginx or Azure App Gateway
7. **Implement CI/CD**: Automate builds and deployments

## 📚 Additional Resources

- [Docker Documentation](https://docs.docker.com/)
- [ASP.NET Core in Docker](https://docs.microsoft.com/en-us/aspnet/core/host-and-deploy/docker/)
- [SQL Server on Docker](https://docs.microsoft.com/en-us/sql/linux/sql-server-linux-docker-container-deployment)

## 📄 Related Documentation

- [Configuration Guide](./CONFIGURATION.md)
- [AppSettings Guide](./APPSETTINGS-GUIDE.md)
- [Database Guide](./README.Database.md)
- [Docker Setup](./README.Docker.md)

---

**Last Updated**: December 6, 2025  
**Maintainer**: Train Backend Team

