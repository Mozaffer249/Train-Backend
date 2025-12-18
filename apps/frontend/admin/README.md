# Sudan Train Admin Dashboard

Admin management portal for the Sudan Train booking system.

## Features

- **Dashboard** - Overview statistics and recent activity
- **User Management** - Manage customers, staff, and administrators
- **Booking Management** - View, update, and cancel bookings
- **Train Management** - Manage train fleet and maintenance
- **Trip Management** - Schedule and manage train trips

## Quick Start

### Prerequisites
- Node.js 18+
- npm or yarn

### Development

```bash
# Install dependencies
npm install

# Start development server
npm run dev
```

The admin dashboard will be available at http://localhost:3001

### Build for Production

```bash
npm run build
```

### Run with Docker

```bash
# From project root
docker-compose up -d admin

# Or build and run
docker-compose up --build -d admin
```

## Authentication

The admin dashboard requires Admin or Staff role to access. Regular users will be denied access.

### Login

- Navigate to http://localhost:3001
- Use admin credentials from the backend
- Only users with `Admin` or `Staff` roles can log in

## Environment Variables

Create a `.env.local` file:

```env
VITE_API_URL=http://localhost:8080
VITE_MESSAGING_API_URL=http://localhost:5001
```

## Project Structure

```
apps/frontend/admin/
├── src/
│   ├── components/
│   │   └── layout/          # Sidebar, Header, Layout
│   ├── pages/               # Dashboard, Users, Bookings, etc.
│   ├── contexts/            # Auth context
│   ├── hooks/               # Custom hooks
│   ├── services/            # API services
│   ├── App.tsx
│   └── main.tsx
├── public/
├── Dockerfile
├── nginx.conf
└── package.json
```

## Pages

- `/login` - Admin login page
- `/dashboard` - Overview statistics
- `/users` - User management
- `/bookings` - Booking management
- `/trains` - Train fleet management
- `/trips` - Trip scheduling

## Tech Stack

- **React 18** - UI framework
- **TypeScript** - Type safety
- **Vite** - Build tool
- **Tailwind CSS** - Styling
- **React Router** - Routing
- **Lucide React** - Icons

## Related Documentation

- [Frontend Overview](../README.md) - Web applications overview
- [Customer App](../customer/README.md) - Public booking site
- [Backend API Documentation](../../backend/docs)
- [Deployment Guide](../../../docs/deployment)
- [Docker Setup](../../../docs/deployment/docker-setup.md)
