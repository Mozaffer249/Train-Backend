import { useCallback, useEffect, useMemo, useState } from 'react';
import { ChevronLeft, ChevronRight, RefreshCw, Search, X } from 'lucide-react';
import { AR } from '../i18n/ar';
import { bookingsApi } from '../services/api';
import { Booking, BOOKING_STATUSES } from '../types/infrastructure';
import FilterDropdown from '../components/common/FilterDropdown';
import { extractErrorMessage, showConfirm, showError, showSuccess } from '../utils/alerts';

const PAGE_SIZE = 20;

function formatDateTime(iso: string): string {
  const d = new Date(iso);
  if (isNaN(d.getTime())) return '--';
  return d.toLocaleString('ar', { dateStyle: 'short', timeStyle: 'short' });
}

function formatTime(iso: string): string {
  const d = new Date(iso);
  if (isNaN(d.getTime())) return '--:--';
  return d.toLocaleTimeString('ar', { hour: '2-digit', minute: '2-digit' });
}

function statusBadgeClass(status: string): string {
  switch (status) {
    case 'Confirmed': return 'bg-green-100 text-green-800';
    case 'Pending': return 'bg-yellow-100 text-yellow-800';
    case 'Cancelled': return 'bg-red-100 text-red-800';
    case 'Completed': return 'bg-blue-100 text-blue-800';
    default: return 'bg-gray-100 text-gray-800';
  }
}

const BookingsPage = () => {
  const [bookings, setBookings] = useState<Booking[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [searchTerm, setSearchTerm] = useState('');
  const [statusFilter, setStatusFilter] = useState<string>('');
  const [page, setPage] = useState(1);
  const [cancellingId, setCancellingId] = useState<number | null>(null);

  const load = useCallback(async () => {
    setLoading(true);
    setError('');
    try {
      const rows = await bookingsApi.getAll({
        status: statusFilter || undefined,
        pageNumber: page,
        pageSize: PAGE_SIZE,
      });
      setBookings(rows);
    } catch (err) {
      setError(extractErrorMessage(err) || AR.bookings.failedLoad);
      setBookings([]);
    } finally {
      setLoading(false);
    }
  }, [statusFilter, page]);

  useEffect(() => {
    load();
  }, [load]);

  // Client-side text search across the current page only — passenger name,
  // booking ref, train, and route are the things admins actually search for.
  const filtered = useMemo(() => {
    const q = searchTerm.trim().toLowerCase();
    if (!q) return bookings;
    return bookings.filter((b) => {
      const haystacks = [
        b.bookingRef,
        b.passenger?.fullNameAr,
        b.passenger?.fullNameEn,
        b.passenger?.idNumber,
        b.trainName,
        b.routeName,
        b.boardingStationName,
        b.alightingStationName,
      ];
      return haystacks.some((v) => v && v.toString().toLowerCase().includes(q));
    });
  }, [bookings, searchTerm]);

  // We can't know the total without a server count. "Has more" = current page
  // came back full, so a next page might exist.
  const hasNext = bookings.length === PAGE_SIZE;
  const hasPrev = page > 1;

  const handleCancel = async (booking: Booking) => {
    const ok = await showConfirm(AR.bookings.cancelTitle, AR.bookings.cancelText, AR.bookings.yesCancel);
    if (!ok) return;
    setCancellingId(booking.id);
    try {
      await bookingsApi.cancel(booking.id);
      await showSuccess(AR.bookings.cancelled);
      await load();
    } catch (err) {
      showError(AR.bookings.cancelFailed, extractErrorMessage(err));
    } finally {
      setCancellingId(null);
    }
  };

  const statusOptions = [
    { value: '', label: AR.bookings.allStatuses },
    ...BOOKING_STATUSES.map((s) => ({ value: s, label: AR.status[s] || s })),
  ];

  return (
    <div>
      <div className="mb-8 flex items-center justify-between">
        <div>
          <h1 className="text-3xl font-bold text-gray-900">{AR.bookings.title}</h1>
          <p className="text-gray-600 mt-2">{AR.bookings.subtitle}</p>
        </div>
        <button
          onClick={load}
          disabled={loading}
          className="admin-button-secondary flex items-center gap-2 disabled:opacity-50"
        >
          <RefreshCw size={18} className={loading ? 'animate-spin' : ''} />
          {AR.common.search}
        </button>
      </div>

      <div className="admin-card mb-6">
        <div className="flex flex-col md:flex-row gap-4 md:items-end">
          <div className="relative flex-1">
            <Search className="absolute start-3 top-1/2 -translate-y-1/2 text-gray-400" size={20} />
            <input
              type="text"
              placeholder={AR.bookings.searchPlaceholder}
              value={searchTerm}
              onChange={(e) => setSearchTerm(e.target.value)}
              className="w-full ps-10 pe-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-admin-primary-500"
            />
          </div>
          <FilterDropdown
            label={AR.bookings.status}
            value={statusFilter}
            onChange={(v) => { setStatusFilter(String(v)); setPage(1); }}
            options={statusOptions}
            className="md:w-56"
          />
        </div>
      </div>

      <div className="admin-card">
        {loading ? (
          <div className="text-center py-12 text-gray-500">{AR.common.loading}</div>
        ) : error ? (
          <div className="p-4 bg-red-50 text-red-700 text-sm">{error}</div>
        ) : filtered.length === 0 ? (
          <div className="text-center py-12 text-gray-500">{AR.bookings.none}</div>
        ) : (
          <div className="overflow-x-auto">
            <table className="w-full">
              <thead className="bg-gray-50 border-b border-gray-200">
                <tr>
                  <th className="px-4 py-3 text-start text-xs font-medium text-gray-500 uppercase tracking-wider">{AR.bookings.bookingRef}</th>
                  <th className="px-4 py-3 text-start text-xs font-medium text-gray-500 uppercase tracking-wider">{AR.bookings.passenger}</th>
                  <th className="px-4 py-3 text-start text-xs font-medium text-gray-500 uppercase tracking-wider">{AR.bookings.trip}</th>
                  <th className="px-4 py-3 text-start text-xs font-medium text-gray-500 uppercase tracking-wider">{AR.bookings.segment}</th>
                  <th className="px-4 py-3 text-start text-xs font-medium text-gray-500 uppercase tracking-wider">{AR.bookings.coachClass}</th>
                  <th className="px-4 py-3 text-start text-xs font-medium text-gray-500 uppercase tracking-wider">{AR.bookings.seat}</th>
                  <th className="px-4 py-3 text-start text-xs font-medium text-gray-500 uppercase tracking-wider">{AR.bookings.status}</th>
                  <th className="px-4 py-3 text-start text-xs font-medium text-gray-500 uppercase tracking-wider">{AR.bookings.total}</th>
                  <th className="px-4 py-3 text-start text-xs font-medium text-gray-500 uppercase tracking-wider">{AR.bookings.createdAt}</th>
                  <th className="px-4 py-3 text-start text-xs font-medium text-gray-500 uppercase tracking-wider">{AR.common.actions}</th>
                </tr>
              </thead>
              <tbody className="bg-white divide-y divide-gray-200">
                {filtered.map((b) => {
                  const canCancel = b.status !== 'Cancelled' && b.status !== 'Completed';
                  return (
                    <tr key={b.id} className="hover:bg-gray-50">
                      <td className="px-4 py-3 whitespace-nowrap font-medium text-gray-900">{b.bookingRef}</td>
                      <td className="px-4 py-3 whitespace-nowrap">
                        <div className="text-gray-900">{b.passenger?.fullNameAr || b.passenger?.fullNameEn}</div>
                        <div className="text-xs text-gray-500">{b.passenger?.idNumber}</div>
                      </td>
                      <td className="px-4 py-3 whitespace-nowrap">
                        <div className="text-gray-900">{b.trainName}</div>
                        <div className="text-xs text-gray-500">{b.routeName}</div>
                      </td>
                      <td className="px-4 py-3 whitespace-nowrap">
                        <div className="text-gray-900">{b.boardingStationName} → {b.alightingStationName}</div>
                        <div className="text-xs text-gray-500">{formatTime(b.departureTime)} - {formatTime(b.arrivalTime)}</div>
                      </td>
                      <td className="px-4 py-3 whitespace-nowrap text-gray-600">{b.coachClass}</td>
                      <td className="px-4 py-3 whitespace-nowrap text-gray-600">{b.seatNumber}</td>
                      <td className="px-4 py-3 whitespace-nowrap">
                        <span className={`px-3 py-1 text-xs rounded-full ${statusBadgeClass(b.status)}`}>
                          {AR.status[b.status] || b.status}
                        </span>
                      </td>
                      <td className="px-4 py-3 whitespace-nowrap font-medium text-gray-900">{Math.round(b.total)} {b.currency === 'SDG' ? 'جنيه' : b.currency}</td>
                      <td className="px-4 py-3 whitespace-nowrap text-xs text-gray-500">{formatDateTime(b.createdAt)}</td>
                      <td className="px-4 py-3 whitespace-nowrap">
                        {canCancel ? (
                          <button
                            onClick={() => handleCancel(b)}
                            disabled={cancellingId === b.id}
                            className="inline-flex items-center gap-1 px-2 py-1 text-xs border border-red-300 text-red-600 rounded hover:bg-red-50 disabled:opacity-50"
                          >
                            <X size={14} />
                            {AR.bookings.cancelAction}
                          </button>
                        ) : (
                          <span className="text-xs text-gray-400">—</span>
                        )}
                      </td>
                    </tr>
                  );
                })}
              </tbody>
            </table>
          </div>
        )}

        {!loading && !error && (hasPrev || hasNext) && (
          <div className="flex items-center justify-between px-4 py-3 border-t border-gray-200">
            <span className="text-sm text-gray-600">صفحة {page}</span>
            <div className="flex items-center gap-2">
              <button
                onClick={() => setPage((p) => Math.max(1, p - 1))}
                disabled={!hasPrev}
                className="p-1 rounded hover:bg-gray-100 disabled:opacity-50 disabled:cursor-not-allowed"
              >
                <ChevronRight size={20} />
              </button>
              <button
                onClick={() => setPage((p) => p + 1)}
                disabled={!hasNext}
                className="p-1 rounded hover:bg-gray-100 disabled:opacity-50 disabled:cursor-not-allowed"
              >
                <ChevronLeft size={20} />
              </button>
            </div>
          </div>
        )}
      </div>
    </div>
  );
};

export default BookingsPage;
