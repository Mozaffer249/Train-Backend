# Migration Guide

This guide provides step-by-step instructions for migrating applications into the Sudan Train Platform monorepo.

## 📁 Monorepo Structure

```
Sudan-Train-Platform/
├── apps/
│   ├── backend/              # .NET 8 API and microservices
│   │   ├── Sudan_Train/          # Main API project
│   │   ├── Sudan_Train.Core/     # Business logic
│   │   ├── Sudan_Train.Data/     # Data entities
│   │   ├── Sudan_Train.Infrastructure/
│   │   ├── Sudan_Train.Service/
│   │   ├── Sudan_Train.MessagingApi/
│   │   ├── _Trains.sln
│   │   └── Dockerfile
│   │
│   └── frontend/             # Web applications
│       ├── customer/         # Public booking site
│       │   ├── src/
│       │   ├── public/
│       │   ├── package.json
│       │   ├── Dockerfile
│       │   └── .env.example
│       │
│       └── admin/            # Admin dashboard
│           ├── src/
│           ├── public/
│           ├── package.json
│           └── Dockerfile
│
├── docs/                     # Documentation
├── docker-compose.yml        # Docker orchestration
├── README.md                 # Project overview
└── MIGRATION_GUIDE.md        # This file
```

---

## 🎨 Migrating a React Application

### Step 1: Copy Your React Project

Copy your existing React application files into `apps/frontend/customer/` (or `apps/frontend/admin/` for admin applications):

```sh
# For customer-facing applications
# Option A: Fresh copy (replaces existing)
rm -rf apps/frontend/customer/*
cp -r /path/to/your-react-app/* apps/frontend/customer/

# Option B: Selective copy (preserve existing config)
cp -r /path/to/your-react-app/src apps/frontend/customer/
cp -r /path/to/your-react-app/public apps/frontend/customer/
cp /path/to/your-react-app/package.json apps/frontend/customer/

# For admin applications, use apps/frontend/admin/ instead
```

### Step 2: Update Environment Configuration

Create or update `apps/frontend/customer/.env.local` for local development:

```env
# API Configuration
VITE_API_URL=http://localhost:8080
VITE_MESSAGING_API_URL=http://localhost:5001

# Add any other environment variables your app needs
```

For Create React App projects, use `REACT_APP_` prefix:

```env
REACT_APP_API_URL=http://localhost:8080
REACT_APP_MESSAGING_API_URL=http://localhost:5001
```

### Step 3: Update API Calls

Ensure your API calls use environment variables instead of hardcoded URLs:

**Before:**

```js
// ❌ Hardcoded URL
const response = await fetch('http://localhost:5000/api/auth/login');
```

**After (Vite):**

```js
// ✅ Environment variable
const API_URL = import.meta.env.VITE_API_URL;
const response = await fetch(`${API_URL}/api/auth/login`);
```

**After (Create React App):**

```js
// ✅ Environment variable
const API_URL = process.env.REACT_APP_API_URL;
const response = await fetch(`${API_URL}/api/auth/login`);
```

### Step 4: Update package.json

Ensure your `package.json` has the necessary scripts and proxy configuration:

```json
{
  "name": "sudan-train-frontend",
  "version": "1.0.0",
  "private": true,
  "scripts": {
    "dev": "vite",
    "build": "vite build",
    "preview": "vite preview",
    "lint": "eslint . --ext .js,.jsx,.ts,.tsx",
    "test": "vitest"
  }
}
```

For Create React App with proxy:

```json
{
  "proxy": "http://localhost:8080"
}
```

### Step 5: Update Dockerfile (if needed)

The frontend Dockerfile should be in `apps/frontend/customer/Dockerfile`:

**For Vite projects:**

```dockerfile
# Build stage
FROM node:18-alpine as build
WORKDIR /app
COPY package*.json ./
RUN npm ci
COPY . .
RUN npm run build

# Production stage
FROM nginx:alpine
COPY --from=build /app/dist /usr/share/nginx/html
COPY nginx.conf /etc/nginx/conf.d/default.conf
EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]
```

**For Create React App:**

```dockerfile
# Build stage
FROM node:18-alpine as build
WORKDIR /app
COPY package*.json ./
RUN npm ci
COPY . .
RUN npm run build

# Production stage
FROM nginx:alpine
COPY --from=build /app/build /usr/share/nginx/html
COPY nginx.conf /etc/nginx/conf.d/default.conf
EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]
```

### Step 6: Create Nginx Configuration

Create `apps/frontend/customer/nginx.conf` for production:

```nginx
server {
    listen 80;
    server_name localhost;
    root /usr/share/nginx/html;
    index index.html;

    # Serve static files
    location / {
        try_files $uri $uri/ /index.html;
    }

    # API proxy (optional - for same-domain API calls)
    location /api/ {
        proxy_pass http://train-api:80/api/;
        proxy_http_version 1.1;
        proxy_set_header Upgrade $http_upgrade;
        proxy_set_header Connection 'upgrade';
        proxy_set_header Host $host;
        proxy_cache_bypass $http_upgrade;
    }

    # Gzip compression
    gzip on;
    gzip_types text/plain text/css application/json application/javascript text/xml application/xml application/xml+rss text/javascript;
}
```

### Step 7: Test the Integration

```sh
# Install dependencies
cd apps/frontend/customer
npm install

# Start development server
npm run dev

# In another terminal, start the backend (if not using Docker)
cd apps/backend
dotnet run --project Sudan_Train

# Or start everything with Docker
cd ../..
docker-compose up -d
```

### Step 8: Verify Everything Works

1. **Frontend loads**: http://localhost:3000
2. **API calls work**: Check browser Network tab
3. **Authentication works**: Test login/logout
4. **CORS is configured**: No cross-origin errors

---

## 🔧 Backend Configuration for Frontend

### CORS Configuration

Ensure the backend allows requests from the frontend origin. In `apps/backend/Sudan_Train/Program.cs`:

```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins(
            "http://localhost:3000",     // Local development
            "http://localhost:5173",     // Vite default port
            "https://your-production-domain.com"
        )
        .AllowAnyMethod()
        .AllowAnyHeader()
        .AllowCredentials();
    });
});

// In the middleware pipeline
app.UseCors("AllowFrontend");
```

### JWT Configuration

If using JWT authentication, ensure cookies or headers are properly handled:

```js
// Frontend: Include credentials in fetch requests
const response = await fetch(`${API_URL}/api/auth/login`, {
    method: 'POST',
    headers: {
        'Content-Type': 'application/json',
    },
    credentials: 'include', // For cookie-based auth
    body: JSON.stringify({ email, password }),
});

// Or use Authorization header
const response = await fetch(`${API_URL}/api/data`, {
    headers: {
        'Authorization': `Bearer ${token}`,
    },
});
```

---

## 🐳 Docker Integration

### Update docker-compose.yml

If you modified the frontend structure, ensure `docker-compose.yml` is updated:

```yaml
customer:
  build:
    context: ./apps/frontend/customer
    dockerfile: Dockerfile
  image: sudan-train-customer:latest
  container_name: sudan-train-customer
  environment:
    - VITE_API_URL=http://localhost:8080
    - VITE_MESSAGING_API_URL=http://localhost:5001
  ports:
    - "3000:80"
  depends_on:
    - train-api
  networks:
    - train-network
  restart: unless-stopped
```

### Build and Test

```sh
# Rebuild just the frontend
docker-compose up --build -d frontend

# View frontend logs
docker logs train-frontend -f

# Test the full stack
docker-compose up --build -d
```

---

## ✅ Post-Migration Checklist

- [ ] All source files copied to `apps/frontend/customer/` (or `apps/frontend/admin/` for admin apps)
- [ ] Environment variables configured (`.env.local`)
- [ ] API calls use environment variables
- [ ] `package.json` scripts are correct
- [ ] `Dockerfile` is present and works
- [ ] `nginx.conf` handles SPA routing
- [ ] CORS configured in backend
- [ ] Authentication flow tested
- [ ] Docker build succeeds
- [ ] Full stack works with `docker-compose up`

---

## 🔍 Troubleshooting

### CORS Errors

```
Access to fetch at 'http://localhost:8080/api/...' from origin 'http://localhost:3000' 
has been blocked by CORS policy
```

**Solution**: Update backend CORS configuration to allow the frontend origin.

### API Connection Refused

```
Failed to fetch: ERR_CONNECTION_REFUSED
```

**Solutions**:
1. Ensure backend is running on the correct port
2. Check environment variable is set correctly
3. Verify Docker network configuration

### Build Fails in Docker

**Solutions**:
1. Check `node_modules` is not being copied (use `.dockerignore`)
2. Ensure all dependencies are in `package.json`
3. Check for incompatible node version

### SPA Routing Doesn't Work

**Solution**: Ensure `nginx.conf` has the `try_files` directive for fallback to `index.html`.

---

## 📚 Related Documentation

- [Root README](./README.md) - Project overview
- [Docker Setup](./docs/deployment/docker-setup.md) - Complete Docker guide
- [Quickstart](./docs/deployment/quickstart.md) - Quick commands
- [Backend README](./apps/backend/README.md) - Backend setup
- [Backend Docs](./apps/backend/docs) - Backend documentation
- [Frontend Overview](./apps/frontend/README.md) - Web applications overview
- [Customer App](./apps/frontend/customer/README.md) - Public booking site
- [Admin App](./apps/frontend/admin/README.md) - Admin dashboard