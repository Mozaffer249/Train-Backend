import { useState, useEffect, useCallback } from 'react';
import { Calendar, X, Clock, MapPin, Train as TrainIcon, QrCode, AlertTriangle, Loader2 } from 'lucide-react';
import QRCode from 'react-qr-code';
import { useLanguage } from '../contexts/LanguageContext';
import { useAuth } from '../contexts/AuthContext';
import { bookingApi } from '../services/bookingApi';
import type { BookingDto } from '../types/api';

function formatDate(iso: string): string {
  const d = new Date(iso);
  return isNaN(d.getTime()) ? '--' : d.toLocaleDateString();
}
function formatTime(iso: string): string {
  const d = new Date(iso);
  return isNaN(d.getTime()) ? '--:--' : d.toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' });
}

// "Upcoming" = the trip hasn't departed yet AND the booking is still alive.
// We use the segment's departureTime — past trips fall off the upcoming list
// even if the booking is still Pending/Confirmed.
function isUpcoming(b: BookingDto): boolean {
  if (b.status === 'Cancelled' || b.status === 'Completed') return false;
  const dep = new Date(b.departureTime).getTime();
  return !isNaN(dep) && dep > Date.now();
}

export default function Dashboard() {
  const { t } = useLanguage();
  const { user } = useAuth();
  const [activeTab, setActiveTab] = useState<'upcoming' | 'past'>('upcoming');
  const [bookings, setBookings] = useState<BookingDto[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [selected, setSelected] = useState<BookingDto | null>(null);
  // Cancellation confirmation modal: booking the user is about to cancel,
  // optional reason text, and submit state.
  const [cancelTarget, setCancelTarget] = useState<BookingDto | null>(null);
  const [cancelReason, setCancelReason] = useState('');
  const [cancelling, setCancelling] = useState(false);
  const [cancelError, setCancelError] = useState('');

  const load = useCallback(() => {
    setLoading(true);
    setError('');
    bookingApi
      .getMyBookings()
      .then((rows) => setBookings(rows))
      .catch((err: unknown) => setError(err instanceof Error ? err.message : t('error')))
      .finally(() => setLoading(false));
  }, [t]);

  useEffect(() => {
    load();
  }, [load]);

  const upcoming = bookings.filter(isUpcoming);
  const past = bookings.filter((b) => !isUpcoming(b));
  const shown = activeTab === 'upcoming' ? upcoming : past;

  // Cancel button on the booking card just opens the confirmation modal —
  // the actual API call happens after the user confirms.
  const promptCancel = (booking: BookingDto) => {
    setCancelTarget(booking);
    setCancelReason('');
    setCancelError('');
  };

  const closeCancelModal = () => {
    if (cancelling) return; // don't close mid-request
    setCancelTarget(null);
    setCancelReason('');
    setCancelError('');
  };

  const confirmCancel = async () => {
    if (!cancelTarget) return;
    setCancelling(true);
    setCancelError('');
    try {
      await bookingApi.cancelBooking(cancelTarget.id, cancelReason.trim() || undefined);
      setCancelTarget(null);
      setCancelReason('');
      load();
    } catch (err) {
      setCancelError(err instanceof Error ? err.message : t('error'));
    } finally {
      setCancelling(false);
    }
  };

  // Backend stores coach class as the human label ("First"/"Second"/"Third").
  const classBadge = (coachClass: string) => {
    const c = (coachClass || '').toLowerCase();
    if (c === 'first') return 'bg-yellow-100 text-yellow-800';
    if (c === 'second') return 'bg-sudan-green-100 text-sudan-green-800';
    return 'bg-gray-100 text-gray-800';
  };

  const classLabel = (coachClass: string) => {
    const c = (coachClass || '').toLowerCase();
    if (c === 'first') return t('first.class') || coachClass;
    if (c === 'second') return t('second.class') || coachClass;
    if (c === 'third') return t('third.class') || coachClass;
    return coachClass;
  };

  return (
    <div className="min-h-screen bg-gray-50 py-8">
      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <div className="mb-8">
          <h1 className="text-3xl font-bold text-gray-900 mb-2">{t('my.trips')}</h1>
          <p className="text-gray-600">{t('welcome.back')}, {user?.name}</p>
        </div>

        {error && (
          <div className="mb-4 bg-red-50 text-red-600 text-sm rounded-lg p-3">{error}</div>
        )}

        <div className="mb-8 border-b border-gray-200">
          <nav className="-mb-px flex space-x-8 rtl:space-x-reverse">
            {(['upcoming', 'past'] as const).map((tab) => (
              <button
                key={tab}
                onClick={() => setActiveTab(tab)}
                className={`py-4 px-1 border-b-2 font-medium text-sm ${
                  activeTab === tab ? 'border-sudan-green-700 text-sudan-green-800' : 'border-transparent text-gray-500 hover:text-gray-700 hover:border-gray-300'
                }`}
              >
                {t(tab)} ({tab === 'upcoming' ? upcoming.length : past.length})
              </button>
            ))}
          </nav>
        </div>

        {loading ? (
          <div className="text-center py-12">
            <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-sudan-green-600 mx-auto mb-4"></div>
            <p className="text-gray-600">{t('loading')}</p>
          </div>
        ) : shown.length === 0 ? (
          <div className="text-center py-12">
            <TrainIcon className="h-12 w-12 text-gray-400 mx-auto mb-4" />
            <p className="text-gray-500">{activeTab === 'upcoming' ? t('no.upcoming.trips') : t('no.past.trips')}</p>
          </div>
        ) : (
          <div className="grid grid-cols-1 lg:grid-cols-2 xl:grid-cols-3 gap-6">
            {shown.map((b) => (
              <div key={b.id} className="bg-white rounded-lg shadow-md hover:shadow-lg transition-shadow">
                <div className="p-6">
                  <div className="flex items-center justify-between mb-4">
                    <div>
                      <h3 className="text-lg font-semibold text-gray-900">{b.trainName}</h3>
                      <p className="text-sm text-gray-600">{b.bookingRef}</p>
                    </div>
                    {b.status === 'Cancelled' ? (
                      <span className="px-2 py-1 rounded-full text-xs font-medium bg-red-100 text-red-700">{t('cancelled')}</span>
                    ) : b.status === 'Completed' ? (
                      <span className="px-2 py-1 rounded-full text-xs font-medium bg-blue-100 text-blue-700">{t('completed') || 'Completed'}</span>
                    ) : (
                      <span className={`px-2 py-1 rounded-full text-xs font-medium ${classBadge(b.coachClass)}`}>{classLabel(b.coachClass)}</span>
                    )}
                  </div>
                  <div className="space-y-3 mb-4">
                    <div className="flex items-center gap-3">
                      <MapPin className="h-4 w-4 text-gray-400" />
                      <span className="text-sm text-gray-600">{b.boardingStationName} → {b.alightingStationName}</span>
                    </div>
                    <div className="flex items-center gap-3">
                      <Calendar className="h-4 w-4 text-gray-400" />
                      <span className="text-sm text-gray-600">{formatDate(b.departureTime)}</span>
                    </div>
                    <div className="flex items-center gap-3">
                      <Clock className="h-4 w-4 text-gray-400" />
                      <span className="text-sm text-gray-600">{formatTime(b.departureTime)} - {formatTime(b.arrivalTime)}</span>
                    </div>
                    {b.passengers && b.passengers.length > 1 ? (
                      <div className="flex items-center gap-3">
                        <TrainIcon className="h-4 w-4 text-gray-400" />
                        <span className="text-sm text-gray-600">
                          {b.passengers.length} {t('passengers')} · {t('seats')} {b.passengers.map((p) => p.seatNumber).join(' · ')}
                        </span>
                      </div>
                    ) : (b.seatNumber && (
                      <div className="flex items-center gap-3">
                        <TrainIcon className="h-4 w-4 text-gray-400" />
                        <span className="text-sm text-gray-600">{t('seat')} {b.seatNumber}</span>
                      </div>
                    ))}
                  </div>
                  <div className="flex items-center justify-between pt-4 border-t border-gray-200">
                    <span className="text-lg font-bold text-gray-900">{Math.round(b.total)} {b.currency === 'SDG' ? t('sdg') : b.currency}</span>
                    <div className="flex gap-2">
                      <button onClick={() => setSelected(b)} className="bg-sudan-green-600 text-white px-3 py-1 rounded text-sm hover:bg-sudan-green-700 flex items-center gap-1">
                        <QrCode className="h-4 w-4" />
                        {t('e.ticket')}
                      </button>
                      {isUpcoming(b) && (
                        <button
                          onClick={() => promptCancel(b)}
                          title={t('cancel.booking')}
                          className="border border-red-300 text-red-600 px-3 py-1 rounded text-sm hover:bg-red-50"
                        >
                          <X className="h-4 w-4" />
                        </button>
                      )}
                    </div>
                  </div>
                </div>
              </div>
            ))}
          </div>
        )}

        {selected && (
          <div className="fixed inset-0 bg-black/50 flex items-center justify-center z-50 p-4 overflow-y-auto">
            <div className="bg-white rounded-lg max-w-lg w-full p-6 my-8">
              <div className="flex items-center justify-between mb-4">
                <h3 className="text-xl font-bold text-gray-900">{t('e.ticket')}</h3>
                <button onClick={() => setSelected(null)} className="text-gray-400 hover:text-gray-600">
                  <X className="h-6 w-6" />
                </button>
              </div>

              {/* Shared booking metadata */}
              <div className="bg-gray-50 rounded-lg p-4 space-y-1 text-sm mb-4">
                <div className="flex justify-between"><span className="text-gray-600">{t('booking.ref')}</span><span className="font-medium">{selected.bookingRef}</span></div>
                <div className="flex justify-between"><span className="text-gray-600">{t('train')}</span><span className="font-medium">{selected.trainName}</span></div>
                <div className="flex justify-between"><span className="text-gray-600">{t('route')}</span><span className="font-medium">{selected.boardingStationName} → {selected.alightingStationName}</span></div>
                <div className="flex justify-between"><span className="text-gray-600">{t('date')}</span><span className="font-medium">{formatDate(selected.departureTime)}</span></div>
                <div className="flex justify-between"><span className="text-gray-600">{t('departure')}</span><span className="font-medium">{formatTime(selected.departureTime)}</span></div>
              </div>

              {/* One ticket per passenger */}
              <div className="space-y-4">
                {(selected.passengers && selected.passengers.length > 0
                  ? selected.passengers
                  : [{ passenger: selected.passenger, seatNumber: selected.seatNumber, coachClass: selected.coachClass, price: selected.total, ticket: selected.ticket }]
                ).map((pd, i) => (
                  <div key={i} className="border border-gray-200 rounded-lg p-4">
                    <div className="flex flex-col sm:flex-row gap-4 items-center">
                      <div className="bg-white border border-gray-200 rounded p-2 flex-shrink-0">
                        <QRCode
                          value={
                            pd.ticket?.qrPayload ||
                            JSON.stringify({
                              ref: selected.bookingRef,
                              trip: selected.tripId,
                              from: selected.boardingStationName,
                              to: selected.alightingStationName,
                              departure: selected.departureTime,
                              seat: pd.seatNumber,
                            })
                          }
                          size={100}
                          bgColor="#ffffff"
                          fgColor="#064e2a"
                          level="M"
                        />
                      </div>
                      <div className="flex-1 text-center sm:text-start text-sm">
                        <p className="text-xs text-gray-500 mb-1">{t('passenger')} {i + 1}</p>
                        <p className="font-semibold text-gray-900">
                          {pd.passenger?.fullNameAr || pd.passenger?.fullNameEn}
                        </p>
                        <p className="text-xs text-gray-500">{pd.passenger?.idNumber}</p>
                        <p className="mt-2">
                          {t('seat')} <span className="font-medium">{pd.seatNumber}</span>
                          <span className="text-gray-400"> · </span>
                          {classLabel(pd.coachClass)}
                        </p>
                        {pd.ticket?.ticketNumber && (
                          <p className="text-[11px] text-gray-500 mt-1">{pd.ticket.ticketNumber}</p>
                        )}
                      </div>
                    </div>
                  </div>
                ))}
              </div>

              {selected.breakdown && (
                <div className="bg-white border border-gray-200 rounded-lg p-4 space-y-1 text-sm mt-4">
                  <div className="flex justify-between"><span className="text-gray-600">{t('ticket.price')} × {selected.passengers?.length ?? 1}</span><span>{Math.round(selected.breakdown.total)} {t('sdg')}</span></div>
                  <div className="flex justify-between font-bold border-t pt-1 mt-1">
                    <span>{t('total')}</span>
                    <span>{Math.round(selected.total)} {t('sdg')}</span>
                  </div>
                </div>
              )}
              <p className="text-xs text-gray-500 mt-3 text-center">{t('scan.qr.code')}</p>
            </div>
          </div>
        )}

        {/* Cancellation confirmation dialog. Renders on top of the e-ticket
            modal if both were somehow open (z-index higher) and traps focus
            in the destructive flow until the user explicitly opts in. */}
        {cancelTarget && (
          <div
            className="fixed inset-0 bg-black/60 flex items-center justify-center z-[60] p-4"
            onClick={closeCancelModal}
            role="dialog"
            aria-modal="true"
          >
            <div className="bg-white rounded-lg max-w-md w-full p-6" onClick={(e) => e.stopPropagation()}>
              <div className="flex items-start gap-3 mb-4">
                <div className="flex-shrink-0 w-10 h-10 rounded-full bg-red-100 flex items-center justify-center">
                  <AlertTriangle className="h-5 w-5 text-red-600" />
                </div>
                <div className="flex-1">
                  <h3 className="text-lg font-bold text-gray-900">{t('cancel.booking.title')}</h3>
                  <p className="text-sm text-gray-600 mt-1">{t('cancel.booking.warning')}</p>
                </div>
              </div>

              {/* Booking-being-cancelled summary so the user can sanity-check
                  before destroying a real booking. */}
              <div className="bg-gray-50 rounded-lg p-3 mb-4 space-y-1 text-sm">
                <div className="flex justify-between"><span className="text-gray-600">{t('booking.ref')}</span><span className="font-medium">{cancelTarget.bookingRef}</span></div>
                <div className="flex justify-between"><span className="text-gray-600">{t('train')}</span><span className="font-medium">{cancelTarget.trainName}</span></div>
                <div className="flex justify-between"><span className="text-gray-600">{t('route')}</span><span className="font-medium">{cancelTarget.boardingStationName} → {cancelTarget.alightingStationName}</span></div>
                <div className="flex justify-between"><span className="text-gray-600">{t('date')}</span><span className="font-medium">{formatDate(cancelTarget.departureTime)} · {formatTime(cancelTarget.departureTime)}</span></div>
                {cancelTarget.passengers && cancelTarget.passengers.length > 0 && (
                  <div className="flex justify-between">
                    <span className="text-gray-600">{t('seats')}</span>
                    <span className="font-medium">{cancelTarget.passengers.map((p) => p.seatNumber).join(' · ')}</span>
                  </div>
                )}
              </div>

              <div className="mb-4">
                <label className="block text-sm font-medium text-gray-700 mb-2">
                  {t('cancel.reason.label')} <span className="text-gray-400 text-xs">({t('optional')})</span>
                </label>
                <textarea
                  value={cancelReason}
                  onChange={(e) => setCancelReason(e.target.value.slice(0, 500))}
                  rows={3}
                  maxLength={500}
                  placeholder={t('cancel.reason.placeholder')}
                  className="w-full px-3 py-2 border border-gray-300 rounded-lg text-sm focus:ring-2 focus:ring-sudan-green-500 focus:border-sudan-green-500"
                />
              </div>

              {cancelError && (
                <div className="mb-4 bg-red-50 text-red-700 text-sm rounded-lg p-3">{cancelError}</div>
              )}

              <div className="flex flex-col-reverse sm:flex-row gap-2 sm:gap-3 sm:justify-end">
                <button
                  type="button"
                  onClick={closeCancelModal}
                  disabled={cancelling}
                  className="px-4 py-2 border border-gray-300 text-gray-700 rounded-lg hover:bg-gray-50 disabled:opacity-50 text-sm"
                >
                  {t('cancel.booking.keep')}
                </button>
                <button
                  type="button"
                  onClick={confirmCancel}
                  disabled={cancelling}
                  className="px-4 py-2 bg-red-600 text-white rounded-lg hover:bg-red-700 disabled:opacity-60 flex items-center justify-center gap-2 text-sm font-medium"
                >
                  {cancelling && <Loader2 className="h-4 w-4 animate-spin" />}
                  {cancelling ? t('processing') : t('cancel.booking.confirm')}
                </button>
              </div>
            </div>
          </div>
        )}
      </div>
    </div>
  );
}
