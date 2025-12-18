# Sudan Train Frontend Applications

This directory contains all web-based client applications for the Sudan Train Platform.

## Applications

### 🌐 Customer App (`/customer`)

Public-facing booking website for customers to search, book, and manage train tickets.

**Features:**
- Train search and booking
- User registration and authentication
- Booking history
- Ticket management
- Responsive design (mobile & desktop)

**Quick Start:**
```bash
cd customer
npm install
npm run dev  # Runs on http://localhost:5173
```

📖 [Customer App Documentation](./customer/README.md)

### 🔐 Admin Dashboard (`/admin`)

Administrative portal for staff and administrators to manage the train booking system.

**Features:**
- User management
- Booking management
- Train fleet management
- Trip scheduling
- Statistics dashboard
- Role-based access control (Admin/Staff only)

**Quick Start:**
```bash
cd admin
npm install
npm run dev  # Runs on http://localhost:3001
```

📖 [Admin Dashboard Documentation](./admin/README.md)

## Tech Stack

Both applications share the same technology stack:

- **React 18** - UI framework
- **TypeScript** - Type safety
- **Vite** - Build tool and dev server
- **Tailwind CSS** - Utility-first styling
- **React Router** - Client-side routing
- **Lucide React** - Icon library

## Development

### Prerequisites

- Node.js 18+
- npm or yarn

### Run Both Apps Simultaneously

```bash
# Terminal 1 - Customer App
cd customer && npm run dev

# Terminal 2 - Admin App
cd admin && npm run dev
```

### Docker Deployment

Both apps are containerized and can be run via Docker Compose:

```bash
# From project root
docker-compose up -d

# Access apps:
# Customer: http://localhost:3000
# Admin: http://localhost:3001
```

## Environment Variables

Both apps require environment configuration:

**`.env.local` (Customer & Admin):**
```env
VITE_API_URL=http://localhost:8080
VITE_MESSAGING_API_URL=http://localhost:5001
```

## Building for Production

```bash
# Customer App
cd customer
npm run build  # Output: customer/dist/

# Admin App
cd admin
npm run build  # Output: admin/dist/
```

## Architecture

```
frontend/
├── customer/          # Public booking site
│   ├── src/
│   │   ├── components/
│   │   ├── contexts/
│   │   ├── pages/
│   │   └── services/
│   ├── public/
│   ├── Dockerfile
│   └── nginx.conf
│
└── admin/            # Admin dashboard
    ├── src/
    │   ├── components/
    │   │   └── layout/
    │   ├── contexts/
    │   ├── pages/
    │   └── services/
    ├── public/
    ├── Dockerfile
    └── nginx.conf
```

## Related Documentation

- [Backend API Documentation](../backend/docs)
- [Deployment Guide](../../docs/deployment)
- [Docker Setup](../../docs/deployment/docker-setup.md)
- [Migration Guide](../../MIGRATION_GUIDE.md)

## Access URLs

| Application | Development | Production (Docker) |
|------------|-------------|---------------------|
| Customer | http://localhost:5173 | http://localhost:3000 |
| Admin | http://localhost:3001 | http://localhost:3001 |

## Notes

- The customer app is accessible to all users
- The admin app requires Admin or Staff role authentication
- Both apps share the same backend API
- CORS is configured in the backend for both origins

