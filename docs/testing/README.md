# 🧪 Testing Documentation

This folder contains all testing-related documentation for the Sudan Train Backend project.

## 📁 Documents

| Document | Description |
|----------|-------------|
| [Postman Testing Guide](./postman-testing-guide.md) | Complete guide for using Postman to test authentication APIs |
| [Postman Collection Summary](./postman-collection-summary.md) | Overview of the Postman collection structure |
| [Testing Quick Reference](./testing-quick-reference.md) | Quick reference for common testing tasks |

## 🚀 Quick Start

### Import Postman Collection
1. Open Postman
2. Click **Import** button
3. Import files from project root:
   - `Sudan_Train_Authentication_Tests.postman_collection.json`
   - `Sudan_Train_Dev.postman_environment.json`

### Run Tests
```bash
# Start the API
docker-compose up -d

# Run tests with Newman (CLI)
npm install -g newman
newman run Sudan_Train_Authentication_Tests.postman_collection.json \
  -e Sudan_Train_Dev.postman_environment.json
```

## 📊 Test Coverage

The testing documentation covers:
- ✅ Registration & Email Confirmation
- ✅ Login & Logout flows
- ✅ Two-Factor Authentication (2FA)
- ✅ Password Management
- ✅ Account Management
- ✅ Session Management
- ✅ Rate Limiting
- ✅ Error Handling
- ✅ Localization (EN/AR)

## 🔗 Related Documentation

- [Authentication Docs](../authentication/) - Authentication implementation details
- [Development Docs](../development/) - Development guidelines
- [Deployment Docs](../deployment/) - Deployment and setup guides
