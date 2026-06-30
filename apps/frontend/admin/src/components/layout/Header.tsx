import { Search, User } from 'lucide-react';
import { useAuth } from '../../contexts/AuthContext';
import { useMe } from '../../contexts/MeContext';
import { AR } from '../../i18n/ar';
import NotificationsDrawer from '../NotificationsDrawer';
import { ROLES } from '../../types/infrastructure';

const roleLabel = (roles: string[]) => {
  if (roles.includes(ROLES.SuperAdmin)) return 'SuperAdmin';
  if (roles.includes(ROLES.Admin)) return 'Admin';
  if (roles.includes(ROLES.StaffCounter)) return 'StaffCounter';
  if (roles.includes(ROLES.StaffBoarding)) return 'StaffBoarding';
  if (roles.includes(ROLES.Staff)) return 'Staff';
  return roles[0] ?? AR.auth.portalTitle;
};

const Header = () => {
  const { user } = useAuth();
  const { me } = useMe();
  const displayRoles = me?.roles ?? user?.roles ?? [];

  return (
    <header className="bg-white shadow-sm border-b border-gray-200">
      <div className="flex items-center justify-between px-6 py-4">
        <div className="flex items-center flex-1 max-w-2xl">
          <div className="relative w-full">
            <Search className="absolute top-1/2 -translate-y-1/2 start-3 text-gray-400" size={20} />
            <input
              type="text"
              placeholder={AR.common.search + '…'}
              className="w-full ps-10 pe-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-admin-primary-500"
            />
          </div>
        </div>

        <div className="flex items-center gap-4">
          <NotificationsDrawer />

          <div className="flex items-center gap-3 ps-4 border-s border-gray-200">
            <div className="text-end">
              <p className="text-sm font-medium text-gray-900">{user?.name || AR.auth.portalTitle}</p>
              <p className="text-xs text-gray-500">{roleLabel(displayRoles)}</p>
            </div>
            <button className="p-2 text-gray-600 hover:bg-gray-100 rounded-lg transition-colors">
              <User size={20} />
            </button>
          </div>
        </div>
      </div>
    </header>
  );
};

export default Header;
