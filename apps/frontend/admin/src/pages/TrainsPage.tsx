import { useState, useEffect, useCallback } from 'react';
import { Search, Plus, Edit, Trash2, Layers } from 'lucide-react';
import { trainsApi } from '../services/api';
import { Train } from '../types/infrastructure';
import { showConfirm, showSuccess, showError, extractErrorMessage } from '../utils/alerts';
import TrainModal from '../components/trains/TrainModal';
import CoachesListModal from '../components/trains/CoachesListModal';
import { AR } from '../i18n/ar';

const TrainsPage = () => {
  const [searchTerm, setSearchTerm] = useState('');
  const [trains, setTrains] = useState<Train[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [modalOpen, setModalOpen] = useState(false);
  const [editing, setEditing] = useState<Train | null>(null);
  const [coachesFor, setCoachesFor] = useState<Train | null>(null);

  const load = useCallback(() => {
    setLoading(true);
    setError('');
    trainsApi
      .getAll()
      .then(setTrains)
      .catch((err) => setError(extractErrorMessage(err)))
      .finally(() => setLoading(false));
  }, []);

  useEffect(() => {
    load();
  }, [load]);

  const handleDelete = async (train: Train) => {
    const ok = await showConfirm(AR.common.confirmDelete, `${AR.trains.deletePrompt} — "${train.nameAr || train.nameEn}"`);
    if (!ok) return;
    try {
      await trainsApi.delete(train.id);
      showSuccess(AR.trains.deleted);
      load();
    } catch (err) {
      showError(AR.trains.failedDelete, extractErrorMessage(err));
    }
  };

  const filtered = trains.filter(
    (t) =>
      t.nameEn.toLowerCase().includes(searchTerm.toLowerCase()) ||
      t.nameAr.includes(searchTerm) ||
      t.trainNumber.toLowerCase().includes(searchTerm.toLowerCase())
  );

  return (
    <div>
      <div className="mb-8 flex items-center justify-between">
        <div>
          <h1 className="text-3xl font-bold text-gray-900">{AR.trains.title}</h1>
          <p className="text-gray-600 mt-2">{AR.trains.subtitle}</p>
        </div>
        <button
          onClick={() => {
            setEditing(null);
            setModalOpen(true);
          }}
          className="admin-button flex items-center gap-2"
        >
          <Plus size={20} />
          {AR.trains.addTrain}
        </button>
      </div>

      <div className="admin-card mb-6">
        <div className="relative">
          <Search className="absolute start-3 top-1/2 -translate-y-1/2 text-gray-400" size={20} />
          <input
            type="text"
            placeholder={AR.trains.searchPlaceholder}
            value={searchTerm}
            onChange={(e) => setSearchTerm(e.target.value)}
            className="w-full ps-10 pe-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-admin-primary-500"
          />
        </div>
      </div>

      <div className="admin-card">
        {loading ? (
          <p className="text-center text-gray-500 py-8">{AR.common.loading}</p>
        ) : error ? (
          <p className="text-center text-red-600 py-8">{error}</p>
        ) : filtered.length === 0 ? (
          <p className="text-center text-gray-500 py-8">{AR.trains.none}</p>
        ) : (
          <div className="overflow-x-auto">
            <table className="w-full">
              <thead className="bg-gray-50 border-b border-gray-200">
                <tr>
                  <th className="px-6 py-3 text-start text-xs font-medium text-gray-500 uppercase tracking-wider">{AR.trains.number}</th>
                  <th className="px-6 py-3 text-start text-xs font-medium text-gray-500 uppercase tracking-wider">{AR.trains.name}</th>
                  <th className="px-6 py-3 text-start text-xs font-medium text-gray-500 uppercase tracking-wider">{AR.trains.coaches}</th>
                  <th className="px-6 py-3 text-start text-xs font-medium text-gray-500 uppercase tracking-wider">{AR.trains.capacity}</th>
                  <th className="px-6 py-3 text-start text-xs font-medium text-gray-500 uppercase tracking-wider">{AR.common.actions}</th>
                </tr>
              </thead>
              <tbody className="bg-white divide-y divide-gray-200">
                {filtered.map((train) => (
                  <tr key={train.id} className="hover:bg-gray-50">
                    <td className="px-6 py-4 whitespace-nowrap font-medium text-gray-900">{train.trainNumber}</td>
                    <td className="px-6 py-4 whitespace-nowrap text-gray-900">
                      <div>{train.nameAr || train.nameEn}</div>
                      <div className="text-xs text-gray-500">{train.nameEn}</div>
                    </td>
                    <td className="px-6 py-4 whitespace-nowrap text-gray-600">{train.coachesCount}</td>
                    <td className="px-6 py-4 whitespace-nowrap text-gray-600">{train.totalCapacity} {AR.trains.seats}</td>
                    <td className="px-6 py-4 whitespace-nowrap text-sm">
                      <button onClick={() => setCoachesFor(train)} title={AR.trains.addCoachesAction} className="text-admin-primary-700 hover:text-admin-primary-900 ms-0 me-3">
                        <Layers size={18} />
                      </button>
                      <button onClick={() => { setEditing(train); setModalOpen(true); }} title={AR.common.edit} className="text-admin-primary-700 hover:text-admin-primary-900 ms-0 me-3">
                        <Edit size={18} />
                      </button>
                      <button onClick={() => handleDelete(train)} title={AR.common.delete} className="text-red-600 hover:text-red-800">
                        <Trash2 size={18} />
                      </button>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        )}
      </div>

      <TrainModal isOpen={modalOpen} onClose={() => setModalOpen(false)} onSuccess={load} train={editing} />
      <CoachesListModal isOpen={!!coachesFor} onClose={() => setCoachesFor(null)} onChange={load} train={coachesFor} />
    </div>
  );
};

export default TrainsPage;
