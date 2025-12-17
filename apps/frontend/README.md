# Sudan Train Frontend

React + TypeScript + Vite frontend application for the Sudan Train booking system.

## Tech Stack

- **React 18** - UI library
- **TypeScript** - Type safety
- **Vite** - Build tool and dev server
- **Tailwind CSS** - Styling
- **React Router** - Client-side routing
- **Lucide React** - Icons

## Prerequisites

- Node.js 18+ 
- npm or yarn

## Getting Started

### 1. Install Dependencies

```bash
npm install
```

### 2. Configure Environment

Copy the example environment file and update with your settings:

```bash
cp .env.example .env.local
```

Edit `.env.local`:

```env
VITE_API_URL=http://localhost:8080
VITE_MESSAGING_API_URL=http://localhost:5001
```

### 3. Run Development Server

```bash
npm run dev
```

The app will be available at `http://localhost:5173` (Vite's default port)

### 4. Build for Production

```bash
npm run build
```

The production build will be in the `dist/` directory.

### 5. Preview Production Build

```bash
npm run preview
```

## Docker

### Build Docker Image

```bash
docker build -t sudan-train-frontend .
```

### Run Docker Container

```bash
docker run -p 3000:80 sudan-train-frontend
```

Access at `http://localhost:3000`

## Project Structure

```
apps/frontend/
├── src/
│   ├── components/       # React components
│   │   ├── AdminPanel.tsx
│   │   ├── BookingPage.tsx
│   │   ├── Dashboard.tsx
│   │   ├── Header.tsx
│   │   ├── Homepage.tsx
│   │   ├── Login.tsx
│   │   └── SearchResults.tsx
│   ├── contexts/         # React contexts
│   │   ├── AuthContext.tsx
│   │   └── LanguageContext.tsx
│   ├── App.tsx          # Main app component
│   ├── main.tsx         # Entry point
│   └── index.css        # Global styles
├── public/              # Static assets
├── index.html           # HTML template
├── vite.config.ts       # Vite configuration
├── tailwind.config.js   # Tailwind configuration
├── tsconfig.json        # TypeScript configuration
├── Dockerfile           # Docker configuration
├── nginx.conf           # Nginx configuration for production
└── package.json         # Dependencies and scripts
```

## Available Scripts

- `npm run dev` - Start development server
- `npm run build` - Build for production
- `npm run lint` - Run ESLint
- `npm run preview` - Preview production build locally

## API Integration

The frontend communicates with the backend API using environment variables:

```typescript
const API_URL = import.meta.env.VITE_API_URL;

// Example API call
const response = await fetch(`${API_URL}/Api/V1/Authentication/Login`, {
  method: 'POST',
  headers: {
    'Content-Type': 'application/json',
  },
  body: JSON.stringify({ username, password }),
});
```

## Environment Variables

All environment variables must be prefixed with `VITE_` to be exposed to the frontend:

- `VITE_API_URL` - Backend API base URL
- `VITE_MESSAGING_API_URL` - Messaging API base URL
- `VITE_ENV` - Environment name (development/staging/production)
- `VITE_DEBUG` - Enable debug mode

## Development with Backend

When running with the backend via Docker Compose:

```bash
# From the root directory
docker-compose up -d

# Frontend: http://localhost:3000
# Backend API: http://localhost:8080/swagger
# Messaging API: http://localhost:5001
```

## Building for Production

1. Set production environment variables
2. Build the app: `npm run build`
3. Deploy the `dist/` directory to your web server
4. Or use the Docker image for containerized deployment

## Troubleshooting

### CORS Issues

If you encounter CORS errors, ensure the backend's CORS configuration includes your frontend URL:

```json
{
  "Cors": {
    "AllowedOrigins": [
      "http://localhost:5173",
      "http://localhost:3000"
    ]
  }
}
```

### API Connection

If the frontend can't connect to the API:

1. Check that `VITE_API_URL` is set correctly
2. Ensure the backend is running
3. Verify the backend port matches the URL
4. Check browser console for specific error messages

### Hot Reload Not Working

If hot reload stops working:

1. Restart the dev server
2. Clear browser cache
3. Check for ESLint errors
4. Ensure you're saving files in the `src/` directory

## Contributing

1. Create a feature branch
2. Make your changes
3. Run linting: `npm run lint`
4. Build successfully: `npm run build`
5. Submit a pull request

## License

[Add your license here]

