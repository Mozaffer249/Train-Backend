# Deployment Documentation

This folder contains all deployment-related documentation for the Sudan Train Backend.

## 📄 Documents

### [Docker Setup](./docker-setup.md)
Complete guide for Docker configuration including:
- Docker Compose setup
- Container configuration
- Multi-container orchestration
- Environment variables
- Volume management
- Network configuration

### [Quickstart Guide](./quickstart.md)
Get the application running quickly:
- Prerequisites
- Quick setup commands
- Common issues and solutions
- First-time setup checklist

### [Deployment Guide](./deployment-guide.md)
Production deployment instructions:
- Production environment setup
- Security configurations
- Performance tuning
- Monitoring setup
- Backup and recovery

### [Localhost Setup Guide](./localhost-setup-guide.md)
Local development environment setup:
- Frontend URL configuration
- Email confirmation links
- Environment-specific settings

### [Localhost vs Production URL](./localhost-vs-production-url.md)
URL configuration for different environments:
- Development URLs
- Staging configuration
- Production settings

### [Production Email Template](./production-email-template-update.md)
Production email template configuration:
- Email template updates
- Branding customization

## 🚀 Quick Commands

```bash
# Start all services
docker-compose up -d

# View logs
docker-compose logs -f

# Stop all services
docker-compose down

# Rebuild and restart
docker-compose up -d --build
```

## 🔗 Related Documentation

- [Configuration Guide](../configuration/appsettings-guide.md) - Environment configuration
- [Database Setup](../database/database-setup.md) - Database initialization
