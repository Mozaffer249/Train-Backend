import { useEffect, useState } from 'react';
import { stationsApi, usersApi } from '../../services/api';
import { AdminUser } from '../../types/infrastructure';
import { Station } from '../../types/geography';
import { AR } from '../../i18n/ar';
import { showError, showSuccess, extractErrorMessage } from '../../utils/alerts';

interface Props {
  isOpen: boolean;
  user: AdminUser | null;
  onClose: () => void;
  onSuccess: () => void;
}

const UserStationsModal = ({ isOpen, user, onClose, onSuccess }: Props) => {
  const [stations, setStations] = useState<Station[]>([]);
  const [selected, setSelected] = useState<number[] | null>(null);
  const [submitting, setSubmitting] = useState(false);
  const [search, setSearch] = useState('');

  useEffect(() => {
    if (isOpen) {
      stationsApi.getAll().then(setStations).catch((err) =>
        showError(AR.common.errorTitle, extractErrorMessage(err))
      );
      setSelected(null);
      setSearch('');
    }
  }, [isOpen]);

  if (!isOpen || !user) return null;

  const current = selected ?? user.stationIds;
  const filtered = stations.filter((s) => {
    const t = search.toLowerCase();
    return !t || s.nameEn.toLowerCase().includes(t) || s.nameAr.toLowerCase().includes(t);
  });

  const toggle = (id: number) => {
    const next = new Set(current);
    if (next.has(id)) next.delete(id); else next.add(id);
    setSelected(Array.from(next));
  };

  const save = async () => {
    setSubmitting(true);
    try {
      await usersApi.assignStations(user.id, current);
      await showSuccess(AR.users.stationsUpdated);
      onSuccess();
    } catch (err) {
      showError(AR.common.errorTitle, extractErrorMessage(err));
    } finally {
      setSubmitting(false);
    }
  };

  return (
    <div className="fixed inset-0 bg-black/40 z-50 flex items-center justify-center">
      <div className="bg-white rounded-lg w-full max-w-lg p-6">
        <h2 className="text-lg font-bold mb-2">{AR.users.assignStations}</h2>
        <p className="text-sm text-gray-600 mb-3">{user.userName}</p>
        <input
          type="text"
          value={search}
          onChange={(e) => setSearch(e.target.value)}
          placeholder={AR.users.searchPlaceholder}
          className="w-full border rounded-md px-3 py-1.5 text-sm mb-3"
        />
        <div className="max-h-64 overflow-y-auto space-y-1">
          {filtered.map((s) => (
            <label key={s.id} className="flex items-center gap-2 border rounded px-3 py-2">
              <input type="checkbox" checked={current.includes(s.id)} onChange={() => toggle(s.id)} />
              <span className="text-sm">{s.nameAr} ({s.code})</span>
            </label>
          ))}
          {!filtered.length && <p className="text-sm text-gray-500 px-1">{AR.common.none}</p>}
        </div>
        <div className="flex justify-end gap-2 mt-4">
          <button onClick={onClose} className="admin-button-secondary">{AR.common.cancel}</button>
          <button onClick={save} disabled={submitting} className="admin-button">
            {submitting ? AR.common.processing : AR.common.save}
          </button>
        </div>
      </div>
    </div>
  );
};

export default UserStationsModal;
