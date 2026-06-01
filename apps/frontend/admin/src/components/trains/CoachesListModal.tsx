import { Fragment, useState, useEffect, useCallback } from 'react';
import { X, Plus, Pencil, Check, XCircle, Armchair, ChevronDown, ChevronUp } from 'lucide-react';
import { Train, CoachClass } from '../../types/infrastructure';
import { trainsApi, coachesApi, AdminCoach, AdminSeat } from '../../services/api';
import { extractErrorMessage, showError, showSuccess } from '../../utils/alerts';
import { AR } from '../../i18n/ar';
import BulkCoachesModal from './BulkCoachesModal';

interface CoachesListModalProps {
  isOpen: boolean;
  onClose: () => void;
  // Parent's `load()` so train cards refresh after a coach edit changes seatsCount, etc.
  onChange: () => void;
  train: Train | null;
}

function classNameToId(name: string): number {
  switch (name) {
    case 'First': return CoachClass.First;
    case 'Third': return CoachClass.Third;
    default: return CoachClass.Second;
  }
}

function classLabel(name: string): string {
  switch (name) {
    case 'First': return AR.trains.classFirst;
    case 'Second': return AR.trains.classSecond;
    case 'Third': return AR.trains.classThird;
    default: return name;
  }
}

interface EditDraft {
  coachNumber: string;
  class: number;
  sequence: number;
}

const CoachesListModal = ({ isOpen, onClose, onChange, train }: CoachesListModalProps) => {
  const [coaches, setCoaches] = useState<AdminCoach[]>([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');
  const [bulkOpen, setBulkOpen] = useState(false);

  // Inline edit state: which coach id is being edited + its draft.
  const [editingId, setEditingId] = useState<number | null>(null);
  const [draft, setDraft] = useState<EditDraft>({ coachNumber: '', class: CoachClass.Second, sequence: 0 });
  const [saving, setSaving] = useState(false);

  // Coach IDs whose seat-map row is expanded + per-coach seat cache so toggling
  // closed-then-open doesn't refetch. `loadingSeats` tracks the in-flight fetch.
  const [expandedIds, setExpandedIds] = useState<Set<number>>(new Set());
  const [seatsByCoach, setSeatsByCoach] = useState<Record<number, AdminSeat[]>>({});
  const [loadingSeats, setLoadingSeats] = useState<Set<number>>(new Set());

  const toggleSeats = useCallback(async (coachId: number) => {
    setExpandedIds((prev) => {
      const next = new Set(prev);
      if (next.has(coachId)) next.delete(coachId); else next.add(coachId);
      return next;
    });
    if (seatsByCoach[coachId] || loadingSeats.has(coachId)) return;
    setLoadingSeats((prev) => new Set(prev).add(coachId));
    try {
      const rows = await coachesApi.getSeats(coachId);
      // Sort by numeric portion of seat number when possible, else lexicographic.
      const sorted = [...rows].sort((a, b) => {
        const an = parseInt(a.seatNumber.replace(/\D/g, ''), 10);
        const bn = parseInt(b.seatNumber.replace(/\D/g, ''), 10);
        if (!isNaN(an) && !isNaN(bn)) return an - bn;
        return a.seatNumber.localeCompare(b.seatNumber);
      });
      setSeatsByCoach((prev) => ({ ...prev, [coachId]: sorted }));
    } catch (err) {
      showError(AR.coaches.failedSave, extractErrorMessage(err));
    } finally {
      setLoadingSeats((prev) => {
        const next = new Set(prev);
        next.delete(coachId);
        return next;
      });
    }
  }, [seatsByCoach, loadingSeats]);

  const load = useCallback(async () => {
    if (!train) return;
    setLoading(true);
    setError('');
    try {
      const rows = await trainsApi.getCoaches(train.id);
      setCoaches([...rows].sort((a, b) => a.sequence - b.sequence));
    } catch (err) {
      setError(extractErrorMessage(err));
    } finally {
      setLoading(false);
    }
  }, [train]);

  useEffect(() => {
    if (isOpen) {
      setEditingId(null);
      load();
    }
  }, [isOpen, load]);

  const startEdit = (coach: AdminCoach) => {
    setEditingId(coach.id);
    setDraft({
      coachNumber: coach.coachNumber,
      class: classNameToId(coach.class),
      sequence: coach.sequence,
    });
  };

  const cancelEdit = () => {
    setEditingId(null);
  };

  const saveEdit = async (coach: AdminCoach) => {
    setSaving(true);
    try {
      await trainsApi.updateCoach(coach.id, {
        coachNumber: draft.coachNumber !== coach.coachNumber ? draft.coachNumber : undefined,
        class: draft.class !== classNameToId(coach.class) ? draft.class : undefined,
        sequence: draft.sequence !== coach.sequence ? draft.sequence : undefined,
      });
      showSuccess(AR.coaches.saved);
      setEditingId(null);
      await load();
      onChange();
    } catch (err) {
      showError(AR.coaches.failedSave, extractErrorMessage(err));
    } finally {
      setSaving(false);
    }
  };

  if (!isOpen || !train) return null;

  const inputClass = 'w-full px-2 py-1 border border-gray-300 rounded text-sm focus:outline-none focus:ring-2 focus:ring-admin-primary-500';

  return (
    <div className="fixed inset-0 z-50 overflow-y-auto">
      <div className="flex items-center justify-center min-h-screen px-4 pt-4 pb-20 text-center sm:block sm:p-0">
        <div className="fixed inset-0 transition-opacity bg-gray-500 bg-opacity-75" onClick={onClose} />
        <div className="inline-block align-bottom bg-white rounded-lg text-left overflow-hidden shadow-xl transform transition-all sm:my-8 sm:align-middle sm:max-w-3xl sm:w-full">
          <div className="bg-white px-6 pt-5 pb-4">
            <div className="flex items-center justify-between mb-4">
              <div>
                <h3 className="text-lg font-medium text-gray-900">{AR.coaches.titleFor} {train.nameAr || train.nameEn}</h3>
                <p className="text-xs text-gray-500 mt-1">{AR.coaches.capacityLocked}</p>
              </div>
              <button onClick={onClose} className="text-gray-400 hover:text-gray-500">
                <X size={24} />
              </button>
            </div>

            {error && <div className="mb-4 p-3 bg-red-50 border border-red-200 rounded-lg text-red-700 text-sm">{error}</div>}

            {loading ? (
              <p className="text-center text-gray-500 py-8">{AR.common.loading}</p>
            ) : coaches.length === 0 ? (
              <p className="text-center text-gray-500 py-8">{AR.coaches.none}</p>
            ) : (
              <div className="overflow-x-auto">
                <table className="w-full text-sm">
                  <thead className="bg-gray-50 border-b border-gray-200">
                    <tr>
                      <th className="px-3 py-2 text-start text-xs font-medium text-gray-500 uppercase tracking-wider">{AR.coaches.sequence}</th>
                      <th className="px-3 py-2 text-start text-xs font-medium text-gray-500 uppercase tracking-wider">{AR.coaches.coachNumber}</th>
                      <th className="px-3 py-2 text-start text-xs font-medium text-gray-500 uppercase tracking-wider">{AR.coaches.class}</th>
                      <th className="px-3 py-2 text-start text-xs font-medium text-gray-500 uppercase tracking-wider">{AR.coaches.capacity}</th>
                      <th className="px-3 py-2 text-start text-xs font-medium text-gray-500 uppercase tracking-wider">{AR.coaches.seats}</th>
                      <th className="px-3 py-2 text-start text-xs font-medium text-gray-500 uppercase tracking-wider">{AR.common.actions}</th>
                    </tr>
                  </thead>
                  <tbody className="bg-white divide-y divide-gray-200">
                    {coaches.map((coach) => {
                      const editing = editingId === coach.id;
                      const isExpanded = expandedIds.has(coach.id);
                      const isLoadingSeats = loadingSeats.has(coach.id);
                      const seats = seatsByCoach[coach.id];
                      return (
                        <Fragment key={coach.id}>
                        <tr className={editing ? 'bg-amber-50' : 'hover:bg-gray-50'}>
                          <td className="px-3 py-2 whitespace-nowrap text-gray-600">
                            {editing ? (
                              <input
                                type="number" min={0}
                                value={draft.sequence}
                                onChange={(e) => setDraft({ ...draft, sequence: Number(e.target.value) })}
                                className={`${inputClass} w-20`}
                              />
                            ) : coach.sequence}
                          </td>
                          <td className="px-3 py-2 whitespace-nowrap text-gray-900 font-medium">
                            {editing ? (
                              <input
                                type="text"
                                value={draft.coachNumber}
                                onChange={(e) => setDraft({ ...draft, coachNumber: e.target.value })}
                                className={`${inputClass} w-28`}
                                maxLength={20}
                              />
                            ) : coach.coachNumber}
                          </td>
                          <td className="px-3 py-2 whitespace-nowrap text-gray-600">
                            {editing ? (
                              <select
                                value={draft.class}
                                onChange={(e) => setDraft({ ...draft, class: Number(e.target.value) })}
                                className={`${inputClass} w-32`}
                              >
                                <option value={CoachClass.First}>{AR.trains.classFirst}</option>
                                <option value={CoachClass.Second}>{AR.trains.classSecond}</option>
                                <option value={CoachClass.Third}>{AR.trains.classThird}</option>
                              </select>
                            ) : classLabel(coach.class)}
                          </td>
                          <td className="px-3 py-2 whitespace-nowrap text-gray-500" title={AR.coaches.capacityLocked}>
                            {coach.capacity}
                          </td>
                          <td className="px-3 py-2 whitespace-nowrap text-gray-500">{coach.seatsCount}</td>
                          <td className="px-3 py-2 whitespace-nowrap">
                            {editing ? (
                              <div className="flex items-center gap-2">
                                <button
                                  onClick={() => saveEdit(coach)}
                                  disabled={saving}
                                  className="inline-flex items-center gap-1 px-2 py-1 text-xs bg-admin-primary-600 text-white rounded hover:bg-admin-primary-700 disabled:opacity-50"
                                  title={AR.common.save}
                                >
                                  <Check size={14} />
                                  {saving ? AR.common.processing : AR.common.save}
                                </button>
                                <button
                                  onClick={cancelEdit}
                                  disabled={saving}
                                  className="inline-flex items-center gap-1 px-2 py-1 text-xs border border-gray-300 text-gray-700 rounded hover:bg-gray-50"
                                  title={AR.common.cancel}
                                >
                                  <XCircle size={14} />
                                </button>
                              </div>
                            ) : (
                              <div className="flex items-center gap-2">
                                <button
                                  onClick={() => toggleSeats(coach.id)}
                                  className={`inline-flex items-center gap-1 px-2 py-1 text-xs border rounded hover:bg-gray-50 ${
                                    isExpanded
                                      ? 'border-admin-primary-300 text-admin-primary-700 bg-admin-primary-50'
                                      : 'border-gray-300 text-gray-700'
                                  }`}
                                  title={AR.coaches.viewSeats}
                                >
                                  <Armchair size={14} />
                                  {isExpanded ? <ChevronUp size={14} /> : <ChevronDown size={14} />}
                                </button>
                                <button
                                  onClick={() => startEdit(coach)}
                                  className="inline-flex items-center gap-1 px-2 py-1 text-xs border border-gray-300 text-gray-700 rounded hover:bg-gray-50"
                                  title={AR.common.edit}
                                >
                                  <Pencil size={14} />
                                </button>
                              </div>
                            )}
                          </td>
                        </tr>

                        {isExpanded && (
                          <tr className="bg-gray-50">
                            <td colSpan={6} className="px-4 py-4">
                              {isLoadingSeats || !seats ? (
                                <p className="text-center text-xs text-gray-500">{AR.common.loading}</p>
                              ) : seats.length === 0 ? (
                                <p className="text-center text-xs text-gray-500">{AR.coaches.noSeats}</p>
                              ) : (
                                <div>
                                  {/* Schematic coach: pill outline with a 4-per-row seat grid (2 seats · aisle · 2 seats).
                                      Window seats get a green dot, accessible seats get a blue ring. */}
                                  <div className="mx-auto max-w-md border-2 border-gray-300 rounded-3xl bg-white p-4">
                                    <div className="text-[10px] text-gray-400 text-center mb-2">
                                      {coach.coachNumber} · {classLabel(coach.class)} · {seats.length} {AR.coaches.seatsTotal}
                                    </div>
                                    <div className="grid grid-cols-[1fr_1fr_16px_1fr_1fr] gap-1.5">
                                      {seats.map((seat, idx) => {
                                        const col = idx % 4; // 0,1 = left; 2,3 = right (aisle in the middle)
                                        const placeAisleBefore = col === 2;
                                        return (
                                          <Fragment key={seat.id}>
                                            {placeAisleBefore && <div />}
                                            <button
                                              type="button"
                                              title={`${seat.seatNumber}${seat.isWindow ? ' · ' + AR.coaches.window : ''}${seat.isAccessible ? ' · ' + AR.coaches.accessible : ''}`}
                                              className={`relative aspect-square flex items-center justify-center rounded-md border text-[10px] font-medium
                                                ${seat.isAccessible
                                                  ? 'border-blue-400 ring-2 ring-blue-200 bg-blue-50 text-blue-800'
                                                  : 'border-gray-300 bg-white text-gray-700'}`}
                                            >
                                              {seat.seatNumber}
                                              {seat.isWindow && (
                                                <span className="absolute top-0.5 right-0.5 w-1.5 h-1.5 rounded-full bg-sudan-green-500" />
                                              )}
                                            </button>
                                          </Fragment>
                                        );
                                      })}
                                    </div>
                                    <div className="flex items-center justify-center gap-4 mt-3 text-[10px] text-gray-500">
                                      <span className="flex items-center gap-1">
                                        <span className="w-1.5 h-1.5 rounded-full bg-sudan-green-500" />
                                        {AR.coaches.window}
                                      </span>
                                      <span className="flex items-center gap-1">
                                        <span className="w-3 h-3 rounded-sm border border-blue-400 ring-2 ring-blue-200 bg-blue-50" />
                                        {AR.coaches.accessible}
                                      </span>
                                    </div>
                                  </div>
                                </div>
                              )}
                            </td>
                          </tr>
                        )}
                        </Fragment>
                      );
                    })}
                  </tbody>
                </table>
              </div>
            )}

            <div className="flex justify-between items-center pt-4 mt-4 border-t">
              <button
                type="button"
                onClick={() => setBulkOpen(true)}
                className="inline-flex items-center gap-2 px-3 py-2 text-sm border border-admin-primary-300 text-admin-primary-700 rounded-lg hover:bg-admin-primary-50"
              >
                <Plus size={16} />
                {AR.trains.addCoachesAction}
              </button>
              <button
                type="button"
                onClick={onClose}
                className="px-4 py-2 text-sm text-gray-700 bg-white border border-gray-300 rounded-lg hover:bg-gray-50"
              >
                {AR.common.close}
              </button>
            </div>
          </div>
        </div>
      </div>

      <BulkCoachesModal
        isOpen={bulkOpen}
        onClose={() => setBulkOpen(false)}
        onSuccess={() => { setBulkOpen(false); load(); onChange(); }}
        train={train}
      />
    </div>
  );
};

export default CoachesListModal;
