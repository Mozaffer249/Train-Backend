import { useState } from 'react';
import { Search, Plus, Edit, Trash2 } from 'lucide-react';
import { AR } from '../i18n/ar';

const UsersPage = () => {
  const [searchTerm, setSearchTerm] = useState('');

  const users = [
    { id: 1, name: 'أحمد حسن', email: 'ahmed@example.com', role: 'Customer', status: 'Active' },
    { id: 2, name: 'فاطمة علي', email: 'fatima@example.com', role: 'Customer', status: 'Active' },
    { id: 3, name: 'محمد إبراهيم', email: 'mohamed@example.com', role: 'Staff', status: 'Active' },
    { id: 4, name: 'سارة أحمد', email: 'sarah@example.com', role: 'Customer', status: 'Inactive' },
    { id: 5, name: 'عمر خليل', email: 'omar@example.com', role: 'Admin', status: 'Active' },
  ];

  const roleLabel: Record<string, string> = { Admin: 'مدير', Staff: 'موظف', Customer: 'عميل' };

  const filtered = users.filter((u) =>
    [u.name, u.email].some((v) => v.toLowerCase().includes(searchTerm.toLowerCase()))
  );

  return (
    <div>
      <div className="mb-8 flex items-center justify-between">
        <div>
          <h1 className="text-3xl font-bold text-gray-900">{AR.users.title}</h1>
          <p className="text-gray-600 mt-2">{AR.users.subtitle}</p>
        </div>
        <button className="admin-button flex items-center gap-2">
          <Plus size={20} />
          {AR.users.addUser}
        </button>
      </div>

      <div className="mb-6 p-4 bg-amber-50 border border-amber-200 rounded-lg text-amber-800 text-sm">
        {AR.common.sample}
      </div>

      <div className="admin-card mb-6">
        <div className="relative">
          <Search className="absolute start-3 top-1/2 -translate-y-1/2 text-gray-400" size={20} />
          <input
            type="text"
            placeholder={AR.users.searchPlaceholder}
            value={searchTerm}
            onChange={(e) => setSearchTerm(e.target.value)}
            className="w-full ps-10 pe-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-admin-primary-500"
          />
        </div>
      </div>

      <div className="admin-card">
        <div className="overflow-x-auto">
          <table className="w-full">
            <thead className="bg-gray-50 border-b border-gray-200">
              <tr>
                <th className="px-6 py-3 text-start text-xs font-medium text-gray-500 uppercase tracking-wider">الاسم</th>
                <th className="px-6 py-3 text-start text-xs font-medium text-gray-500 uppercase tracking-wider">البريد الإلكتروني</th>
                <th className="px-6 py-3 text-start text-xs font-medium text-gray-500 uppercase tracking-wider">الصلاحية</th>
                <th className="px-6 py-3 text-start text-xs font-medium text-gray-500 uppercase tracking-wider">{AR.trips.status}</th>
                <th className="px-6 py-3 text-start text-xs font-medium text-gray-500 uppercase tracking-wider">{AR.common.actions}</th>
              </tr>
            </thead>
            <tbody className="bg-white divide-y divide-gray-200">
              {filtered.map((user) => (
                <tr key={user.id} className="hover:bg-gray-50">
                  <td className="px-6 py-4 whitespace-nowrap font-medium text-gray-900">{user.name}</td>
                  <td className="px-6 py-4 whitespace-nowrap text-gray-600">{user.email}</td>
                  <td className="px-6 py-4 whitespace-nowrap">
                    <span
                      className={`px-3 py-1 text-xs rounded-full ${
                        user.role === 'Admin'
                          ? 'bg-sudan-gold-100 text-sudan-gold-800'
                          : user.role === 'Staff'
                          ? 'bg-admin-primary-50 text-admin-primary-800'
                          : 'bg-gray-100 text-gray-800'
                      }`}
                    >
                      {roleLabel[user.role] || user.role}
                    </span>
                  </td>
                  <td className="px-6 py-4 whitespace-nowrap">
                    <span
                      className={`px-3 py-1 text-xs rounded-full ${
                        user.status === 'Active' ? 'bg-green-100 text-green-800' : 'bg-red-100 text-red-800'
                      }`}
                    >
                      {AR.status[user.status] || user.status}
                    </span>
                  </td>
                  <td className="px-6 py-4 whitespace-nowrap text-sm">
                    <button className="text-admin-primary-700 hover:text-admin-primary-900 ms-0 me-3">
                      <Edit size={18} />
                    </button>
                    <button className="text-red-600 hover:text-red-800">
                      <Trash2 size={18} />
                    </button>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      </div>
    </div>
  );
};

export default UsersPage;
