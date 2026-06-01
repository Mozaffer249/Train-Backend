import { Bell, Search, User } from 'lucide-react';
import { useAuth } from '../../contexts/AuthContext';
import { AR } from '../../i18n/ar';

const Header = () => {
  const { user } = useAuth();

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
          <button className="relative p-2 text-gray-600 hover:bg-gray-100 rounded-lg transition-colors">
            <Bell size={20} />
            <span className="absolute top-1 end-1 w-2 h-2 bg-sudan-red-500 rounded-full"></span>
          </button>

          <div className="flex items-center gap-3 ps-4 border-s border-gray-200">
            <div className="text-end">
              <p className="text-sm font-medium text-gray-900">{user?.name || AR.auth.portalTitle}</p>
              <p className="text-xs text-gray-500">{user?.role === 'Admin' ? 'Super Admin' : 'Staff'}</p>
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
