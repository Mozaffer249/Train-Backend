# 🔧 Configuration Guide - appsettings.json

## 📋 Overview

Your `appsettings.json` contains several settings that **MUST** be updated based on your environment.

---

## 🚨 **REQUIRED CHANGES**

### 1. **Connection String** ⚠️ CRITICAL

**Current (Local Development)**:
```json
"ConnectionStrings": {
  "dbcontext": "Server=DESKTOP-OHORTEL;Database=TrainsDb;Trusted_Connection=True;TrustServerCertificate=True;"
}
```

#### ✅ For Local Development (Windows)
```json
"ConnectionStrings": {
  "dbcontext": "Server=localhost;Database=TrainsDb;Trusted_Connection=True;TrustServerCertificate=True;"
}
```
Or with SQL Server name:
```json
"dbcontext": "Server=.\\SQLEXPRESS;Database=TrainsDb;Trusted_Connection=True;TrustServerCertificate=True;"
```

#### ✅ For Docker
```json
"ConnectionStrings": {
  "dbcontext": "Server=sqlserver,1433;Database=TrainsDb;User Id=sa;Password=YourStrong@Passw0rd;TrustServerCertificate=True;"
}
```
> **Note**: `sqlserver` is the service name in `docker-compose.yml`

#### ✅ For Production (Azure SQL / Remote Server)
```json
"ConnectionStrings": {
  "dbcontext": "Server=your-server.database.windows.net;Database=TrainsDb;User Id=yourusername;Password=YourSecurePassword;Encrypt=True;TrustServerCertificate=False;"
}
```

---

### 2. **Email Settings** 📧 REQUIRED FOR FEATURES

**Current (Placeholder)**:
```json
"emailSettings": {
  "Host": "smtp.gmail.com",
  "Port": 587,
  "FromEmail": "your-email@gmail.com",
  "Password": "your-app-password"
}
```

#### ✅ Gmail Configuration

1. **Enable 2-Factor Authentication** on your Gmail account
2. **Generate App Password**: 
   - Go to: https://myaccount.google.com/apppasswords
   - Create app password for "Mail"
3. **Update settings**:

```json
"emailSettings": {
  "Host": "smtp.gmail.com",
  "Port": 587,
  "FromEmail": "your-actual-email@gmail.com",
  "Password": "xxxx xxxx xxxx xxxx"  // App password (16 chars)
}
```

#### ✅ Other Email Providers

**Microsoft 365**:
```json
"emailSettings": {
  "Host": "smtp.office365.com",
  "Port": 587,
  "FromEmail": "noreply@yourcompany.com",
  "Password": "your-password"
}
```

**SendGrid**:
```json
"emailSettings": {
  "Host": "smtp.sendgrid.net",
  "Port": 587,
  "FromEmail": "noreply@yourcompany.com",
  "Password": "your-sendgrid-api-key"
}
```

**Mailgun**:
```json
"emailSettings": {
  "Host": "smtp.mailgun.org",
  "Port": 587,
  "FromEmail": "noreply@yourcompany.com",
  "Password": "your-mailgun-password"
}
```

---

### 3. **JWT Secret** 🔐 CRITICAL FOR PRODUCTION

**Current (Development)**:
```json
"jwtSettings": {
  "Secret": "TrainProjectSecretKey123456789TrainProjectSecretKey123456789"
}
```

⚠️ **This is OK for development, but MUST change for production!**

#### ✅ Production Secret

**Generate a secure random secret**:

```bash
# Option 1: Using PowerShell
[Convert]::ToBase64String((1..64|ForEach-Object{Get-Random -Maximum 256}))

# Option 2: Using OpenSSL
openssl rand -base64 64

# Option 3: Using Node.js
node -e "console.log(require('crypto').randomBytes(64).toString('base64'))"
```

**Update**:
```json
"jwtSettings": {
  "Secret": "YOUR_GENERATED_64_CHARACTER_RANDOM_SECRET_HERE",
  "Issuer": "TrainProject",
  "Audience": "TrainProjectUsers",
  "AccessTokenExpireDate": 60,      // 60 minutes for production
  "RefreshTokenExpireDate": 43200   // 30 days
}
```

**Production Recommendations**:
```json
"jwtSettings": {
  "Secret": "USE_ENVIRONMENT_VARIABLE",  // Store in env var
  "AccessTokenExpireDate": 30,           // 30 min (more secure)
  "RefreshTokenExpireDate": 10080        // 7 days
}
```

---

## 🔒 **SECURITY BEST PRACTICES**

### Use Environment Variables for Secrets

**Instead of hardcoding in appsettings.json**:

#### Option 1: User Secrets (Development)
```bash
# Initialize user secrets
dotnet user-secrets init --project Sudan_Train

# Add secrets
dotnet user-secrets set "ConnectionStrings:dbcontext" "Server=localhost;Database=TrainsDb;..." --project Sudan_Train
dotnet user-secrets set "emailSettings:Password" "your-app-password" --project Sudan_Train
dotnet user-secrets set "jwtSettings:Secret" "your-secret-key" --project Sudan_Train
```

#### Option 2: Environment Variables (Production)
```bash
# Linux/Mac
export ConnectionStrings__dbcontext="Server=..."
export emailSettings__Password="your-password"
export jwtSettings__Secret="your-secret"

# Windows
set ConnectionStrings__dbcontext=Server=...
set emailSettings__Password=your-password
set jwtSettings__Secret=your-secret

# Docker
# Add to docker-compose.yml environment section
```

#### Option 3: Azure App Settings (Production)
```bash
# Set in Azure Portal → Configuration → Application Settings
ConnectionStrings__dbcontext
emailSettings__Password
jwtSettings__Secret
```

---

## 📝 **OPTIONAL CHANGES**

### 4. **Serilog Configuration**

**Current (Good for Development)**:
```json
"Serilog": {
  "MinimumLevel": {
    "Default": "Information"
  }
}
```

#### For Production (Less Verbose):
```json
"Serilog": {
  "MinimumLevel": {
    "Default": "Warning",
    "Override": {
      "Microsoft": "Error",
      "System": "Error"
    }
  },
  "WriteTo": [
    {
      "Name": "Console"
    },
    {
      "Name": "File",
      "Args": {
        "path": "/var/log/trains/log-.txt",
        "retainedFileCountLimit": 90  // Keep 90 days
      }
    }
  ]
}
```

---

### 5. **CORS AllowedOrigins**

**Current (Open to All)**:
```json
"AllowedOrigins": "*"
```

#### For Production (Specific Domains):
```json
"AllowedOrigins": "https://yourfrontend.com,https://admin.yourfrontend.com"
```

Update `Program.cs`:
```csharp
var allowedOrigins = builder.Configuration["AllowedOrigins"]?.Split(',') ?? new[] { "*" };
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: CORS, policy =>
    {
        policy.WithOrigins(allowedOrigins)
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});
```

---

## 🎯 **COMPLETE EXAMPLES**

### Development (appsettings.Development.json)
```json
{
  "ConnectionStrings": {
    "dbcontext": "Server=localhost;Database=TrainsDb;Trusted_Connection=True;TrustServerCertificate=True;"
  },
  "Serilog": {
    "MinimumLevel": {
      "Default": "Debug"
    }
  }
}
```

### Docker (Use Environment Variables in docker-compose.yml)
```yaml
environment:
  - ConnectionStrings__dbcontext=Server=sqlserver,1433;Database=TrainsDb;User Id=sa;Password=YourStrong@Passw0rd;TrustServerCertificate=True;
  - jwtSettings__Secret=${JWT_SECRET}
  - emailSettings__Password=${EMAIL_PASSWORD}
```

### Production (appsettings.Production.json)
```json
{
  "ConnectionStrings": {
    "dbcontext": "USE_ENVIRONMENT_VARIABLE"
  },
  "jwtSettings": {
    "Secret": "USE_ENVIRONMENT_VARIABLE",
    "AccessTokenExpireDate": 30,
    "RefreshTokenExpireDate": 10080
  },
  "emailSettings": {
    "FromEmail": "noreply@yourcompany.com",
    "Password": "USE_ENVIRONMENT_VARIABLE"
  },
  "AllowedOrigins": "https://yourapp.com"
}
```

---

## ✅ **QUICK CHECKLIST**

Before deploying, verify:

- [ ] Connection string updated for target environment
- [ ] Email settings configured with real credentials
- [ ] JWT secret changed to secure random value
- [ ] Secrets stored in environment variables (not hardcoded)
- [ ] CORS origins restricted to specific domains
- [ ] Log level set appropriately (Warning/Error for production)
- [ ] Admin password changed after first login
- [ ] SQL Server password is strong (production)
- [ ] TLS/SSL enabled for database connection (production)

---

## 🚀 **Testing Configuration**

### Test Database Connection
```bash
# Add to a test endpoint
dotnet run --project Sudan_Train
# Check logs for "Database initialization completed"
```

### Test Email Configuration
```bash
# Send test email via Swagger
POST /api/test-email
```

### Test JWT Configuration
```bash
# Login and verify token generation
POST /api/authentication/login
```

---

## 📚 **Resources**

- [ASP.NET Core Configuration](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/configuration/)
- [User Secrets](https://learn.microsoft.com/en-us/aspnet/core/security/app-secrets)
- [Environment Variables](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/configuration/#environment-variables)
- [Gmail App Passwords](https://support.google.com/accounts/answer/185833)

---

## ⚠️ **NEVER COMMIT SECRETS TO GIT!**

Make sure these files are in `.gitignore`:
```
appsettings.Development.json
appsettings.Production.json
appsettings.*.json  # Except appsettings.json
*.secrets.json
.env
```

Use environment variables or Azure Key Vault for production secrets!

