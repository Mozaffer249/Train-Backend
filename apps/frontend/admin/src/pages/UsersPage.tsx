import { useEffect, useMemo, useState } from 'react';
import { Plus, Edit, Shield, MapPin, ToggleLeft, ToggleRight } from 'lucide-react';
import { AR } from '../i18n/ar';
import { usersApi, stationsApi } from '../services/api';
import { AdminUser, ROLES } from '../types/infrastructure';
import type { Station } from '../types/geography';
import { showConfirm, showError, showSuccess, extractErrorMessage } from '../utils/alerts';
import UserModal from '../components/users/UserModal';
import UserRolesModal from '../components/users/UserRolesModal';
import UserStationsModal from '../components/users/UserStationsModal';
import { useMe } from '../contexts/MeContext';
import { OPERATIONAL_ROLES, canManageUser } from '../utils/roleHierarchy';

const UsersPage = () => {
  const { me } = useMe();
  const callerRoles = me?.roles ?? [];
  const [users, setUsers] = useState<AdminUser[]>([]);
  const [loading, setLoading] = useState(false);
  const [search, setSearch] = useState('');
  const [roleFilter, setRoleFilter] = useState('');
  const [activeFilter, setActiveFilter] = useState<string>('');
  // Stations lookup so we can render assigned-station Arabic names per row.
  const [stations, setStations] = useState<Station[]>([]);
  const stationNameById = useMemo(() => {
    const m = new Map<number, string>();
    stations.forEach((s) => m.set(s.id, s.nameAr || s.nameEn || s.code));
    return m;
  }, [stations]);

  useEffect(() => {
    stationsApi.getAll().then(setStations).catch(() => setStations([]));
  }, []);
  const [modalOpen, setModalOpen] = useState(false);
  const [editing, setEditing] = useState<AdminUser | null>(null);
  const [rolesUser, setRolesUser] = useState<AdminUser | null>(null);
  const [stationsUser, setStationsUser] = useState<AdminUser | null>(null);

  const load = async () => {
    setLoading(true);
    try {
      const data = await usersApi.getAll({
        search: search || undefined,
        role: roleFilter || undefined,
        isActive: activeFilter === '' ? undefined : activeFilter === 'true',
        excludePrivileged: true,
      });
      setUsers(data);
    } catch (err) {
      showError(AR.common.errorTitle, extractErrorMessage(err));
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => { load(); }, [roleFilter, activeFilter]);

  const handleToggleActive = async (u: AdminUser) => {
    const next = !u.isActive;
    const ok = await showConfirm(
      next ? AR.users.confirmEnable : AR.users.confirmDisable,
      u.userName,
    );
    if (!ok) return;
    try {
      await usersApi.setActive(u.id, next);
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

  const hasStaffRole = (u: AdminUser) =>
    u.roles.includes(ROLES.Staff) ||
    u.roles.includes(ROLES.StaffCounter) ||
    u.roles.includes(ROLES.StaffBoarding);

  return (
    <div>
      <div className="mb-6 flex items-center justify-between">
        <div>
          <h1 className="text-2xl font-bold text-gray-900">{AR.users.title}</h1>
          <p className="text-gray-600">{AR.users.subtitleOperational}</p>
        </div>
        <button className="admin-button flex items-center gap-2"
          onClick={() => { setEditing(null); setModalOpen(true); }}
        >
          <Plus size={18} />{AR.users.addUser}
        </button>
      </div>

      <form onSubmit={onSearchSubmit} className="admin-card p-4 mb-4 flex flex-wrap gap-3 items-end">
        <div className="flex-1 min-w-[200px]">
          <label className="text-xs text-gray-500">{AR.common.search}</label>
          <input value={search} onChange={(e) => setSearch(e.target.value)}
            placeholder={AR.users.searchPlaceholder}
            className="w-full border rounded-md px-3 py-1.5 text-sm" />
        </div>
        <div>
          <label className="text-xs text-gray-500">{AR.users.filterRole}</label>
          <select value={roleFilter} onChange={(e) => setRoleFilter(e.target.value)}
            className="border rounded-md px-3 py-1.5 text-sm">
            <option value="">{AR.common.none}</option>
            {OPERATIONAL_ROLES.map((r) => (<option key={r} value={r}>{r}</option>))}
          </select>
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
              <th className="px-4 py-3 text-start">{AR.users.stations}</th>
              <th className="px-4 py-3 text-start">{AR.common.actions}</th>
            </tr>
          </thead>
          <tbody>
            {loading && (<tr><td colSpan={6} className="px-4 py-6 text-center text-gray-500">{AR.common.loading}</td></tr>)}
            {!loading && users.length === 0 && (<tr><td colSpan={6} className="px-4 py-6 text-center text-gray-500">{AR.common.none}</td></tr>)}
            {users.map((u) => (
              <tr key={u.id} className="border-t">
                <td className="px-4 py-3">
                  {[u.firstName, u.lastName].filter(Boolean).join(' ') || '—'}
                  {!u.isActive && <span className="ms-2 px-2 py-0.5 text-xs bg-red-100 text-red-700 rounded">{AR.status.Inactive}</span>}
                </td>
                <td className="px-4 py-3">{u.userName}</td>
                <td className="px-4 py-3">{u.email || '—'}</td>
                <td className="px-4 py-3">
                  {u.roles.map((r) => (
                    <span key={r} className="me-1 px-2 py-0.5 rounded bg-admin-primary-50 text-admin-primary-800 text-xs">{r}</span>
                  ))}
                </td>
                <td className="px-4 py-3">
                  {u.stationIds.length === 0
                    ? '—'
                    : u.stationIds.map((id) => (
                        <span key={id} className="me-1 mb-1 inline-block px-2 py-0.5 rounded bg-sudan-gold-100 text-sudan-gold-800 text-xs">
                          {stationNameById.get(id) ?? `#${id}`}
                        </span>
                      ))}
                </td>
                <td className="px-4 py-3 flex gap-2">
                  {canManageUser(callerRoles, u.roles) ? (
                    <>
                      <button className="text-admin-primary-700" title={AR.common.edit}
                        onClick={() => { setEditing(u); setModalOpen(true); }}>
                        <Edit size={16} />
                      </button>
                      <button className="text-admin-primary-700" title={AR.users.assignRoles}
                        onClick={() => setRolesUser(u)}>
                        <Shield size={16} />
                      </button>
                      {hasStaffRole(u) && (
                        <button className="text-admin-primary-700" title={AR.users.assignStations}
                          onClick={() => setStationsUser(u)}>
                          <MapPin size={16} />
                        </button>
                      )}
                      <button className={u.isActive ? 'text-green-600' : 'text-gray-400'}
                        title={u.isActive ? AR.users.disable : AR.users.enable}
                        onClick={() => handleToggleActive(u)}>
                        {u.isActive ? <ToggleRight size={18} /> : <ToggleLeft size={18} />}
                      </button>
                    </>
                  ) : (
                    <span className="text-xs text-gray-400">{AR.users.requiresSuperAdmin}</span>
                  )}
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>

      <UserModal isOpen={modalOpen} editUser={editing}
        onClose={() => { setModalOpen(false); setEditing(null); }}
        onSuccess={() => { setModalOpen(false); setEditing(null); load(); }} />

      <UserRolesModal isOpen={!!rolesUser} user={rolesUser}
        onClose={() => setRolesUser(null)}
        onSuccess={() => { setRolesUser(null); load(); }} />

      <UserStationsModal isOpen={!!stationsUser} user={stationsUser}
        onClose={() => setStationsUser(null)}
        onSuccess={() => { setStationsUser(null); load(); }} />
    </div>
  );
};

export default UsersPage;
