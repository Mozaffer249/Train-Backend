import { useEffect, useState } from 'react';
import { adminsApi } from '../../services/api';
import { AdminUser, ROLES } from '../../types/infrastructure';
import { AR } from '../../i18n/ar';
import { showError, showSuccess, extractErrorMessage } from '../../utils/alerts';
import { getAdminAssignableRoles, getDemotionRoles, targetHasPrivilegedRole } from '../../utils/roleHierarchy';

interface Props {
  isOpen: boolean;
  user: AdminUser | null;
  onClose: () => void;
  onSuccess: () => void;
}

const AdminRolesModal = ({ isOpen, user, onClose, onSuccess }: Props) => {
  const [roles, setRoles] = useState<string[]>([]);
  const [demoteMode, setDemoteMode] = useState(false);
  const [submitting, setSubmitting] = useState(false);

  useEffect(() => {
    if (user) {
      setRoles(user.roles);
      setDemoteMode(false);
    }
  }, [user, isOpen]);

  if (!isOpen || !user) return null;

  const privilegedRoles = getAdminAssignableRoles();
  const demotionRoles = getDemotionRoles();
  const currentRoles = roles.length ? roles : user.roles;

  const togglePrivileged = (r: string) => {
    setDemoteMode(false);
    const base = currentRoles.filter((x) => privilegedRoles.includes(x as typeof ROLES.Admin));
    const next = new Set(base);
    if (next.has(r)) next.delete(r); else next.add(r);
    setRoles(Array.from(next));
  };

  const toggleDemotion = (r: string) => {
    setDemoteMode(true);
    setRoles([r]);
  };

  const save = async () => {
    if (currentRoles.length === 0) {
      showError(AR.common.errorTitle, AR.admins.rolesRequired);
      return;
    }
    setSubmitting(true);
    try {
      await adminsApi.assignRoles(user.id, currentRoles);
      await showSuccess(AR.users.rolesUpdated);
      setRoles([]);
      setDemoteMode(false);
      onSuccess();
    } catch (err) {
      showError(AR.common.errorTitle, extractErrorMessage(err));
    } finally {
      setSubmitting(false);
    }
  };

  const isPrivilegedSelection = targetHasPrivilegedRole(currentRoles);

  return (
    <div className="fixed inset-0 bg-black/40 z-50 flex items-center justify-center">
      <div className="bg-white rounded-lg w-full max-w-md p-6">
        <h2 className="text-lg font-bold mb-2">{AR.admins.assignRoles}</h2>
        <p className="text-sm text-gray-600 mb-3">{user.userName}</p>

        <p className="text-xs font-semibold text-gray-500 mb-1">{AR.admins.privilegedRoles}</p>
        <div className="space-y-1 mb-4">
          {privilegedRoles.map((r) => (
            <label key={r} className="flex items-center gap-2 border rounded px-3 py-2">
              <input
                type="checkbox"
                checked={!demoteMode && currentRoles.includes(r)}
                disabled={demoteMode}
                onChange={() => togglePrivileged(r)}
              />
              <span className="text-sm">{r}</span>
            </label>
          ))}
        </div>

        <p className="text-xs font-semibold text-gray-500 mb-1">{AR.admins.demoteTo}</p>
        <p className="text-xs text-gray-500 mb-2">{AR.admins.demoteHint}</p>
        <div className="space-y-1">
          {demotionRoles.map((r) => (
            <label key={r} className="flex items-center gap-2 border rounded px-3 py-2 border-amber-200 bg-amber-50">
              <input
                type="radio"
                name="demoteRole"
                checked={demoteMode && currentRoles.length === 1 && currentRoles[0] === r}
                onChange={() => toggleDemotion(r)}
              />
              <span className="text-sm">{r}</span>
            </label>
          ))}
        </div>

        {!isPrivilegedSelection && demoteMode && (
          <p className="text-xs text-amber-700 mt-2">{AR.admins.demoteWarning}</p>
        )}

        <div className="flex justify-end gap-2 mt-4">
          <button onClick={() => { setRoles([]); setDemoteMode(false); onClose(); }} className="admin-button-secondary">{AR.common.cancel}</button>
          <button onClick={save} disabled={submitting} className="admin-button">
            {submitting ? AR.common.processing : AR.common.save}
          </button>
        </div>
      </div>
    </div>
  );
};

export default AdminRolesModal;
