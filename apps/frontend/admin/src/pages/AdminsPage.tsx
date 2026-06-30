import { useEffect, useState } from 'react';
import { Plus, Edit, Shield, ToggleLeft, ToggleRight } from 'lucide-react';
import { AR } from '../i18n/ar';
import { adminsApi } from '../services/api';
import { AdminUser, ROLES } from '../types/infrastructure';
import { showConfirm, showError, showSuccess, extractErrorMessage } from '../utils/alerts';
import AdminModal from '../components/admins/AdminModal';
import AdminRolesModal from '../components/admins/AdminRolesModal';

const AdminsPage = () => {
  const [admins, setAdmins] = useState<AdminUser[]>([]);
  const [loading, setLoading] = useState(false);
  const [search, setSearch] = useState('');
  const [activeFilter, setActiveFilter] = useState<string>('');
  const [modalOpen, setModalOpen] = useState(false);
  const [editing, setEditing] = useState<AdminUser | null>(null);
  const [rolesUser, setRolesUser] = useState<AdminUser | null>(null);

  const load = async () => {
    setLoading(true);
    try {
      const data = await adminsApi.getAll({
        search: search || undefined,
        isActive: activeFilter === '' ? undefined : activeFilter === 'true',
      });
      setAdmins(data);
    } catch (err) {
      showError(AR.common.errorTitle, extractErrorMessage(err));
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => { load(); }, [activeFilter]);

  const handleToggleActive = async (u: AdminUser) => {
    const next = !u.isActive;
    const ok = await showConfirm(
      next ? AR.users.confirmEnable : AR.users.confirmDisable,
      u.userName,
    );
    if (!ok) return;
    try {
      await adminsApi.setActive(u.id, next);
      await showSuccess(next ? AR.users.enabled : AR.users.disabled);
      load();
    } catch (err) {
      showError(AR.common.errorTitle, extractErrorMessage(err));
    }
  };

  const onSearchSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    load();
  };

  const roleLabel = (r: string) => {
    if (r === ROLES.SuperAdmin) return AR.admins.roleSuperAdmin;
    if (r === ROLES.Admin) return AR.admins.roleAdmin;
    return r;
  };

  return (
    <div>
      <div className="mb-6 flex items-center justify-between">
        <div>
          <h1 className="text-2xl font-bold text-gray-900">{AR.admins.title}</h1>
          <p className="text-gray-600">{AR.admins.subtitle}</p>
        </div>
        <button className="admin-button flex items-center gap-2"
          onClick={() => { setEditing(null); setModalOpen(true); }}
        >
          <Plus size={18} />{AR.admins.addAdmin}
        </button>
      </div>

      <form onSubmit={onSearchSubmit} className="admin-card p-4 mb-4 flex flex-wrap gap-3 items-end">
        <div className="flex-1 min-w-[200px]">
          <label className="text-xs text-gray-500">{AR.common.search}</label>
          <input value={search} onChange={(e) => setSearch(e.target.value)}
            placeholder={AR.admins.searchPlaceholder}
            className="w-full border rounded-md px-3 py-1.5 text-sm" />
        </div>
        <div>
          <label className="text-xs text-gray-500">{AR.users.filterActive}</label>
          <select value={activeFilter} onChange={(e) => setActiveFilter(e.target.value)}
            className="border rounded-md px-3 py-1.5 text-sm">
            <option value="">{AR.users.allActive}</option>
            <option value="true">{AR.users.activeOnly}</option>
            <option value="false">{AR.users.inactiveOnly}</option>
          </select>
        </div>
        <button className="admin-button">{AR.common.search}</button>
      </form>

      <div className="admin-card overflow-hidden">
        <table className="min-w-full text-sm">
          <thead className="bg-gray-50 text-gray-700">
            <tr>
              <th className="px-4 py-3 text-start">{AR.users.fullName}</th>
              <th className="px-4 py-3 text-start">{AR.users.userName}</th>
              <th className="px-4 py-3 text-start">{AR.users.email}</th>
              <th className="px-4 py-3 text-start">{AR.users.roles}</th>
              <th className="px-4 py-3 text-start">{AR.common.actions}</th>
            </tr>
          </thead>
          <tbody>
            {loading && (<tr><td colSpan={5} className="px-4 py-6 text-center text-gray-500">{AR.common.loading}</td></tr>)}
            {!loading && admins.length === 0 && (<tr><td colSpan={5} className="px-4 py-6 text-center text-gray-500">{AR.common.none}</td></tr>)}
            {admins.map((u) => (
              <tr key={u.id} className="border-t">
                <td className="px-4 py-3">
                  {[u.firstName, u.lastName].filter(Boolean).join(' ') || '—'}
                  {!u.isActive && <span className="ms-2 px-2 py-0.5 text-xs bg-red-100 text-red-700 rounded">{AR.status.Inactive}</span>}
                </td>
                <td className="px-4 py-3">{u.userName}</td>
                <td className="px-4 py-3">{u.email || '—'}</td>
                <td className="px-4 py-3">
                  {u.roles.filter((r) => r === ROLES.Admin || r === ROLES.SuperAdmin).map((r) => (
                    <span key={r} className="me-1 px-2 py-0.5 rounded bg-purple-100 text-purple-800 text-xs">{roleLabel(r)}</span>
                  ))}
                </td>
                <td className="px-4 py-3 flex gap-2">
                  <button className="text-admin-primary-700" title={AR.common.edit}
                    onClick={() => { setEditing(u); setModalOpen(true); }}>
                    <Edit size={16} />
                  </button>
                  <button className="text-admin-primary-700" title={AR.admins.assignRoles}
                    onClick={() => setRolesUser(u)}>
                    <Shield size={16} />
                  </button>
                  <button className={u.isActive ? 'text-green-600' : 'text-gray-400'}
                    title={u.isActive ? AR.users.disable : AR.users.enable}
                    onClick={() => handleToggleActive(u)}>
                    {u.isActive ? <ToggleRight size={18} /> : <ToggleLeft size={18} />}
                  </button>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>

      <AdminModal isOpen={modalOpen} editUser={editing}
        onClose={() => { setModalOpen(false); setEditing(null); }}
        onSuccess={() => { setModalOpen(false); setEditing(null); load(); }} />

      <AdminRolesModal isOpen={!!rolesUser} user={rolesUser}
        onClose={() => setRolesUser(null)}
        onSuccess={() => { setRolesUser(null); load(); }} />
    </div>
  );
};

export default AdminsPage;
