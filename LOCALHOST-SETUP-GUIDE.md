# Using Localhost for Email Confirmation Links

## ✅ Setup Complete!

Your email confirmation links are now configured to use **localhost** for development.

---

## 🔧 Configuration

### Current Setup

**appsettings.json:**
```json
{
  "Frontend": {
    "BaseUrl": "http://localhost:3000"
  }
}
```

**RegisterCommandHandler.cs:**
```csharp
var frontendBaseUrl = _configuration["Frontend:BaseUrl"] ?? "http://localhost:3000";
var confirmationUrl = $"{frontendBaseUrl}/confirm-email?userId={encodedUserId}&code={encodedToken}";
```

### Result
Confirmation emails will contain links like:
```
http://localhost:3000/confirm-email?userId=1&code=CfDJ8ABC123...
```

---

## 🧪 Testing Flow

### 1. Start Your Frontend (React/Angular/Vue)
```bash
# React
npm start   # Usually runs on http://localhost:3000

# Angular
ng serve    # Usually runs on http://localhost:4200

# Vue
npm run dev # Usually runs on http://localhost:3000
```

### 2. Register a User
```http
POST http://localhost:5000/Api/V1/Authentication/Register
{
  "email": "test@example.com",
  "password": "Test@123456",
  "confirmPassword": "Test@123456",
  "firstName": "Test",
  "lastName": "User"
}
```

### 3. Check Email
Open the confirmation email and you'll see:
```
Button text: "Confirm Email Address"
Link: http://localhost:3000/confirm-email?userId=1&code=ABC123...
```

### 4. Click the Button
- Opens your frontend at `http://localhost:3000/confirm-email`
- Frontend extracts `userId` and `code` from URL
- Frontend calls backend API to confirm

---

## 🎨 Frontend Confirmation Page

Create a page at `/confirm-email` in your frontend:

### React Example
```jsx
// pages/ConfirmEmail.jsx
import { useEffect, useState } from 'react';
import { useSearchParams, useNavigate } from 'react-router-dom';

export default function ConfirmEmail() {
  const [searchParams] = useSearchParams();
  const navigate = useNavigate();
  const [status, setStatus] = useState('loading'); // loading, success, error
  const [message, setMessage] = useState('');

  useEffect(() => {
    const confirmEmail = async () => {
      const userId = searchParams.get('userId');
      const code = searchParams.get('code');

      if (!userId || !code) {
        setStatus('error');
        setMessage('Invalid confirmation link');
        return;
      }

      try {
        const response = await fetch('http://localhost:5000/Api/V1/Authentication/ConfirmEmail', {
          method: 'POST',
          headers: { 'Content-Type': 'application/json' },
          body: JSON.stringify({
            userId: parseInt(userId),
            code: code
          })
        });

        const data = await response.json();

        if (data.succeeded) {
          setStatus('success');
          setMessage('Email confirmed successfully!');
          // Redirect to login after 2 seconds
          setTimeout(() => navigate('/login'), 2000);
        } else {
          setStatus('error');
          setMessage(data.message || 'Confirmation failed');
        }
      } catch (error) {
        setStatus('error');
        setMessage('Network error. Please try again.');
      }
    };

    confirmEmail();
  }, [searchParams, navigate]);

  return (
    <div className="confirm-email-page">
      {status === 'loading' && <p>Confirming your email...</p>}
      {status === 'success' && (
        <div className="success">
          <h2>✅ {message}</h2>
          <p>Redirecting to login...</p>
        </div>
      )}
      {status === 'error' && (
        <div className="error">
          <h2>❌ {message}</h2>
          <button onClick={() => navigate('/register')}>Back to Register</button>
        </div>
      )}
    </div>
  );
}
```

### Angular Example
```typescript
// confirm-email.component.ts
import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-confirm-email',
  template: `
    <div class="confirm-email-page">
      <div *ngIf="status === 'loading'">
        <p>Confirming your email...</p>
      </div>
      <div *ngIf="status === 'success'" class="success">
        <h2>✅ {{ message }}</h2>
        <p>Redirecting to login...</p>
      </div>
      <div *ngIf="status === 'error'" class="error">
        <h2>❌ {{ message }}</h2>
        <button (click)="goToRegister()">Back to Register</button>
      </div>
    </div>
  `
})
export class ConfirmEmailComponent implements OnInit {
  status = 'loading';
  message = '';

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private http: HttpClient
  ) {}

  ngOnInit() {
    const userId = this.route.snapshot.queryParamMap.get('userId');
    const code = this.route.snapshot.queryParamMap.get('code');

    if (!userId || !code) {
      this.status = 'error';
      this.message = 'Invalid confirmation link';
      return;
    }

    this.http.post('http://localhost:5000/Api/V1/Authentication/ConfirmEmail', {
      userId: parseInt(userId),
      code: code
    }).subscribe({
      next: (data: any) => {
        if (data.succeeded) {
          this.status = 'success';
          this.message = 'Email confirmed successfully!';
          setTimeout(() => this.router.navigate(['/login']), 2000);
        } else {
          this.status = 'error';
          this.message = data.message || 'Confirmation failed';
        }
      },
      error: () => {
        this.status = 'error';
        this.message = 'Network error. Please try again.';
      }
    });
  }

  goToRegister() {
    this.router.navigate(['/register']);
  }
}
```

---

## 🔄 Changing Ports

### If Your Frontend Runs on Different Port

#### Option 1: Update appsettings.json
```json
{
  "Frontend": {
    "BaseUrl": "http://localhost:4200"  // Change to your port
  }
}
```

#### Option 2: Use Environment Variables
```bash
# Set environment variable
export Frontend__BaseUrl="http://localhost:4200"

# Or in docker-compose.yml
environment:
  - Frontend__BaseUrl=http://localhost:4200
```

---

## 🌍 Environment-Specific URLs

### appsettings.Development.json
```json
{
  "Frontend": {
    "BaseUrl": "http://localhost:3000"
  }
}
```

### appsettings.Staging.json
```json
{
  "Frontend": {
    "BaseUrl": "https://staging.sudantrain.com"
  }
}
```

### appsettings.Production.json
```json
{
  "Frontend": {
    "BaseUrl": "https://sudantrain.com"
  }
}
```

---

## 🐳 Docker Configuration

### docker-compose.yml
```yaml
services:
  train-api:
    environment:
      - Frontend__BaseUrl=http://localhost:3000  # Development
      # - Frontend__BaseUrl=https://sudantrain.com  # Production
```

---

## ✅ Complete Test Example

### 1. Start Services
```bash
# Terminal 1: Start backend
docker-compose up -d

# Terminal 2: Start frontend
cd frontend
npm start
```

### 2. Register User
```bash
curl -X POST http://localhost:5000/Api/V1/Authentication/Register \
  -H "Content-Type: application/json" \
  -d '{
    "email": "test@example.com",
    "password": "Test@123456",
    "confirmPassword": "Test@123456",
    "firstName": "Test",
    "lastName": "User"
  }'
```

### 3. Check Email
Email will contain:
```html
<a href="http://localhost:3000/confirm-email?userId=1&code=CfDJ8...">
  Confirm Email Address
</a>
```

### 4. Click Link
- Opens: `http://localhost:3000/confirm-email?userId=1&code=CfDJ8...`
- Frontend calls API
- Account confirmed
- Redirects to login

---

## 🚀 Production Deployment

When deploying to production:

### 1. Update appsettings.Production.json
```json
{
  "Frontend": {
    "BaseUrl": "https://sudantrain.com"
  }
}
```

### 2. Update CORS (if needed)
```json
{
  "Cors": {
    "AllowedOrigins": [
      "http://localhost:3000",
      "https://sudantrain.com"
    ]
  }
}
```

### 3. Deploy
```bash
# Build and deploy
dotnet publish -c Release
# Deploy to server
```

---

## 🔍 Troubleshooting

### Email Link Goes to Wrong Port
**Fix:** Update `Frontend:BaseUrl` in appsettings.json

### CORS Error When Calling API
**Fix:** Add your frontend URL to `Cors:AllowedOrigins`

### Link Works but API Call Fails
**Check:**
- Backend is running on correct port
- API URL in frontend is correct
- CORS is configured properly

### Token Invalid Error
**Causes:**
- Token expired (>24 hours old)
- Wrong userId
- Token already used

---

## 📊 Current Configuration Summary

| Setting | Value | Purpose |
|---------|-------|---------|
| Frontend URL | `http://localhost:3000` | Email confirmation links |
| Backend URL | `http://localhost:5000` | API endpoint |
| Messaging API | `http://localhost:5001` | Email service |
| CORS Origins | `localhost:3000, localhost:4200` | Allowed frontends |

---

## ✨ Benefits of This Setup

✅ **Flexible:** Change URL via config file  
✅ **Environment-Specific:** Different URLs per environment  
✅ **Easy Testing:** Works with localhost  
✅ **Production-Ready:** Just update config for production  
✅ **No Code Changes:** Update appsettings.json only  

---

**Your system is now configured for localhost development!** 🎉

When you're ready for production, just update `Frontend:BaseUrl` in `appsettings.Production.json`!
