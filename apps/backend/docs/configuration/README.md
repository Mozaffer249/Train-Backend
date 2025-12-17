# Configuration Documentation

Application configuration and settings documentation.

## 📄 Documents

### [AppSettings Guide](./appsettings-guide.md)
Comprehensive guide for application settings:
- Connection strings
- JWT configuration
- Email settings
- SMS settings
- Push notification settings
- Localization settings
- Logging configuration

### [Configuration Overview](./configuration.md)
General configuration guidelines:
- Configuration hierarchy
- Environment-specific settings
- Secrets management
- Best practices

## ⚙️ Configuration Files

The application uses the following configuration files:
- `appsettings.json` - Base configuration
- `appsettings.Development.json` - Development overrides
- `appsettings.Production.json` - Production overrides
- `docker-compose.yml` - Container environment variables
- `.env` - Local environment variables (gitignored)

## 🔐 Secrets Management

**Never commit sensitive data!**
- Use environment variables for secrets
- Use `.env` file for local development
- Use Azure Key Vault or similar for production

## 🔗 Related Documentation

- [Docker Setup](../deployment/docker-setup.md) - Container configuration
- [Deployment Guide](../deployment/deployment-guide.md) - Production configuration
