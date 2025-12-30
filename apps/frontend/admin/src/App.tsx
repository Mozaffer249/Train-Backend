import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom';
import { AuthProvider } from './contexts/AuthContext';
import Layout from './components/layout/Layout';
import Login from './pages/Login';
import Dashboard from './pages/Dashboard';
import GeographyPage from './pages/GeographyPage';
import RoutesPage from './pages/RoutesPage';
import FaresPage from './pages/FaresPage';
import SeedingPage from './pages/SeedingPage';
import UsersPage from './pages/UsersPage';
import BookingsPage from './pages/BookingsPage';
import TrainsPage from './pages/TrainsPage';
import TripsPage from './pages/TripsPage';

function App() {
  return (
    <AuthProvider>
      <BrowserRouter>
        <Routes>
          <Route path="/login" element={<Login />} />
          <Route path="/" element={<Layout />}>
            <Route index element={<Navigate to="/dashboard" replace />} />
            <Route path="dashboard" element={<Dashboard />} />
            <Route path="geography" element={<GeographyPage />} />
            <Route path="routes" element={<RoutesPage />} />
            <Route path="fares" element={<FaresPage />} />
            <Route path="seeding" element={<SeedingPage />} />
            <Route path="users" element={<UsersPage />} />
            <Route path="bookings" element={<BookingsPage />} />
            <Route path="trains" element={<TrainsPage />} />
            <Route path="trips" element={<TripsPage />} />
          </Route>
        </Routes>
      </BrowserRouter>
    </AuthProvider>
  );
}

export default App;
