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
docker logs sudan-train-backend-api -f

# Customer app only
docker logs sudan-train-customer -f

# Admin dashboard only
docker logs sudan-train-admin -f

# Messaging API only
docker logs sudan-train-messaging-api -f

# SQL Server only
docker logs sudan-train-db -f
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
| **Customer App** | http://localhost:3000 |
| **Admin Dashboard** | <http://localhost:3001> |
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
docker exec -it sudan-train-backend-api bash
```

### Access Customer App Container Shell

```bash
docker exec -it sudan-train-customer sh
```

### Access Admin Container Shell

```bash
docker exec -it sudan-train-admin sh
```

### Restart a Service

```bash
docker-compose restart backend-api
docker-compose restart customer
docker-compose restart admin
docker-compose restart messaging-api
```

### View Environment Variables

```bash
# Backend
docker exec sudan-train-backend-api env | grep -E "ASPNETCORE|ConnectionStrings|jwt"

# Customer app
docker exec sudan-train-customer env | grep VITE

# Admin
docker exec sudan-train-admin env | grep VITE
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
cd apps/frontend/customer
npm install
npm run dev
```

### Admin Development (without Docker)

```bash
cd apps/frontend/admin
npm install
npm run dev
```

### Build Individual Images

```bash
# Backend
docker build -t sudan-train-backend-api:dev ./apps/backend

# Customer app
docker build -t sudan-train-customer:dev ./apps/frontend/customer

# Admin
docker build -t sudan-train-admin:dev ./apps/admin
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
docker logs sudan-train-backend-api --tail 100

# Ensure SQL is healthy first
docker ps

# Rebuild
docker-compose up --build -d backend-api
```

### Customer App Won't Start

```bash
# Check logs
docker logs sudan-train-customer --tail 100

# Rebuild
docker-compose up --build -d customer
```

### Admin Won't Start

```bash
# Check logs
docker logs sudan-train-admin --tail 100

# Rebuild
docker-compose up --build -d admin
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

- **Admin Access**: Admin/Staff roles required

## 🔗 Related Docs

- Full Guide: [docker-setup.md](./docker-setup.md)
- Deployment: [deployment-guide.md](./deployment-guide.md)
- Configuration: [appsettings-guide.md](../../apps/backend/docs/configuration/appsettings-guide.md)
- Backend README: [apps/backend/README.md](../../apps/backend/README.md)
- Frontend README: [apps/frontend/README.md](../../apps/frontend/README.md)

---

**Quick Tip**: Keep this file open in a terminal while developing! 🎯
