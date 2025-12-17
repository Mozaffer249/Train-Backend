# ⚙️ Quick appsettings.json Configuration Guide

## 📝 What to Change in appsettings.json

Your `appsettings.json` is now **valid JSON** (no syntax errors). Here's what you need to update:

---

## 🔧 Required Changes

### 1. **Connection String** (Line 3)

**Current**:
```json
"dbcontext": "Server=localhost;Database=TrainsDb;Trusted_Connection=True;TrustServerCertificate=True;"
```

**Options**:

#### ✅ Local SQL Server (Windows Authentication):
```json
"dbcontext": "Server=localhost;Database=TrainsDb;Trusted_Connection=True;TrustServerCertificate=True;"
```
*or*
```json
"dbcontext": "Server=.\\SQLEXPRESS;Database=TrainsDb;Trusted_Connection=True;TrustServerCertificate=True;"
```

#### ✅ Local SQL Server (SQL Authentication):
```json
"dbcontext": "Server=localhost;Database=TrainsDb;User Id=sa;Password=YourPassword;TrustServerCertificate=True;"
```

#### ✅ Docker (use service name):
```json
"dbcontext": "Server=sqlserver,1433;Database=TrainsDb;User Id=sa;Password=YourStrong@Passw0rd;TrustServerCertificate=True;"
```

---

### 2. **Email Settings** (Lines 49-54) - Optional but Recommended

**Current**:
```json
"emailSettings": {
  "Host": "smtp.gmail.com",
  "Port": 587,
  "FromEmail": "your-email@gmail.com",
  "Password": "your-app-password"
}
```

**To Configure Gmail**:

1. Enable 2-Factor Authentication on your Gmail account
2. Generate App Password: https://myaccount.google.com/apppasswords
3. Update:

```json
"emailSettings": {
  "Host": "smtp.gmail.com",
  "Port": 587,
  "FromEmail": "youractualemail@gmail.com",
  "Password": "xxxx xxxx xxxx xxxx"
}
```

---

### 3. **JWT Secret** (Line 39) - Change for Production

**Current** (OK for development):
```json
"Secret": "TrainProjectSecretKey123456789TrainProjectSecretKey123456789"
```

**For Production** - Generate a secure random secret:

```bash
# Mac/Linux
openssl rand -base64 64

# Windows PowerShell
[Convert]::ToBase64String((1..64|ForEach-Object{Get-Random -Maximum 256}))
```

Then update:
```json
"Secret": "YOUR_GENERATED_64_CHARACTER_SECRET_HERE"
```

---

## 📊 Full Settings Reference

### Connection Strings
| Setting | Description |
|---------|-------------|
| `Server` | SQL Server address (localhost, IP, or service name) |
| `Database` | Database name (TrainsDb) |
| `Trusted_Connection` | Use Windows Authentication (True/False) |
| `User Id` | SQL Server username (if not using Windows Auth) |
| `Password` | SQL Server password (if not using Windows Auth) |
| `TrustServerCertificate` | Trust self-signed certificates (True for dev) |

### JWT Settings
| Setting | Value | Description |
|---------|-------|-------------|
| `Secret` | 64+ chars | Secret key for JWT signing |
| `Issuer` | TrainProject | Who issued the token |
| `Audience` | TrainProjectUsers | Who can use the token |
| `AccessTokenExpireDate` | 60 | Minutes (1 hour) |
| `RefreshTokenExpireDate` | 43200 | Minutes (30 days) |

### Email Settings
| Setting | Example | Description |
|---------|---------|-------------|
| `Host` | smtp.gmail.com | SMTP server address |
| `Port` | 587 | SMTP port (587 for TLS) |
| `FromEmail` | your-email@gmail.com | Sender email address |
| `Password` | App password | Gmail App Password (16 chars) |

---

## ✅ Quick Start

### Minimum Changes to Run Locally:

1. **Update Connection String** (if your SQL Server is not on localhost)
2. **Leave email settings** as-is (email features won't work but app will run)
3. **Run the app**:
   ```bash
   dotnet run --project Sudan_Train
   ```

That's it! The app will:
- ✅ Auto-create the database
- ✅ Create all tables
- ✅ Seed roles and admin user
- ✅ Start on http://localhost:5000

### Access Swagger:
http://localhost:5000/swagger

### Default Login:
- **Username**: admin
- **Password**: Admin@123

---

## 🚨 Important Notes

1. **JSON doesn't support comments** - All `//` comments have been removed
2. **Refer to CONFIGURATION.md** - For detailed configuration guide
3. **Email is optional** - App runs without it, but email features won't work
4. **Change JWT secret** - Before production deployment
5. **Change admin password** - After first login

---

## 📁 Configuration Files

- `appsettings.json` - Base configuration (this file)
- `appsettings.Development.json` - Development overrides
- `appsettings.Production.json` - Production template
- `appsettings.Docker.json` - Docker-specific settings
- `CONFIGURATION.md` - Complete detailed guide (377 lines)

---

## 🔐 Security Best Practices

### Development
Use User Secrets:
```bash
dotnet user-secrets init --project Sudan_Train
dotnet user-secrets set "emailSettings:Password" "your-password" --project Sudan_Train
```

### Production
Use Environment Variables:
```bash
export ConnectionStrings__dbcontext="Server=..."
export jwtSettings__Secret="your-secret"
export emailSettings__Password="your-password"
```

---

## 🎯 Summary

**Current State**: ✅ Valid JSON, ready to run

**To Run Locally**: 
- Update connection string if needed
- Run: `dotnet run --project Sudan_Train`

**To Configure Email**:
- Get Gmail App Password
- Update `FromEmail` and `Password`

**For Production**:
- Generate new JWT secret
- Use environment variables for secrets
- Change admin password after first login

For complete details, see **CONFIGURATION.md**