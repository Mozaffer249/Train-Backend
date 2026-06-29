import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom';
import { AuthProvider } from './contexts/AuthContext';
import { MeProvider } from './contexts/MeContext';
import Layout from './components/layout/Layout';
import RequireRole from './components/auth/RequireRole';
import { ROLES } from './types/infrastructure';
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
import BoardingPage from './pages/BoardingPage';
import CounterBookingPage from './pages/CounterBookingPage';
import RefundsPage from './pages/RefundsPage';
import PaymentsReportPage from './pages/PaymentsReportPage';

function App() {
  return (
    <AuthProvider>
      <MeProvider>
        <BrowserRouter>
          <Routes>
            <Route path="/login" element={<Login />} />
            <Route path="/" element={<Layout />}>
              <Route index element={<Navigate to="/dashboard" replace />} />
              <Route path="dashboard" element={<Dashboard />} />

              {/* Operational pages — open to all staff sub-roles + admin. */}
              <Route path="trips" element={<TripsPage />} />
              <Route path="bookings" element={<BookingsPage />} />

              {/* Counter sale — StaffCounter + Admin. */}
              <Route path="counter" element={
                <RequireRole roles={[ROLES.SuperAdmin, ROLES.Admin, ROLES.StaffCounter]}>
                  <CounterBookingPage />
                </RequireRole>
              } />

              {/* Boarding portal — StaffBoarding + Admin. */}
              <Route path="boarding" element={
                <RequireRole roles={[ROLES.SuperAdmin, ROLES.Admin, ROLES.StaffBoarding]}>
                  <BoardingPage />
                </RequireRole>
              } />

              {/* Admin-only pages. */}
              <Route path="users" element={
                <RequireRole roles={[ROLES.SuperAdmin, ROLES.Admin]}>
                  <UsersPage />
                </RequireRole>
              } />
              <Route path="refunds" element={
                <RequireRole roles={[ROLES.SuperAdmin, ROLES.Admin]}>
                  <RefundsPage />
                </RequireRole>
              } />
              <Route path="payments-report" element={
                <RequireRole roles={[ROLES.SuperAdmin, ROLES.Admin]}>
                  <PaymentsReportPage />
                </RequireRole>
              } />
              <Route path="fares" element={
                <RequireRole roles={[ROLES.SuperAdmin, ROLES.Admin]}>
                  <FaresPage />
                </RequireRole>
              } />
              <Route path="routes" element={
                <RequireRole roles={[ROLES.SuperAdmin, ROLES.Admin]}>
                  <RoutesPage />
                </RequireRole>
              } />
              <Route path="trains" element={
                <RequireRole roles={[ROLES.SuperAdmin, ROLES.Admin]}>
                  <TrainsPage />
                </RequireRole>
              } />
              <Route path="geography" element={
                <RequireRole roles={[ROLES.SuperAdmin, ROLES.Admin]}>
                  <GeographyPage />
                </RequireRole>
              } />
              <Route path="seeding" element={
                <RequireRole roles={[ROLES.SuperAdmin]}>
                  <SeedingPage />
                </RequireRole>
              } />
            </Route>
          </Routes>
        </BrowserRouter>
      </MeProvider>
    </AuthProvider>
  );
}

export default App;
