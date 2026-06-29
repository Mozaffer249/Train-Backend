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
  ScanLine,
  RefreshCw,
  CreditCard,
} from 'lucide-react';
import { useAuth } from '../../contexts/AuthContext';
import { useMe } from '../../contexts/MeContext';
import { AR } from '../../i18n/ar';
import { Role, ROLES } from '../../types/infrastructure';

interface MenuItem {
  path: string;
  icon: typeof LayoutDashboard;
  label: string;
  roles: Role[]; // intersection with current user's roles → show
}

// Single source of truth for sidebar items + their role-visibility. The
// hide-decision here is UX only — backend authorization is still the security
// boundary (see RequireRole guards on the routes themselves).
const ALL_ITEMS: MenuItem[] = [
  { path: '/dashboard', icon: LayoutDashboard, label: AR.nav.dashboard,
    roles: [ROLES.SuperAdmin, ROLES.Admin, ROLES.Staff, ROLES.StaffCounter, ROLES.StaffBoarding] },
  { path: '/counter',   icon: Ticket,          label: AR.nav.counter,
    roles: [ROLES.SuperAdmin, ROLES.Admin, ROLES.StaffCounter] },
  { path: '/boarding',  icon: ScanLine,        label: AR.nav.boarding,
    roles: [ROLES.SuperAdmin, ROLES.Admin, ROLES.StaffBoarding] },
  { path: '/trips',     icon: CalendarDays,    label: AR.nav.trips,
    roles: [ROLES.SuperAdmin, ROLES.Admin, ROLES.Staff, ROLES.StaffCounter, ROLES.StaffBoarding] },
  { path: '/bookings',  icon: Ticket,          label: AR.nav.bookings,
    roles: [ROLES.SuperAdmin, ROLES.Admin, ROLES.Staff, ROLES.StaffCounter, ROLES.StaffBoarding] },
  { path: '/refunds',   icon: RefreshCw,       label: AR.nav.refunds,
    roles: [ROLES.SuperAdmin, ROLES.Admin] },
  { path: '/payments-report', icon: CreditCard, label: AR.nav.paymentsReport,
    roles: [ROLES.SuperAdmin, ROLES.Admin] },
  { path: '/users',     icon: Users,           label: AR.nav.users,
    roles: [ROLES.SuperAdmin, ROLES.Admin] },
  { path: '/fares',     icon: DollarSign,      label: AR.nav.fares,
    roles: [ROLES.SuperAdmin, ROLES.Admin] },
  { path: '/routes',    icon: RouteIcon,       label: AR.nav.routes,
    roles: [ROLES.SuperAdmin, ROLES.Admin] },
  { path: '/trains',    icon: Train,           label: AR.nav.trains,
    roles: [ROLES.SuperAdmin, ROLES.Admin] },
  { path: '/geography', icon: MapPin,          label: AR.nav.geography,
    roles: [ROLES.SuperAdmin, ROLES.Admin] },
  { path: '/seeding',   icon: Database,        label: AR.nav.seeding,
    roles: [ROLES.SuperAdmin] },
];

const Sidebar = () => {
  const location = useLocation();
  const navigate = useNavigate();
  const { logout } = useAuth();
  const { hasRole } = useMe();

  const menuItems = ALL_ITEMS.filter((item) => hasRole(...item.roles));

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
