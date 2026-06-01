import { Link, useLocation, useNavigate } from 'react-router-dom';
import {
  LayoutDashboard,
  Users,
  CalendarDays,
  Train,
  Ticket,
  MapPin,
  Database,
  Route as RouteIcon,
  DollarSign,
  LogOut,
} from 'lucide-react';
import { useAuth } from '../../contexts/AuthContext';
import { AR } from '../../i18n/ar';

const Sidebar = () => {
  const location = useLocation();
  const navigate = useNavigate();
  const { logout } = useAuth();

  const menuItems = [
    { path: '/dashboard', icon: LayoutDashboard, label: AR.nav.dashboard },
    { path: '/geography', icon: MapPin, label: AR.nav.geography },
    { path: '/routes', icon: RouteIcon, label: AR.nav.routes },
    { path: '/fares', icon: DollarSign, label: AR.nav.fares },
    { path: '/seeding', icon: Database, label: AR.nav.seeding },
    { path: '/users', icon: Users, label: AR.nav.users },
    { path: '/bookings', icon: Ticket, label: AR.nav.bookings },
    { path: '/trains', icon: Train, label: AR.nav.trains },
    { path: '/trips', icon: CalendarDays, label: AR.nav.trips },
  ];

  const isActive = (path: string) => location.pathname === path;

  const handleLogout = () => {
    logout();
    navigate('/login', { replace: true });
  };

  return (
    <aside className="bg-admin-secondary-900 text-white w-64 min-h-screen flex flex-col">
      <div className="p-6 border-b border-admin-secondary-800 flex items-center gap-3">
        <div className="bg-sudan-gold-400 p-2 rounded-lg">
          <Train className="h-6 w-6 text-admin-primary-800" />
        </div>
        <div className="leading-tight">
          <h1 className="text-lg font-bold">{AR.brand.name}</h1>
          <p className="text-xs text-admin-secondary-400">{AR.brand.portal}</p>
        </div>
      </div>

      <nav className="flex-1 p-4">
        <ul className="space-y-2">
          {menuItems.map((item) => (
            <li key={item.path}>
              <Link
                to={item.path}
                className={`flex items-center gap-3 px-4 py-3 rounded-lg transition-colors ${
                  isActive(item.path)
                    ? 'bg-admin-primary-700 text-white'
                    : 'text-admin-secondary-300 hover:bg-admin-secondary-800'
                }`}
              >
                <item.icon size={20} />
                <span>{item.label}</span>
              </Link>
            </li>
          ))}
        </ul>
      </nav>

      <div className="p-4 border-t border-admin-secondary-800">
        <button
          onClick={handleLogout}
          className="flex items-center gap-3 px-4 py-3 rounded-lg text-admin-secondary-300 hover:bg-admin-secondary-800 w-full transition-colors"
        >
          <LogOut size={20} />
          <span>{AR.nav.logout}</span>
        </button>
      </div>
    </aside>
  );
};

export default Sidebar;
