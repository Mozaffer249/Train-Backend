# Sudan Train Backend - Postman Collections

This directory contains Postman collections and environment files for testing the Sudan Train Backend APIs.

## 📦 Files

| File | Description |
|------|-------------|
| `Sudan_Train_Authentication_Tests.postman_collection.json` | Complete API test suite (Authentication + Infrastructure) - 71 endpoints |
| `Sudan_Train_Dev.postman_environment.json` | Development environment variables |
| `Sudan_Train_Test_Environment.postman_environment.json` | Test environment variables |

## 🚀 Quick Start

### 1. Import into Postman

1. Open Postman Desktop App
2. Click **Import** (top left)
3. Drag and drop all three files from this directory
4. Select **Sudan Train - Development** environment from the dropdown (top right)

### 2. Configure Environment

Edit the environment variables if needed:

**Development Environment:**
- `baseUrl`: `http://localhost:8080` (Backend API)
- `messagingApiUrl`: `http://localhost:5001` (Messaging API)

**Test Environment:**
- `baseUrl`: Your test server URL
- `messagingApiUrl`: Your test messaging API URL

### 3. Start the API

```bash
# From monorepo root (recommended)
cd ../../../
docker-compose up -d

# Or run locally
cd apps/backend
dotnet run --project Sudan_Train/Trains.Api.csproj
```

### 4. Run Tests

The collection is organized in folders representing different test scenarios. Run them in order:

**Customer:**
1. Authentication (Register, Login, Logout, Tokens)
2. Security (2FA, Password Management, Sessions)
3. Account Management (Profile, Email, Data Export)
4. Testing & Validation (Rate Limiting, Errors, Localization)

**Admin:**
1. Authentication (Admin Login)
2. Geography (Regions, States, Cities)
3. Infrastructure (Stations, Routes)
4. Fleet Management (Trains, Coaches & Seats)
5. Operations (Trips)

## 📋 Collection Structure

**File:** `Sudan_Train_Authentication_Tests.postman_collection.json`  
**Total Endpoints:** 72  
**Organization:** 2 main folders (Customer & Admin) with logical subfolders

### Customer Endpoints

Comprehensive test suite for the authentication system covering:

#### Module Coverage

- ✅ **Module 1: Core Security** - Account lockout, email confirmation, logout
- ✅ **Module 2: Two-Factor Authentication** - TOTP, recovery codes
- ✅ **Module 3: Session Management** - Active sessions, device tracking
- ✅ **Module 5: Rate Limiting** - Brute force protection
- ✅ **Module 6: Account Management** - Profile, email change, data export
- ✅ **Module 7: Password Security** - Password policies
- ✅ **Module 8: Security Notifications** - Email alerts
- ✅ **Module 13: Localization** - EN/AR translations

#### Automated Features

The collection includes automated scripts that:

- **Auto-save tokens** - JWT tokens saved to environment after login
- **Auto-validation** - Response validation with tests
- **Auto-extraction** - Extracts and saves important IDs (2FA keys, session IDs, etc.)
- **Console logging** - Important data logged for reference

### Admin Endpoints

Complete infrastructure and fleet management API:

- **Authentication** - Admin login (1 endpoint)
- **Geography** - Regions, States, Cities hierarchy (17 endpoints)
  - Regions (5 endpoints)
  - States (6 endpoints)
  - Cities (6 endpoints)
- **Infrastructure** - Stations and Routes (13 endpoints)
  - Stations (6 endpoints)
  - Routes (7 endpoints)
- **Fleet Management** - Trains, Coaches, Seats (8 endpoints)
  - Trains (6 endpoints)
  - Coaches & Seats (2 endpoints)
- **Operations** - Trip scheduling (5 endpoints)

## 📚 Documentation

For detailed testing guides, see:

- **[Testing Overview](../docs/testing/README.md)** - Complete guide and setup
- **[Postman Testing Guide](../docs/testing/postman-testing-guide.md)** - Step-by-step walkthrough
- **[Postman Collection Summary](../docs/testing/postman-collection-summary.md)** - Collection structure
- **[Testing Quick Reference](../docs/testing/testing-quick-reference.md)** - Common tasks and commands

## 🧪 Test Scenarios

### Basic Authentication Flow

```
1. POST /Register - Create new user
   → Response includes userId
2. GET Email - Check inbox for confirmation code
3. POST /ConfirmEmail - Confirm with userId + code
4. POST /Login - Get JWT tokens (auto-saved)
5. GET /ValidateToken - Verify token works
6. POST /RefreshToken - Get new access token
7. POST /Logout - Revoke tokens
```

### 2FA Enrollment Flow

```
1. POST /Login - Get authenticated
2. POST /EnableTwoFactor - Get QR code + manual key
3. Scan QR with Google Authenticator app
4. POST /VerifyTwoFactor - Enter 6-digit code
5. POST /GenerateRecoveryCodes - Get 10 backup codes
6. POST /Logout
7. POST /LoginWithTwoFactor - Login with TOTP code
```

### Account Management Flow

```
1. POST /Login - Authenticate
2. GET /Profile - View user details
3. PUT /UpdateProfile - Change name, phone, etc.
4. POST /ChangeEmail/Request - Request email change
5. POST /ChangeEmail/Confirm - Confirm with token
6. GET /ExportUserData - Download all user data (GDPR)
```

## 🔧 Environment Variables

The collection uses these environment variables:

| Variable | Description | Auto-Set |
|----------|-------------|----------|
| `baseUrl` | Backend API URL | Manual |
| `messagingApiUrl` | Messaging API URL | Manual |
| `auth_token` | JWT access token | ✅ After login |
| `refresh_token` | JWT refresh token | ✅ After login |
| `2fa_manual_key` | 2FA setup key | ✅ After enable 2FA |
| `recovery_code_1` | First recovery code | ✅ After generate |
| `session_id` | Session ID | ✅ After get sessions |
| `test_username` | Default test username | Manual |
| `test_email` | Default test email | Manual |
| `test_password` | Default test password | Manual |

## 🌐 Newman (CLI Testing)

Run tests from command line:

### Install Newman

```bash
npm install -g newman newman-reporter-html
```

### Run Collection

```bash
# Run all tests
newman run Sudan_Train_Authentication_Tests.postman_collection.json \
  -e Sudan_Train_Dev.postman_environment.json

# Run specific folder
newman run Sudan_Train_Authentication_Tests.postman_collection.json \
  -e Sudan_Train_Dev.postman_environment.json \
  --folder "02. Login & Logout"

# Generate HTML report
newman run Sudan_Train_Authentication_Tests.postman_collection.json \
  -e Sudan_Train_Dev.postman_environment.json \
  -r html \
  --reporter-html-export test-report.html
```

### CI/CD Integration

Example GitHub Actions workflow:

```yaml
name: API Tests

on: [push, pull_request]

jobs:
  test:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v2
      
      - name: Run API Tests
        run: |
          npm install -g newman
          newman run apps/backend/postman/Sudan_Train_Authentication_Tests.postman_collection.json \
            -e apps/backend/postman/Sudan_Train_Test_Environment.postman_environment.json \
            --reporters cli,json \
            --reporter-json-export test-results.json
```

## 🔍 Troubleshooting

### Issue: 401 Unauthorized

**Solution:**
- Token expired → Login again to get fresh token
- Wrong environment selected → Check environment dropdown
- Token not saved → Ensure login request ran successfully

### Issue: 429 Too Many Requests

**Solution:**
- Rate limit hit → Wait 1 minute
- Check `appsettings.json` for rate limit settings
- Adjust test delays if running automated tests

### Issue: Email Confirmation Not Working

**Solutions:**
1. Check email inbox (and spam folder)
2. View API logs: `docker-compose logs backend-api | grep "Confirmation"`
3. Check Messaging API logs: `docker-compose logs messaging-api`
4. Verify SMTP settings in docker-compose.yml

### Issue: Tests Failing

**Checklist:**
1. ✅ Backend API running and accessible
2. ✅ Database seeded with test data
3. ✅ Correct environment selected
4. ✅ Environment variables configured
5. ✅ No conflicting test data (delete and re-register user)

## 📊 Test Coverage

| Feature | Endpoint Count | Test Count |
|---------|----------------|------------|
| Authentication | 12 | 25+ |
| Two-Factor Auth | 6 | 15+ |
| Account Management | 6 | 12+ |
| Session Management | 3 | 8+ |
| Password Management | 3 | 6+ |
| Rate Limiting | Various | 5+ |
| Error Handling | Various | 10+ |
| Localization | Various | 4+ |
| **Total** | **30+** | **85+** |

## 🔗 Related Resources

- [Backend API Documentation](../docs/README.md)
- [Authentication Documentation](../docs/authentication/)
- [Platform Documentation](../../../docs/README.md)
- [Swagger UI](http://localhost:8080/swagger) - When API is running
- [Messaging API](http://localhost:5001) - When messaging-api is running

## 🆕 Adding New Tests

To add tests to the collection:

1. Open Postman
2. Navigate to the Sudan Train collection
3. Create new request or folder
4. Add test scripts in the **Tests** tab:

```javascript
pm.test("Status code is 200", function () {
    pm.response.to.have.status(200);
});

pm.test("Response has expected data", function () {
    var jsonData = pm.response.json();
    pm.expect(jsonData).to.have.property('data');
    pm.expect(jsonData.succeeded).to.be.true;
});

// Save data to environment
pm.environment.set("variableName", jsonData.data.value);
```

5. Export the collection (overwrite existing file)
6. Update this README if needed

## 📝 Best Practices

1. **Always use environment variables** for URLs and sensitive data
2. **Run tests in order** - Some tests depend on previous ones
3. **Clean up test data** - Delete test users between full test runs
4. **Check console output** - Important data is logged there
5. **Save recovery codes** - You'll need them for 2FA recovery tests
6. **Use descriptive test names** - Makes debugging easier
7. **Add delays for rate-limited endpoints** - Prevents test failures

## 🤝 Contributing

To contribute test improvements:

1. Make changes in Postman
2. Export the collection (overwrite the JSON file)
3. Test thoroughly
4. Update this README if needed
5. Submit a pull request

---

**Last Updated:** December 20, 2024  
**Collection Version:** 2.0  
**Postman Version:** 10.0+  
**Monorepo Location:** `apps/backend/postman/`

