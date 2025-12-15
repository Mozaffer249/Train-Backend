# Testing Quick Reference Card

## 🚀 Quick Start (5 Minutes)

### 1. Import to Postman
```
Files to Import:
✓ Sudan_Train_Authentication_Tests.postman_collection.json
✓ Sudan_Train_Test_Environment.postman_environment.json
```

### 2. Set Environment
```
Top-right dropdown → Select "Sudan Train - Test Environment"
```

### 3. Update Variables
```
base_url = http://localhost:5000 (or your API URL)
test_email = your-email@example.com (you have access to)
```

### 4. Start Testing!
```
Run requests in order from folders 1-8
```

---

## 📋 Test Execution Order

### Phase 1: Basic Auth (5 minutes)
```
1. Register New User
2. Confirm Email (get code from email)
3. Login Success (saves token automatically)
```

### Phase 2: 2FA Setup (5 minutes)
```
4. Enable 2FA (scan QR code)
5. Verify 2FA Code (enter 6-digit code)
6. Generate Recovery Codes (save them!)
7. Logout
8. Login With 2FA Code
```

### Phase 3: Account Management (5 minutes)
```
9. Get Profile
10. Update Profile
11. Change Password
12. Get Active Sessions
13. Export User Data
```

### Phase 4: Security Tests (5 minutes)
```
14. Rate Limiting Test (run 6+ times)
15. Invalid Token Test
16. SQL Injection Test
17. Unauthorized Access Test
```

---

## 🔑 Variables You Need to Set Manually

| Variable | Where to Get It | Example |
|----------|-----------------|---------|
| `email_confirmation_code` | Email or Database | ABC123 |
| `two_factor_code` | Authenticator App | 123456 |
| `reset_code` | Password Reset Email | XYZ789 |
| `email_change_token` | Email Change Email | token-string |

**All other variables are auto-populated by test scripts!**

---

## ✅ Expected Results Checklist

### Module 1: Core Security
- [ ] Registration sends email
- [ ] Cannot login without email confirmation
- [ ] Can login after confirmation
- [ ] Account locks after 5 failed attempts

### Module 2: Two-Factor Auth
- [ ] QR code generated
- [ ] Can scan with authenticator app
- [ ] 6-digit code verification works
- [ ] 10 recovery codes generated
- [ ] Can login with TOTP code
- [ ] Can login with recovery code
- [ ] Recovery code marked as used

### Module 3: Session Management
- [ ] Can view all active sessions
- [ ] Can terminate single session
- [ ] Can terminate all except current
- [ ] Terminated sessions cannot be used

### Module 4: Audit Logging
- [ ] Logs created for all actions
- [ ] Check database: `SELECT * FROM AuditLogs`

### Module 5: Rate Limiting
- [ ] Login blocked after 5 attempts
- [ ] Returns 429 status code
- [ ] Resets after configured time

### Module 6: Account Management
- [ ] Profile retrieval works
- [ ] Profile update works
- [ ] Email change sends confirmation
- [ ] Data export includes all user data
- [ ] Account deletion works (⚠️ permanent!)

### Module 7: Password Security
- [ ] Password change works
- [ ] Notification email received
- [ ] Cannot reuse recent password

### Module 8: Security Notifications
Check your email for:
- [ ] Password changed email
- [ ] 2FA enabled email
- [ ] 2FA disabled email
- [ ] Email changed notification (to old email)
- [ ] Session terminated email

---

## 🐛 Quick Troubleshooting

### Problem: 401 Unauthorized
```
Solution: Run "Login Success" request first
Check: access_token variable is set (eye icon)
```

### Problem: Email not received
```
1. Check spam folder
2. Verify MessagingApi running: docker-compose ps
3. Check logs: docker-compose logs messaging-api
```

### Problem: Invalid 2FA code
```
1. Get fresh code (updates every 30 seconds)
2. Check device time is synced
3. Verify you scanned correct QR code
```

### Problem: Rate limit hit
```
Wait 1 minute or restart API:
docker-compose restart train-api
```

---

## 🔍 Quick Database Queries

### Check User Status
```sql
SELECT Id, UserName, Email, EmailConfirmed, TwoFactorEnabled, AccessFailedCount
FROM Users 
WHERE Email = 'test1@example.com'
```

### View Active Sessions
```sql
SELECT * FROM LoginSessions 
WHERE UserId = 1 AND IsActive = 1
```

### View Audit Logs
```sql
SELECT TOP 20 * FROM AuditLogs 
WHERE UserId = 1 
ORDER BY Timestamp DESC
```

### View Security Events
```sql
SELECT * FROM SecurityEvents 
WHERE UserId = 1 
ORDER BY OccurredAt DESC
```

### Check 2FA Recovery Codes
```sql
SELECT Code, IsUsed FROM TwoFactorRecoveryCodes 
WHERE UserId = 1
```

---

## 📊 Test Coverage Summary

| Module | Requests | Critical | Optional |
|--------|----------|----------|----------|
| Registration | 3 | 3 | 0 |
| Login/Logout | 5 | 5 | 0 |
| 2FA | 7 | 5 | 2 |
| Password | 3 | 2 | 1 |
| Account Mgmt | 6 | 4 | 2 |
| Sessions | 3 | 3 | 0 |
| Rate Limiting | 1 | 1 | 0 |
| Security | 3 | 3 | 0 |
| **TOTAL** | **31** | **26** | **5** |

---

## ⚡ Speed Testing (Run All Critical)

### Automated Run with Newman
```bash
# Install
npm install -g newman

# Run all critical tests
newman run Sudan_Train_Authentication_Tests.postman_collection.json \
  -e Sudan_Train_Test_Environment.postman_environment.json \
  --delay-request 500

# Generate HTML report
newman run Sudan_Train_Authentication_Tests.postman_collection.json \
  -e Sudan_Train_Test_Environment.postman_environment.json \
  --reporters cli,html \
  --reporter-html-export test-results.html
```

---

## 🎯 Test Priorities

### Priority 1 (MUST TEST)
1. Register + Email Confirm
2. Login + Logout
3. 2FA Enable + Verify
4. Change Password
5. Session Management

### Priority 2 (SHOULD TEST)
6. Profile Management
7. Rate Limiting
8. Security Notifications
9. Data Export

### Priority 3 (NICE TO HAVE)
10. SQL Injection Tests
11. Performance Tests
12. Edge Cases

---

## 📱 Multi-Device Testing

### Setup
1. Run "Login Success" from **Postman** (Device 1)
2. Login from **Chrome** (Device 2)
3. Login from **Firefox** (Device 3)

### Test
4. Run "Get Active Sessions" → Should show 3 sessions
5. Run "Terminate Single Session" → Device 2 logged out
6. Run "Terminate All Sessions" → All except Postman logged out

---

## 🔐 Security Checklist

- [ ] Cannot access endpoints without token
- [ ] Invalid tokens are rejected
- [ ] Expired tokens don't work
- [ ] SQL injection blocked
- [ ] XSS attempts sanitized
- [ ] Rate limiting works
- [ ] Account lockout works
- [ ] 2FA cannot be bypassed
- [ ] Session termination works
- [ ] Passwords are hashed (check DB)

---

## 📈 Success Criteria

### All Green ✅
- All 26 critical requests pass
- All security tests pass
- Emails received for notifications
- No SQL exceptions
- No 500 errors
- Response times < 2 seconds

### Ready for Production ✅
- All modules tested
- No critical bugs
- Documentation complete
- Security verified
- Performance acceptable

---

## 🎉 Test Completion

When done, you should have:
- ✅ Postman collection fully executed
- ✅ All critical tests passing
- ✅ Security notifications in email
- ✅ Database records verified
- ✅ Multi-device sessions tested
- ✅ Rate limiting verified
- ✅ Test report generated

**Time Investment**: ~30 minutes for full test run

**Returns**: Production-ready authentication system! 🚀
