import { useState } from 'react';
import { usersApi } from '../../services/api';
import { AdminUser, ROLES } from '../../types/infrastructure';
import { AR } from '../../i18n/ar';
import { showError, showSuccess, extractErrorMessage } from '../../utils/alerts';

interface Props {
  isOpen: boolean;
  user: AdminUser | null;
  onClose: () => void;
  onSuccess: () => void;
}

const UserRolesModal = ({ isOpen, user, onClose, onSuccess }: Props) => {
  const [roles, setRoles] = useState<string[]>([]);
  const [submitting, setSubmitting] = useState(false);

  if (!isOpen || !user) return null;

  const initialRoles = roles.length ? roles : user.roles;

  const toggle = (r: string) => {
    const next = new Set(initialRoles);
    if (next.has(r)) next.delete(r); else next.add(r);
    setRoles(Array.from(next));
  };

  const save = async () => {
    setSubmitting(true);
    try {
      await usersApi.assignRoles(user.id, initialRoles);
      await showSuccess(AR.users.rolesUpdated);
      setRoles([]);
      onSuccess();
    } catch (err) {
      showError(AR.common.errorTitle, extractErrorMessage(err));
    } finally {
      setSubmitting(false);
    }
  };

  return (
    <div className="fixed inset-0 bg-black/40 z-50 flex items-center justify-center">
      <div className="bg-white rounded-lg w-full max-w-md p-6">
        <h2 className="text-lg font-bold mb-2">{AR.users.assignRoles}</h2>
        <p className="text-sm text-gray-600 mb-3">{user.userName}</p>
        <div className="space-y-1">
          {Object.values(ROLES).map((r) => (
            <label key={r} className="flex items-center gap-2 border rounded px-3 py-2">
              <input type="checkbox" checked={initialRoles.includes(r)} onChange={() => toggle(r)} />
              <span className="text-sm">{r}</span>
            </label>
          ))}
        </div>
        <div className="flex justify-end gap-2 mt-4">
          <button onClick={() => { setRoles([]); onClose(); }} className="admin-button-secondary">{AR.common.cancel}</button>
          <button onClick={save} disabled={submitting} className="admin-button">
            {submitting ? AR.common.processing : AR.common.save}
          </button>
        </div>
      </div>
    </div>
  );
};

export default UserRolesModal;
