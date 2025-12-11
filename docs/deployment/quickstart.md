# 🚀 Docker Quick Start Guide

One-page reference for common Docker operations.

## ⚡ Quick Commands

### Start Everything

```bash
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

# API only
docker logs train-api -f

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

- **Swagger UI**: http://localhost:8080/swagger
- **API Base**: http://localhost:8080
- **SQL Server**: localhost:1433

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

### Restart a Service

```bash
docker-compose restart train-api
docker-compose restart train-sqlserver
```

### View Environment Variables

```bash
docker exec train-api env | grep -E "ASPNETCORE|ConnectionStrings|jwt"
```

## ⚠️ Troubleshooting

### Port Already in Use

```bash
# Find what's using port 8080
lsof -i :8080

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
docker-compose up --build -d
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
  
- **API**: Check seeded users in logs

## 🔗 Related Docs

- Full Guide: [DEPLOYMENT-GUIDE.md](./DEPLOYMENT-GUIDE.md)
- Docker Details: [README.Docker.md](./README.Docker.md)
- Configuration: [CONFIGURATION.md](./CONFIGURATION.md)

---

**Quick Tip**: Keep this file open in a terminal while developing! 🎯

