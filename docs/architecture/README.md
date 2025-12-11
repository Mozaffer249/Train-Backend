# Architecture Documentation

System architecture, design patterns, and implementation details.

## 📄 Documents

### [Messaging API](./messaging-api.md)
Standalone microservice for all messaging operations:
- **Email Service** - SMTP integration with Gmail/custom servers
- **SMS Service** - Twilio integration
- **Push Notifications** - Firebase Cloud Messaging
- **Message Queue** - RabbitMQ for async processing
- **Message Tracking** - Database logging and status tracking

**Key Features:**
- Multiple sending strategies (Direct, Queued, Fallback)
- Background workers for queue processing
- Retry logic and error handling
- RESTful API endpoints

### [Email Service](./email-service.md)
Email service implementation details:
- SMTP configuration
- Email templates
- Attachment handling
- HTML and plain text support
- Error handling and logging

### [Email Strategy](./email-strategy.md)
Email sending strategies:
- **Direct** - Send immediately via SMTP
- **Queued** - Queue to RabbitMQ for later processing
- **Fallback** - Try direct, queue if fails

## 🏗️ Architecture Patterns

### Microservices
- **Train API** - Main application (Port 8080)
- **Messaging API** - Messaging microservice (Port 5001)

### Message Queue
- **RabbitMQ** - Async message processing
- **Background Workers** - EmailConsumerService, SmsConsumerService, PushConsumerService

### CQRS & MediatR
- Command/Query separation
- Handler pattern for business logic

### Clean Architecture
- Domain-driven design
- Dependency inversion
- Separation of concerns

## 🔗 Related Documentation

- [Development Guide](../development/register-handler-refactoring.md) - Clean code examples
- [Configuration](../configuration/appsettings-guide.md) - Service configuration
