# 🚀 Docker Quick Start Guide

One-page reference for common Docker operations in the Sudan Train Platform monorepo.

## ⚡ Quick Commands

### Start Everything

```bash
# From project root
docker-compose up -d
```

### Stop Everything

```bash
docker-compose down
```

### View Logs

```bash
# All services
docker-compose logs -f

# Backend API only
docker logs train-api -f

# Frontend only
docker logs train-frontend -f

# Messaging API only
docker logs train-messaging-api -f

# SQL Server only
docker logs train-sqlserver -f
```

### Rebuild & Restart

```bash
docker-compose up --build -d
```

### Fresh Start (Deletes Database!)

```bash
docker-compose down -v && docker-compose up --build -d
```

## 🌐 Access URLs

| Service | URL |
|---------|-----|
| **Frontend** | http://localhost:3000 |
| **Swagger UI** | http://localhost:8080/swagger |
| **API Base** | http://localhost:8080 |
| **Messaging API** | http://localhost:5001 |
| **RabbitMQ UI** | http://localhost:15672 |
| **SQL Server** | localhost:1433 |

## 📊 Check Status

```bash
# List running containers
docker-compose ps

# Check container health
docker ps

# View resource usage
docker stats
```

## 🔧 Common Tasks

### Access SQL Server

```bash
docker exec -it train-sqlserver /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "YourStrong@Passw0rd" -C
```

### Access API Container Shell

```bash
docker exec -it train-api bash
```

### Access Frontend Container Shell

```bash
docker exec -it train-frontend sh
```

### Restart a Service

```bash
docker-compose restart train-api
docker-compose restart train-frontend
docker-compose restart train-sqlserver
docker-compose restart messaging-api
```

### View Environment Variables

```bash
# Backend
docker exec train-api env | grep -E "ASPNETCORE|ConnectionStrings|jwt"

# Frontend
docker exec train-frontend env | grep VITE
```

## 🏗️ Development Workflow

### Backend Development (without Docker)

```bash
cd apps/backend
dotnet restore _Trains.sln
dotnet run --project Sudan_Train
```

### Frontend Development (without Docker)

```bash
cd apps/frontend
npm install
npm run dev
```

### Build Individual Images

```bash
# Backend
docker build -t train-api:dev ./apps/backend

# Frontend
docker build -t train-frontend:dev ./apps/frontend
```

## ⚠️ Troubleshooting

### Port Already in Use

```bash
# Find what's using the port
lsof -i :8080
lsof -i :3000

# Kill the process
kill -9 <PID>
```

### SQL Server Unhealthy

```bash
# Check logs
docker logs train-sqlserver --tail 50

# Restart
docker-compose restart train-sqlserver
```

### API Won't Start

```bash
# Check logs
docker logs train-api --tail 100

# Ensure SQL is healthy first
docker ps

# Rebuild
docker-compose up --build -d train-api
```

### Frontend Won't Start

```bash
# Check logs
docker logs train-frontend --tail 100

# Rebuild
docker-compose up --build -d frontend
```

### Complete Reset

```bash
# WARNING: Deletes all data!
docker-compose down -v
docker system prune -f
docker-compose up --build -d
```

## 📝 Default Credentials

- **SQL Server**:
  - Username: `sa`
  - Password: `YourStrong@Passw0rd`
  
- **RabbitMQ**:
  - Username: `guest`
  - Password: `guest`

- **API**: Check seeded users in logs

## 🔗 Related Docs

- Full Guide: [docker-setup.md](./docker-setup.md)
- Deployment: [deployment-guide.md](./deployment-guide.md)
- Configuration: [appsettings-guide.md](../../apps/backend/docs/configuration/appsettings-guide.md)
- Backend README: [apps/backend/README.md](../../apps/backend/README.md)
- Frontend README: [apps/frontend/README.md](../../apps/frontend/README.md)

---

**Quick Tip**: Keep this file open in a terminal while developing! 🎯
