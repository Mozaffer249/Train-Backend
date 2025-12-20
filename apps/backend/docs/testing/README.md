# 🧪 Testing Documentation

Comprehensive testing guides and resources for the Sudan Train Backend API.

## 📦 Postman Collection

Complete Postman collection for testing all API endpoints is located in the [`postman/`](../../postman/) directory.

### Collection Files

| File | Description |
|------|-------------|
| [`Sudan_Train_Authentication_Tests.postman_collection.json`](../../postman/Sudan_Train_Authentication_Tests.postman_collection.json) | Complete API test suite - 71 endpoints (Authentication + Infrastructure) |
| [`Sudan_Train_Dev.postman_environment.json`](../../postman/Sudan_Train_Dev.postman_environment.json) | Development environment variables |
| [`Sudan_Train_Test_Environment.postman_environment.json`](../../postman/Sudan_Train_Test_Environment.postman_environment.json) | Test environment variables |

**Total Coverage: 72 endpoints in 2 main folders (Customer & Admin) with logical subfolders**

### Quick Start

1. **Import Collection into Postman:**
   ```bash
   # Open Postman Desktop
   # Click Import > File
   # Select: apps/backend/postman/Sudan_Train_Authentication_Tests.postman_collection.json
   ```

2. **Import Environment:**
   ```bash
   # Import the environment file
   # Select: apps/backend/postman/Sudan_Train_Dev.postman_environment.json
   # Set as active environment
   ```

3. **Start API:**
   ```bash
   cd ../../..  # Navigate to project root
   docker-compose up -d
   ```

4. **Run Tests:**
   - Open collection in Postman
   - Run requests individually or use Collection Runner

## 📚 Testing Guides

### Complete Guides

| Document | Description |
|----------|-------------|
| [Postman Testing Guide](./postman-testing-guide.md) | Step-by-step guide for using Postman |
| [Postman Collection Summary](./postman-collection-summary.md) | Collection structure overview |
| [Testing Quick Reference](./testing-quick-reference.md) | Quick commands and tips |

### Testing Workflow

```
1. Register User
   └→ Confirm Email
       └→ Login
           ├→ Access Protected Endpoints
           ├→ Enable 2FA
           ├→ Test Role-Based Access
           └→ Logout
```

## 🎯 Test Coverage

The Postman collection covers:

### Customer Endpoints
- ✅ Register new user
- ✅ Email confirmation
- ✅ Login (username/password)
- ✅ Login with 2FA
- ✅ Refresh token
- ✅ Logout
- ✅ Password reset
- ✅ Change password

### Two-Factor Authentication
- ✅ Enable 2FA
- ✅ Verify 2FA code
- ✅ Disable 2FA
- ✅ Generate recovery codes
- ✅ Login with recovery code

### Account Management
- ✅ Get profile
- ✅ Update profile
- ✅ Change email
- ✅ Delete account
- ✅ Export user data (GDPR)

### Session Management
- ✅ Get active sessions
- ✅ Terminate session
- ✅ Terminate all sessions

### Admin Endpoints
- ✅ Admin authentication
- ✅ Geography hierarchy (Regions → States → Cities)
- ✅ Infrastructure (Train stations with GPS, Railway routes with intermediate stops)
- ✅ Fleet Management (Trains, Coaches, Seats)
- ✅ Operations (Trip scheduling and management)
- ✅ Role-based access control (Admin/Staff/SuperAdmin)

## 🔧 Environment Configuration

### Development Environment

Update `Sudan_Train_Dev.postman_environment.json`:

```json
{
  "baseUrl": "http://localhost:8080",
  "messagingApiUrl": "http://localhost:5001"
}
```

### Test Environment

Update `Sudan_Train_Test_Environment.postman_environment.json` for your test server.

### Variables Auto-Set by Tests

These variables are automatically populated during test execution:

- `auth_token` - JWT access token
- `refresh_token` - JWT refresh token
- `user_id` - Current user ID
- `2fa_secret` - Two-factor authentication secret
- `recovery_code` - First recovery code

## 🚀 Running Tests

### Manual Testing

1. **Start Services:**
   ```bash
   docker-compose up -d
   ```

2. **Open Postman Collection**

3. **Run Folder or Individual Request**

### Automated Testing with Newman

Newman is Postman's command-line collection runner:

```bash
# Install Newman
npm install -g newman

# Run entire collection
newman run apps/backend/postman/Sudan_Train_Authentication_Tests.postman_collection.json \
  -e apps/backend/postman/Sudan_Train_Dev.postman_environment.json

# Run specific folder
newman run apps/backend/postman/Sudan_Train_Authentication_Tests.postman_collection.json \
  -e apps/backend/postman/Sudan_Train_Dev.postman_environment.json \
  --folder "Authentication"

# Generate HTML report
npm install -g newman-reporter-html
newman run apps/backend/postman/Sudan_Train_Authentication_Tests.postman_collection.json \
  -e apps/backend/postman/Sudan_Train_Dev.postman_environment.json \
  -r html --reporter-html-export report.html
```

## 📊 Test Scenarios

### Scenario 1: New User Registration

```
POST /Api/V1/Authentication/Register
  → Get confirmation email
  → Copy userId and code
POST /Api/V1/Authentication/ConfirmEmail
  → Success
POST /Api/V1/Authentication/Login
  → Get tokens
```

### Scenario 2: Enable Two-Factor Authentication

```
POST /Api/V1/Authentication/Login
  → Get tokens
POST /Api/V1/Authentication/EnableTwoFactor
  → Get QR code and secret
POST /Api/V1/Authentication/VerifyTwoFactor
  → Enter TOTP code
  → 2FA enabled
POST /Api/V1/Authentication/GenerateRecoveryCodes
  → Get 10 recovery codes (save them!)
```

### Scenario 3: Login with 2FA

```
POST /Api/V1/Authentication/Login
  → Response: "2FA required"
POST /Api/V1/Authentication/LoginWithTwoFactor
  → Enter TOTP code from authenticator app
  → Get tokens
```

### Scenario 4: Password Reset

```
POST /Api/V1/Authentication/SendResetPasswordCode
  → Get code via email
POST /Api/V1/Authentication/ResetPassword
  → Use code and set new password
  → Success
```

## 🐛 Debugging Failed Tests

### Common Issues

1. **401 Unauthorized**
   - Token expired → Login again
   - Missing token → Check auth_token variable
   - Invalid token → Clear variables and re-authenticate

2. **400 Bad Request**
   - Check request body format
   - Verify required fields
   - Check validation errors in response

3. **404 Not Found**
   - Verify API is running: `http://localhost:8080/swagger`
   - Check baseUrl in environment
   - Ensure correct endpoint path

4. **500 Internal Server Error**
   - Check API logs: `docker-compose logs backend-api`
   - Check database connection
   - Verify all services are running: `docker-compose ps`

### Logging

View API logs for debugging:

```bash
# All logs
docker-compose logs -f backend-api

# Last 100 lines
docker-compose logs --tail=100 backend-api

# Search for errors
docker-compose logs backend-api | grep -i error
```

## 📝 Test Data

### Default Test User

```json
{
  "username": "testuser",
  "email": "test@example.com",
  "password": "Test@123456",
  "firstName": "Test",
  "lastName": "User"
}
```

### Admin User (Seeded)

```json
{
  "username": "admin",
  "email": "admin@sudantrain.com",
  "password": "Admin@123456"
}
```

## 🔐 Security Testing

### Test Security Features

- ✅ Account lockout after failed attempts
- ✅ Email confirmation enforcement
- ✅ Rate limiting
- ✅ Token expiration
- ✅ Password strength validation
- ✅ 2FA enforcement
- ✅ Session management
- ✅ CORS restrictions

### Security Checklist

```bash
# Test account lockout
# Attempt login 5 times with wrong password
# 6th attempt should be blocked

# Test rate limiting
# Rapid fire requests to same endpoint
# Should get 429 Too Many Requests

# Test token expiration
# Wait for token to expire (check JWT settings)
# Request should fail with 401

# Test email confirmation
# Try login before confirming email
# Should be rejected
```

## 📖 Related Documentation

- [Postman Testing Guide](./postman-testing-guide.md) - Detailed testing instructions
- [API Documentation](../README.md) - Backend API overview
- [Authentication Docs](../authentication/) - Auth system details
- [Docker Setup](../../../../docs/deployment/docker-setup.md) - Running with Docker

## 🔗 External Resources

- [Postman Documentation](https://learning.postman.com/)
- [Newman CLI](https://github.com/postmanlabs/newman)
- [JWT.io](https://jwt.io/) - Decode JWT tokens
- [Swagger UI](http://localhost:8080/swagger) - Interactive API docs

## 💡 Tips

1. **Use Collection Variables** for common values
2. **Pre-request Scripts** can automate token refresh
3. **Test Scripts** can validate responses automatically
4. **Collection Runner** for batch testing
5. **Mock Servers** for frontend development without backend
6. **Monitors** for scheduled API health checks

## 🎓 Best Practices

### Organizing Tests

- Group related endpoints in folders
- Use descriptive request names
- Add documentation to requests
- Save example responses

### Writing Test Scripts

```javascript
// Validate status code
pm.test("Status code is 200", function () {
    pm.response.to.have.status(200);
});

// Validate response structure
pm.test("Response has token", function () {
    var jsonData = pm.response.json();
    pm.expect(jsonData.data.accessToken).to.exist;
});

// Auto-save tokens
if (pm.response.code === 200) {
    var jsonData = pm.response.json();
    pm.environment.set("auth_token", jsonData.data.accessToken);
    pm.environment.set("refresh_token", jsonData.data.refreshToken);
}
```

### Environment Management

- Use separate environments for dev/test/prod
- Never commit sensitive data
- Use `.env` files for local secrets
- Rotate credentials regularly

---

**Last Updated**: December 20, 2024
**Collection Version**: 2.0
**Endpoints Covered**: 40+
