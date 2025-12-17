# Postman Testing Guide - Sudan Train Authentication

## 📦 Files Generated

1. **Sudan_Train_Authentication_Tests.postman_collection.json** - Complete test collection
2. **Sudan_Train_Dev.postman_environment.json** - Development environment variables

---

## 🚀 Quick Start

### Step 1: Import into Postman

1. Open Postman
2. Click **Import** button (top left)
3. Drag and drop both files:
   - `Sudan_Train_Authentication_Tests.postman_collection.json`
   - `Sudan_Train_Dev.postman_environment.json`
4. Select **Sudan Train - Development** environment from dropdown (top right)

### Step 2: Verify API is Running

1. Make sure Docker services are running:
   ```bash
   docker-compose up -d
   ```

2. Verify API is accessible:
   - Open browser: `http://localhost:5000/swagger`
   - Or test in Postman: GET `http://localhost:5000/Api/V1/Authentication/ValidateToken`

### Step 3: Start Testing

The collection is organized in **9 folders** representing different test scenarios.

---

## 📁 Collection Structure

### 01. Registration & Email Confirmation
- **Register New User** - Create a test account
- **Confirm Email** - Verify email (check inbox/spam)

**⚠️ IMPORTANT: Email Confirmation Required**

After registration, you MUST confirm your email before you can login:

1. **Check Email Inbox** (or spam folder) for confirmation email
2. **Find the confirmation code** in the email body
3. **Copy User ID and Code** from the email
4. **Run "Confirm Email"** request with those values
5. **Only after confirmation** can you successfully login

**If Email Service is Not Working:**
Check API logs for the confirmation code:
```bash
docker-compose logs train-api | grep "Confirmation Code"
```
The token will be logged there for testing purposes.

### 02. Login & Logout
- **Login (Without 2FA)** - Get JWT tokens (auto-saved to environment)
- **Validate Token** - Check if token is valid
- **Refresh Token** - Get new access token
- **Logout** - Revoke tokens

### 03. Two-Factor Authentication (2FA)
- **Enable 2FA - Get QR Code** - Scan with Google Authenticator
- **Verify 2FA Code** - Activate 2FA with 6-digit code
- **Get 2FA Status** - Check if enabled
- **Generate Recovery Codes** - Get 10 backup codes
- **Login with 2FA Code** - Login with authenticator app code
- **Login with Recovery Code** - Use backup code
- **Disable 2FA** - Turn off 2FA

### 04. Password Management
- **Change Password** - While authenticated
- **Send Password Reset Code** - For forgot password
- **Reset Password** - Using code from email

### 05. Account Management
- **Get Profile** - View user details
- **Update Profile** - Change name, address, phone, etc.
- **Change Email - Request** - Start email change process
- **Change Email - Confirm** - Complete with token from email
- **Export User Data (GDPR)** - Download all user data as JSON
- **Delete Account** - Permanently remove account

### 06. Session Management
- **Get Active Sessions** - View all login sessions
- **Terminate Single Session** - Logout from specific device
- **Terminate All Sessions** - Logout from all devices

### 07. Rate Limiting Tests
- **Test Login Rate Limit** - Run 6 times to trigger limit
- **Test Registration Rate Limit** - Rapid registration attempts

### 08. Error Handling Tests
- **Invalid Credentials** - Wrong username/password
- **Missing Required Fields** - Validation errors
- **Weak Password** - Password strength validation
- **Expired Token** - Token expiration handling
- **Invalid Token Format** - Malformed JWT

### 09. Localization Tests
- **Test English** - Default language
- **Test Arabic** - Arabic translations

---

## 🎯 Recommended Testing Workflow

### Basic Flow (First Time)
```
1. Register New User
   → Response: "Please check your email to confirm your account"
2. Check Email Inbox (or docker-compose logs)
   → Find User ID and Confirmation Code
3. Confirm Email
   → Use userId and code from email
   → Response: "Email confirmed successfully. You can now login"
4. Login (Without 2FA)
   → Token automatically saved to environment
5. Get Profile
6. Update Profile
7. Logout
```

**Common Issues:**
- **Error: "Please confirm your email before logging in"**
  → You skipped step 3, go back and confirm email first
- **Error: "User account is not active"**
  → Email confirmation failed or not completed
- **Email not received?**
  → Check spam folder or docker logs

### 2FA Flow
```
1. Login (get token)
2. Enable 2FA - Get QR Code
   → Scan with Google Authenticator app
3. Verify 2FA Code (enter 6-digit code from app)
4. Generate Recovery Codes (save them!)
5. Logout
6. Login with 2FA Code
   → Test with code from authenticator app
7. Test Login with Recovery Code
8. Disable 2FA
```

### Security Testing Flow
```
1. Login
2. Change Password
   → Check email notification
3. Enable 2FA
   → Check email notification
4. Get Active Sessions
5. Login from "another device" (new Postman tab/window)
6. Terminate Single Session
7. Change Email - Request
   → Check new email inbox
8. Change Email - Confirm
   → Check old email for security notification
```

### Account Management Flow
```
1. Login
2. Get Profile
3. Update Profile (change multiple fields)
4. Export User Data
   → Review JSON response
5. Get Active Sessions
6. Terminate All Sessions (Except Current)
7. Delete Account (if testing deletion)
   → Cannot be undone!
```

---

## 🔧 Environment Variables

The environment auto-manages these variables:

| Variable | Description | Auto-set |
|----------|-------------|----------|
| `baseUrl` | API base URL | Manual |
| `auth_token` | JWT access token | Yes (after login) |
| `refresh_token` | JWT refresh token | Yes (after login) |
| `2fa_manual_key` | 2FA manual entry key | Yes (after enable 2FA) |
| `recovery_code_1` | First recovery code | Yes (after generate) |
| `session_id` | Session ID for testing | Yes (after get sessions) |
| `test_username` | Default test username | Manual |
| `test_email` | Default test email | Manual |
| `test_password` | Default test password | Manual |

**To change API URL**: Edit `baseUrl` in environment (e.g., for production testing)

---

## 📝 Test Scripts Explained

### Auto-saving Tokens
When you login, test scripts automatically save tokens:
```javascript
pm.environment.set("auth_token", jsonData.data.accessToken);
pm.environment.set("refresh_token", jsonData.data.refreshToken);
```

### Auto-validation
Many requests include automatic validation:
```javascript
pm.test("Status code is 200", function () {
    pm.response.to.have.status(200);
});

pm.test("Response has token", function () {
    var jsonData = pm.response.json();
    pm.expect(jsonData.token).to.exist;
});
```

### Console Logging
Important data is logged to console:
- 2FA QR code URL
- Manual entry key
- Recovery codes (save these!)

**View Console**: Click "Console" button (bottom left in Postman)

---

## 🧪 Testing Scenarios

### Test 1: Complete New User Journey
1. Run folder **01. Registration & Email Confirmation**
2. Check email, copy confirmation code
3. Run **Confirm Email** with the code
4. Run **Login (Without 2FA)**
5. Run entire folder **05. Account Management**

### Test 2: 2FA Setup and Usage
1. Login first
2. Run entire folder **03. Two-Factor Authentication (2FA)**
3. Have Google Authenticator app ready
4. Follow prompts in response/console

### Test 3: Security Events & Notifications
1. Login
2. Run **Change Password** → Check email
3. Run **Enable 2FA** → Check email
4. Run **Disable 2FA** → Check email
5. Run **Change Email - Request** → Check new email
6. Run **Change Email - Confirm** → Check old email for notification

### Test 4: Rate Limiting
1. Run **Test Login Rate Limit** 6 times rapidly
2. 6th request should return 429 (Too Many Requests)
3. Wait 1 minute (check `appsettings.json` for duration)
4. Try again - should work

### Test 5: Error Handling
1. Run entire folder **08. Error Handling Tests**
2. Verify appropriate error messages
3. Test both English and Arabic (folder 09)

---

## 📧 Email Testing

### Gmail Setup (Development)
1. Use a real email address for testing
2. Check **Spam folder** if emails don't arrive
3. Email types you'll receive:
   - Welcome email (registration)
   - Email confirmation code
   - Password reset code
   - Email change confirmation
   - Security notifications:
     - Password changed
     - Email changed (to old email)
     - 2FA enabled
     - 2FA disabled
     - Session terminated

### Email Service Issues?
If emails not arriving:
```bash
# Check messaging-api logs
docker-compose logs messaging-api

# Check RabbitMQ
docker-compose logs rabbitmq

# Verify Gmail credentials in docker-compose.yml
# EmailSettings__FromEmail
# EmailSettings__UserName
# EmailSettings__Password (App Password)
```

---

## 🔍 Debugging Tips

### 1. Check Response Body
Always check the response:
- **Status Code** (200, 400, 401, 429, etc.)
- **Error messages** in response body
- **Validation errors** in `errors` array

### 2. View Console
Click **Console** (bottom left):
- See all request/response details
- View auto-saved variables
- Check test script output

### 3. Check Test Results
After each request, check **Test Results** tab:
- Green = Passed
- Red = Failed (with error details)

### 4. Verify Environment
Click eye icon (👁️) next to environment dropdown:
- Check `auth_token` is set after login
- Verify `baseUrl` is correct
- See all current variable values

### 5. Common Issues

**401 Unauthorized**
- Token expired or invalid
- Login again to get fresh token

**429 Too Many Requests**
- Rate limit hit
- Wait (default: 1 minute)

**400 Bad Request**
- Validation error
- Check request body format
- Verify required fields

**500 Internal Server Error**
- Check API logs: `docker-compose logs train-api`
- Check database connection

---

## 🎨 Using Variables in Requests

### In URL
```
{{baseUrl}}/Authentication/Login
```

### In Headers
```
Authorization: Bearer {{auth_token}}
```

### In Body
```json
{
  "userName": "{{test_username}}",
  "password": "{{test_password}}"
}
```

---

## 🔄 Running Collection with Newman (CLI)

### Install Newman
```bash
npm install -g newman
```

### Run Entire Collection
```bash
newman run Sudan_Train_Authentication_Tests.postman_collection.json \
  -e Sudan_Train_Dev.postman_environment.json
```

### Run Specific Folder
```bash
newman run Sudan_Train_Authentication_Tests.postman_collection.json \
  -e Sudan_Train_Dev.postman_environment.json \
  --folder "02. Login & Logout"
```

### Generate HTML Report
```bash
npm install -g newman-reporter-html

newman run Sudan_Train_Authentication_Tests.postman_collection.json \
  -e Sudan_Train_Dev.postman_environment.json \
  -r html
```

---

## 📊 Test Coverage

This collection covers:

✅ **Module 1: Core Security**
- Email confirmation enforcement
- Account lockout after failed attempts
- JWT token validation

✅ **Module 2: Two-Factor Authentication**
- Enable/disable 2FA
- QR code generation
- TOTP verification
- Recovery codes
- Login with 2FA

✅ **Module 3: Session Management**
- View active sessions
- Terminate single session
- Terminate all sessions

✅ **Module 5: Rate Limiting**
- Login rate limit
- Registration rate limit

✅ **Module 6: Account Management**
- Profile CRUD operations
- Email change flow
- Data export (GDPR)
- Account deletion

✅ **Module 7: Password Security**
- Password change
- Password reset flow
- Strength validation

✅ **Module 8: Security Notifications**
- All notification triggers tested
- Email delivery verification

✅ **Module 13: Localization**
- English translations
- Arabic translations

---

## 🛡️ Security Testing Checklist

- [ ] Expired token properly rejected
- [ ] Invalid token format handled
- [ ] Rate limiting works
- [ ] Account lockout after 5 failed attempts
- [ ] 2FA codes expire after use
- [ ] Recovery codes single-use only
- [ ] Password validation enforced
- [ ] Session termination works
- [ ] Email notifications sent
- [ ] HTTPS redirect (if enabled)

---

## 📈 Performance Metrics to Watch

Monitor these in Postman:
- **Response Time**: Should be < 2 seconds
- **Token Size**: JWT should be reasonable size
- **Session Count**: Can handle multiple sessions
- **Rate Limit Response**: Quick 429 response

---

## 🎓 Tips for Effective Testing

1. **Test in Order**: Follow folder numbers (01, 02, 03...)
2. **Save Tokens**: Login responses auto-save tokens
3. **Check Email**: Many flows require email verification
4. **Use Console**: View QR codes, recovery codes, errors
5. **Test Both Languages**: Switch `Accept-Language` header
6. **Clean State**: Delete test accounts between full test runs
7. **Document Bugs**: Use the bug report template in testing plan
8. **Test Edge Cases**: Empty strings, special characters, very long inputs

---

## 🔗 Additional Resources

- **Swagger UI**: `http://localhost:5000/swagger`
- **API Documentation**: See `MODULE-6-8-IMPLEMENTATION-SUMMARY.md`
- **Testing Plan**: See `authentication_testing_plan_*.plan.md`
- **Docker Logs**: `docker-compose logs -f train-api`

---

## 🆘 Support

If you encounter issues:

1. Check API logs: `docker-compose logs train-api`
2. Verify database: `docker-compose logs mssql`
3. Check email service: `docker-compose logs messaging-api`
4. Review test plan for detailed test cases
5. Check Postman console for detailed errors

---

**Happy Testing! 🚂🎉**

For questions or issues, refer to the comprehensive testing plan document.
