import { useState, useEffect } from 'react';
import { X } from 'lucide-react';
import { Trip, Train, Route, TRIP_STATUSES } from '../../types/infrastructure';
import { tripsApi, trainsApi, routesApi } from '../../services/api';
import { showSuccess, showError, extractErrorMessage } from '../../utils/alerts';

interface TripModalProps {
  isOpen: boolean;
  onClose: () => void;
  onSuccess: () => void;
  trip?: Trip | null;
}

function toLocalInput(iso?: string): string {
  const d = iso ? new Date(iso) : new Date();
  if (isNaN(d.getTime())) return '';
  const pad = (n: number) => String(n).padStart(2, '0');
  return `${d.getFullYear()}-${pad(d.getMonth() + 1)}-${pad(d.getDate())}T${pad(d.getHours())}:${pad(d.getMinutes())}`;
}

const TripModal = ({ isOpen, onClose, onSuccess, trip }: TripModalProps) => {
  const isEdit = !!trip;
  const [trains, setTrains] = useState<Train[]>([]);
  const [routes, setRoutes] = useState<Route[]>([]);
  const [trainId, setTrainId] = useState<number | ''>('');
  const [routeId, setRouteId] = useState<number | ''>('');
  const [departure, setDeparture] = useState('');
  const [arrival, setArrival] = useState('');
  const [status, setStatus] = useState<string>('Scheduled');
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [error, setError] = useState('');

  useEffect(() => {
    if (!isOpen) return;
    setError('');
    if (trip) {
      setTrainId(trip.trainId);
      setRouteId(trip.routeId);
      setDeparture(toLocalInput(trip.departureTime));
      setArrival(toLocalInput(trip.arrivalTime));
      setStatus(trip.status || 'Scheduled');
    } else {
      setTrainId('');
      setRouteId('');
      setDeparture('');
      setArrival('');
      setStatus('Scheduled');
    }
    if (!trip) {
      trainsApi.getAll().then(setTrains).catch(() => setTrains([]));
      routesApi.getAll({ isActive: true, pageSize: 1000 }).then(setRoutes).catch(() => setRoutes([]));
    }
  }, [isOpen, trip]);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError('');
    setIsSubmitting(true);
    try {
      // The datetime-local input gives a local-naive ISO string like
      // "2024-06-15T06:00". Send it as-is so the backend stores the value the
      // user actually picked. Using `new Date(...).toISOString()` here would
      // shift the time by the user's UTC offset (e.g. 06:00 → 03:00 for UTC+3).
      if (isEdit && trip) {
        await tripsApi.update(trip.id, {
          departureTime: departure,
          arrivalTime: arrival,
          status,
        });
        showSuccess('تم تحديث الرحلة');
      } else {
        if (!trainId || !routeId) {
          setError('يرجى اختيار قطار ومسار');
          setIsSubmitting(false);
          return;
        }
        await tripsApi.create({
          trainId: Number(trainId),
          routeId: Number(routeId),
          departureTime: departure,
          arrivalTime: arrival,
        });
        showSuccess('تم إنشاء الرحلة');
      }
      onSuccess();
      onClose();
    } catch (err) {
      const msg = extractErrorMessage(err);
      setError(msg);
      showError('تعذّر حفظ الرحلة', msg);
    } finally {
      setIsSubmitting(false);
    }
  };

  if (!isOpen) return null;

  const inputClass = 'w-full px-3 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-admin-primary-500';

  return (
    <div className="fixed inset-0 z-50 overflow-y-auto">
      <div className="flex items-center justify-center min-h-screen px-4 pt-4 pb-20 text-center sm:block sm:p-0">
        <div className="fixed inset-0 transition-opacity bg-gray-500 bg-opacity-75" onClick={onClose} />
        <div className="inline-block align-bottom bg-white rounded-lg text-left overflow-hidden shadow-xl transform transition-all sm:my-8 sm:align-middle sm:max-w-lg sm:w-full">
          <div className="bg-white px-6 pt-5 pb-4">
            <div className="flex items-center justify-between mb-4">
              <h3 className="text-lg font-medium text-gray-900">{isEdit ? 'تعديل رحلة' : 'إضافة رحلة'}</h3>
              <button onClick={onClose} className="text-gray-400 hover:text-gray-500">
                <X size={24} />
              </button>
            </div>

            {error && <div className="mb-4 p-3 bg-red-50 border border-red-200 rounded-lg text-red-700 text-sm">{error}</div>}

            <form onSubmit={handleSubmit} className="space-y-4">
              {isEdit ? (
                <div className="p-3 bg-gray-50 rounded-lg text-sm text-gray-700">
                  <div><span className="font-medium">القطار:</span> {trip?.trainName}</div>
                  <div><span className="font-medium">المسار:</span> {trip?.routeName}</div>
                </div>
              ) : (
                <>
                  <div>
                    <label className="block text-sm font-medium text-gray-700 mb-1">القطار *</label>
                    <select value={trainId} onChange={(e) => setTrainId(Number(e.target.value) || '')} required className={inputClass}>
                      <option value="">اختر قطاراً…</option>
                      {trains.map((tr) => (
                        <option key={tr.id} value={tr.id}>{tr.trainNumber} — {tr.nameEn}</option>
                      ))}
                    </select>
                  </div>
                  <div>
                    <label className="block text-sm font-medium text-gray-700 mb-1">المسار *</label>
                    <select value={routeId} onChange={(e) => setRouteId(Number(e.target.value) || '')} required className={inputClass}>
                      <option value="">اختر مساراً…</option>
                      {routes.map((r) => (
                        <option key={r.id} value={r.id}>{r.nameEn} ({r.origin?.nameEn} → {r.destination?.nameEn})</option>
                      ))}
                    </select>
                  </div>
                </>
              )}

              <div className="grid grid-cols-2 gap-4">
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">المغادرة *</label>
                  <input type="datetime-local" value={departure} onChange={(e) => setDeparture(e.target.value)} required className={inputClass} />
                </div>
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">الوصول *</label>
                  <input type="datetime-local" value={arrival} onChange={(e) => setArrival(e.target.value)} required className={inputClass} />
                </div>
              </div>

              {isEdit && (
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">الحالة *</label>
                  <select value={status} onChange={(e) => setStatus(e.target.value)} className={inputClass}>
                    {TRIP_STATUSES.map((s) => (
                      <option key={s} value={s}>{s}</option>
                    ))}
                  </select>
                </div>
              )}

              <div className="flex justify-end gap-3 pt-4 border-t">
                <button type="button" onClick={onClose} disabled={isSubmitting} className="px-4 py-2 text-gray-700 bg-white border border-gray-300 rounded-lg hover:bg-gray-50">إلغاء</button>
                <button type="submit" disabled={isSubmitting} className="admin-button">
                  {isSubmitting ? 'جاري الحفظ…' : isEdit ? 'حفظ التغييرات' : 'إنشاء رحلة'}
                </button>
              </div>
            </form>
          </div>
        </div>
      </div>
    </div>
  );
};

export default TripModal;
