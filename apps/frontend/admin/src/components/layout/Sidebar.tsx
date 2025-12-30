import { Link, useLocation } from 'react-router-dom';
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
  LogOut 
} from 'lucide-react';

const Sidebar = () => {
  const location = useLocation();

  const menuItems = [
    { path: '/dashboard', icon: LayoutDashboard, label: 'Dashboard' },
    { path: '/geography', icon: MapPin, label: 'Geography' },
    { path: '/routes', icon: RouteIcon, label: 'Routes' },
    { path: '/fares', icon: DollarSign, label: 'Fares' },
    { path: '/seeding', icon: Database, label: 'Data Seeding' },
    { path: '/users', icon: Users, label: 'Users' },
    { path: '/bookings', icon: Ticket, label: 'Bookings' },
    { path: '/trains', icon: Train, label: 'Trains' },
    { path: '/trips', icon: CalendarDays, label: 'Trips' },
  ];

  const isActive = (path: string) => location.pathname === path;

  return (
    <aside className="bg-admin-secondary-900 text-white w-64 min-h-screen flex flex-col">
      <div className="p-6 border-b border-admin-secondary-800">
        <h1 className="text-2xl font-bold">Sudan Train</h1>
        <p className="text-sm text-admin-secondary-400">Admin Dashboard</p>
      </div>

      <nav className="flex-1 p-4">
        <ul className="space-y-2">
          {menuItems.map((item) => (
            <li key={item.path}>
              <Link
                to={item.path}
                className={`flex items-center gap-3 px-4 py-3 rounded-lg transition-colors ${
                  isActive(item.path)
                    ? 'bg-admin-primary-600 text-white'
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
        <button className="flex items-center gap-3 px-4 py-3 rounded-lg text-admin-secondary-300 hover:bg-admin-secondary-800 w-full transition-colors">
          <LogOut size={20} />
          <span>Logout</span>
        </button>
      </div>
    </aside>
  );
};

export default Sidebar;
