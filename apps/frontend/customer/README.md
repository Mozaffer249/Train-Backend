# Sudan Train Customer App

Public-facing booking website for the Sudan Train Platform.

## Features

- **Train Search** - Search for available trains by route and date
- **Booking System** - Book train tickets online
- **User Authentication** - Register and login for customers
- **Booking History** - View past and upcoming bookings
- **Responsive Design** - Mobile-friendly interface
- **Multi-language** - Support for English and Arabic

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

The customer app will be available at http://localhost:5173 (Vite default)

### Build for Production

```bash
npm run build
```

### Run with Docker

```bash
# From project root
docker-compose up -d customer

# Or build and run
docker-compose up --build -d customer
```

## Environment Variables

Create a `.env.local` file:

```env
VITE_API_URL=http://localhost:8080
VITE_MESSAGING_API_URL=http://localhost:5001
```

## Project Structure

```
apps/frontend/customer/
├── src/
│   ├── components/      # Reusable UI components
│   ├── contexts/        # React contexts (Auth, etc.)
│   ├── pages/           # Page components
│   ├── services/        # API services
│   ├── App.tsx
│   └── main.tsx
├── public/
├── Dockerfile
├── nginx.conf
└── package.json
```

## Tech Stack

- **React 18** - UI framework
- **TypeScript** - Type safety
- **Vite** - Build tool and dev server
- **Tailwind CSS** - Utility-first CSS
- **React Router** - Client-side routing
- **Lucide React** - Icon library

## Available Scripts

- `npm run dev` - Start development server
- `npm run build` - Build for production
- `npm run preview` - Preview production build locally
- `npm run lint` - Run ESLint

## Related Documentation

- [Frontend Overview](../README.md) - Web applications overview
- [Admin Dashboard](../admin/README.md) - Admin portal
- [Backend API Documentation](../../backend/docs)
- [Deployment Guide](../../../docs/deployment)
- [Docker Setup](../../../docs/deployment/docker-setup.md)
- [Migration Guide](../../../MIGRATION_GUIDE.md)

## Access URLs

| Environment | URL |
|------------|-----|
| Development | http://localhost:5173 |
| Docker (Production) | http://localhost:3000 |

## Contributing

See the main [README](../../../README.md) for contribution guidelines.
