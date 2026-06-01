import { useState, useEffect, useCallback } from 'react';
import { Search, Plus, Edit, Ban, Tag } from 'lucide-react';
import { tripsApi } from '../services/api';
import { Trip, TRIP_STATUSES } from '../types/infrastructure';
import { showConfirm, showSuccess, showError, extractErrorMessage } from '../utils/alerts';
import TripModal from '../components/trips/TripModal';
import FareModal from '../components/fares/FareModal';
import { AR } from '../i18n/ar';

function fmt(iso: string): string {
  const d = new Date(iso);
  return isNaN(d.getTime()) ? '--' : d.toLocaleString('ar-EG', { dateStyle: 'medium', timeStyle: 'short' });
}

const statusBadge = (status: string) => {
  switch (status) {
    case 'Scheduled':
      return 'bg-admin-primary-50 text-admin-primary-800';
    case 'Departed':
      return 'bg-sudan-gold-100 text-sudan-gold-800';
    case 'Completed':
      return 'bg-gray-100 text-gray-800';
    case 'Cancelled':
      return 'bg-red-100 text-red-800';
    default:
      return 'bg-orange-100 text-orange-800';
  }
};

const TripsPage = () => {
  const [searchTerm, setSearchTerm] = useState('');
  const [statusFilter, setStatusFilter] = useState('');
  const [dateFilter, setDateFilter] = useState('');
  const [trips, setTrips] = useState<Trip[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [modalOpen, setModalOpen] = useState(false);
  const [editing, setEditing] = useState<Trip | null>(null);
  // Per-row "Assign Fare" entry point — opens FareModal pre-scoped to this trip.
  const [fareTrip, setFareTrip] = useState<Trip | null>(null);

  const load = useCallback(() => {
    setLoading(true);
    setError('');
    tripsApi
      .getAll({ status: statusFilter || undefined, date: dateFilter || undefined })
      .then(setTrips)
      .catch((err) => setError(extractErrorMessage(err)))
      .finally(() => setLoading(false));
  }, [statusFilter, dateFilter]);

  useEffect(() => {
    load();
  }, [load]);

  const handleCancel = async (trip: Trip) => {
    const ok = await showConfirm(AR.trips.cancelPromptTitle, `${AR.trips.cancelPromptText} (${trip.routeName})`, AR.trips.yesCancel);
    if (!ok) return;
    try {
      await tripsApi.cancel(trip.id);
      showSuccess(AR.trips.cancelled);
      load();
    } catch (err) {
      showError(AR.trips.failedCancel, extractErrorMessage(err));
    }
  };

  const filtered = trips.filter(
    (t) =>
      t.trainName.toLowerCase().includes(searchTerm.toLowerCase()) ||
      t.routeName.toLowerCase().includes(searchTerm.toLowerCase())
  );

  return (
    <div>
      <div className="mb-8 flex items-center justify-between">
        <div>
          <h1 className="text-3xl font-bold text-gray-900">{AR.trips.title}</h1>
          <p className="text-gray-600 mt-2">{AR.trips.subtitle}</p>
        </div>
        <button
          onClick={() => {
            setEditing(null);
            setModalOpen(true);
          }}
          className="admin-button flex items-center gap-2"
        >
          <Plus size={20} />
          {AR.trips.addTrip}
        </button>
      </div>

      <div className="admin-card mb-6 flex flex-col md:flex-row gap-4">
        <div className="relative flex-1">
          <Search className="absolute start-3 top-1/2 -translate-y-1/2 text-gray-400" size={20} />
          <input
            type="text"
            placeholder={AR.trips.searchPlaceholder}
            value={searchTerm}
            onChange={(e) => setSearchTerm(e.target.value)}
            className="w-full ps-10 pe-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-admin-primary-500"
          />
        </div>
        <select
          value={statusFilter}
          onChange={(e) => setStatusFilter(e.target.value)}
          className="px-3 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-admin-primary-500"
        >
          <option value="">{AR.trips.allStatuses}</option>
          {TRIP_STATUSES.map((s) => (
            <option key={s} value={s}>{AR.status[s] || s}</option>
          ))}
        </select>
        <input
          type="date"
          value={dateFilter}
          onChange={(e) => setDateFilter(e.target.value)}
          className="px-3 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-admin-primary-500"
        />
      </div>

      <div className="admin-card">
        {loading ? (
          <p className="text-center text-gray-500 py-8">{AR.common.loading}</p>
        ) : error ? (
          <p className="text-center text-red-600 py-8">{error}</p>
        ) : filtered.length === 0 ? (
          <p className="text-center text-gray-500 py-8">{AR.trips.none}</p>
        ) : (
          <div className="overflow-x-auto">
            <table className="w-full">
              <thead className="bg-gray-50 border-b border-gray-200">
                <tr>
                  <th className="px-6 py-3 text-start text-xs font-medium text-gray-500 uppercase tracking-wider">{AR.trips.train}</th>
                  <th className="px-6 py-3 text-start text-xs font-medium text-gray-500 uppercase tracking-wider">{AR.trips.route}</th>
                  <th className="px-6 py-3 text-start text-xs font-medium text-gray-500 uppercase tracking-wider">{AR.trips.departure}</th>
                  <th className="px-6 py-3 text-start text-xs font-medium text-gray-500 uppercase tracking-wider">{AR.trips.arrival}</th>
                  <th className="px-6 py-3 text-start text-xs font-medium text-gray-500 uppercase tracking-wider">{AR.trips.seats}</th>
                  <th className="px-6 py-3 text-start text-xs font-medium text-gray-500 uppercase tracking-wider">{AR.trips.status}</th>
                  <th className="px-6 py-3 text-start text-xs font-medium text-gray-500 uppercase tracking-wider">{AR.common.actions}</th>
                </tr>
              </thead>
              <tbody className="bg-white divide-y divide-gray-200">
                {filtered.map((trip) => (
                  <tr key={trip.id} className="hover:bg-gray-50">
                    <td className="px-6 py-4 whitespace-nowrap text-gray-900">{trip.trainName}</td>
                    <td className="px-6 py-4 whitespace-nowrap text-gray-600">{trip.routeName}</td>
                    <td className="px-6 py-4 whitespace-nowrap text-gray-600">{fmt(trip.departureTime)}</td>
                    <td className="px-6 py-4 whitespace-nowrap text-gray-600">{fmt(trip.arrivalTime)}</td>
                    <td className="px-6 py-4 whitespace-nowrap text-gray-600">{trip.availableSeats}/{trip.totalSeats}</td>
                    <td className="px-6 py-4 whitespace-nowrap">
                      <span className={`px-3 py-1 text-xs rounded-full ${statusBadge(trip.status)}`}>{AR.status[trip.status] || trip.status}</span>
                    </td>
                    <td className="px-6 py-4 whitespace-nowrap text-sm">
                      <button onClick={() => setFareTrip(trip)} title={AR.fares.addFare} className="text-sudan-gold-700 hover:text-sudan-gold-900 ms-0 me-3">
                        <Tag size={18} />
                      </button>
                      <button onClick={() => { setEditing(trip); setModalOpen(true); }} title={AR.common.edit} className="text-admin-primary-700 hover:text-admin-primary-900 ms-0 me-3">
                        <Edit size={18} />
                      </button>
                      {trip.status !== 'Cancelled' && (
                        <button onClick={() => handleCancel(trip)} title={AR.trips.yesCancel} className="text-red-600 hover:text-red-800">
                          <Ban size={18} />
                        </button>
                      )}
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        )}
      </div>

      <TripModal isOpen={modalOpen} onClose={() => setModalOpen(false)} onSuccess={load} trip={editing} />
      <FareModal
        isOpen={!!fareTrip}
        pinnedTrip={fareTrip}
        onClose={() => setFareTrip(null)}
        onSuccess={() => setFareTrip(null)}
      />
    </div>
  );
};

export default TripsPage;
