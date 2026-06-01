import { useState, useEffect } from 'react';
import { X } from 'lucide-react';
import { Fare, FareFormData, CoachClass, Trip } from '../../types/infrastructure';
import { faresApi, routesApi, stationsApi, tripsApi } from '../../services/api';
import { Route } from '../../types/infrastructure';
import { Station } from '../../types/geography';

interface FareModalProps {
  isOpen: boolean;
  onClose: () => void;
  onSuccess: () => void;
  // When supplied the modal opens in edit mode; scope selectors are locked.
  editFare?: Fare | null;
  // When supplied the modal opens scoped to a specific trip — scope radios are
  // locked to 'trip', the trip dropdown is pre-filled and locked. Used by the
  // TripsPage "Assign Fare" per-row button.
  pinnedTrip?: Trip | null;
}

function formatTripLabel(trip: Trip): string {
  const dep = new Date(trip.departureTime);
  const when = isNaN(dep.getTime())
    ? '--'
    : dep.toLocaleString('ar', { dateStyle: 'short', timeStyle: 'short' });
  return `${trip.trainName} · ${trip.routeName} · ${when}`;
}

type PricingScope = 'route' | 'segment' | 'trip';

const EMPTY_FORM: FareFormData = {
  routeId: undefined,
  originStationId: undefined,
  destinationStationId: undefined,
  tripId: undefined,
  coachClass: CoachClass.Second,
  basePrice: 0,
  discountPercent: undefined,
};

function classNameToId(name: string): number {
  switch (name) {
    case 'First': return CoachClass.First;
    case 'Third': return CoachClass.Third;
    default: return CoachClass.Second;
  }
}

const FareModal = ({ isOpen, onClose, onSuccess, editFare, pinnedTrip }: FareModalProps) => {
  const [routes, setRoutes] = useState<Route[]>([]);
  const [stations, setStations] = useState<Station[]>([]);
  const [trips, setTrips] = useState<Trip[]>([]);
  const [pricingScope, setPricingScope] = useState<PricingScope>('route');
  const [formData, setFormData] = useState<FareFormData>(EMPTY_FORM);
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [error, setError] = useState('');

  const isEdit = !!editFare;
  // Lock scope inputs when editing OR when the modal is pinned to a specific trip.
  const scopeLocked = isEdit || !!pinnedTrip;

  useEffect(() => {
    if (!isOpen) return;
    loadRoutes();
    loadStations();
    loadTrips();
    if (editFare) {
      const scope: PricingScope = editFare.tripId
        ? 'trip'
        : editFare.originStationId
          ? 'segment'
          : 'route';
      setPricingScope(scope);
      setFormData({
        routeId: editFare.routeId ?? undefined,
        originStationId: editFare.originStationId ?? undefined,
        destinationStationId: editFare.destinationStationId ?? undefined,
        tripId: editFare.tripId ?? undefined,
        coachClass: classNameToId(editFare.coachClass),
        basePrice: editFare.basePrice,
        discountPercent: editFare.discountPercent ?? undefined,
        effectiveFrom: editFare.effectiveFrom,
        effectiveTo: editFare.effectiveTo,
      });
    } else if (pinnedTrip) {
      setPricingScope('trip');
      setFormData({ ...EMPTY_FORM, tripId: pinnedTrip.id });
    } else {
      setFormData(EMPTY_FORM);
      setPricingScope('route');
    }
    setError('');
  }, [isOpen, editFare, pinnedTrip]);

  const loadRoutes = async () => {
    try {
      const data = await routesApi.getAll({ isActive: true, pageSize: 1000 });
      setRoutes(data);
    } catch (error) {
      console.error('Failed to load routes:', error);
    }
  };

  const loadStations = async () => {
    try {
      const data = await stationsApi.getAll({ isActive: true, pageSize: 1000 });
      setStations(data);
    } catch (error) {
      console.error('Failed to load stations:', error);
    }
  };

  const loadTrips = async () => {
    try {
      const data = await tripsApi.getAll();
      setTrips(data);
    } catch (error) {
      console.error('Failed to load trips:', error);
    }
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError('');

    if (!isEdit) {
      if (pricingScope === 'route' && !formData.routeId) {
        setError('يرجى اختيار مسار');
        return;
      }
      if (pricingScope === 'segment' && (!formData.originStationId || !formData.destinationStationId)) {
        setError('يرجى اختيار محطتي الانطلاق والوصول');
        return;
      }
      if (pricingScope === 'segment' && formData.originStationId === formData.destinationStationId) {
        setError('يجب أن تكون محطتا الانطلاق والوصول مختلفتين');
        return;
      }
      if (pricingScope === 'trip' && !formData.tripId) {
        setError('يرجى اختيار رحلة');
        return;
      }
    }

    if (formData.basePrice <= 0) {
      setError('يجب أن يكون السعر الأساسي أكبر من الصفر');
      return;
    }

    setIsSubmitting(true);

    try {
      if (isEdit && editFare) {
        // PATCH: only pricing fields. Scope columns excluded.
        await faresApi.update(editFare.id, {
          basePrice: formData.basePrice,
          discountPercent: formData.discountPercent,
          effectiveTo: formData.effectiveTo,
        });
      } else {
        const submitData: FareFormData = {
          ...formData,
          routeId: pricingScope === 'route' ? formData.routeId : undefined,
          originStationId: pricingScope === 'segment' ? formData.originStationId : undefined,
          destinationStationId: pricingScope === 'segment' ? formData.destinationStationId : undefined,
          tripId: pricingScope === 'trip' ? formData.tripId : undefined,
        };
        await faresApi.create(submitData);
      }
      onSuccess();
      onClose();
    } catch (error: any) {
      setError(error.message || (isEdit ? 'تعذّر تحديث السعر' : 'تعذّر إنشاء السعر'));
    } finally {
      setIsSubmitting(false);
    }
  };

  // Simple base − discount = total preview.
  const preview = (() => {
    const discountPct = formData.discountPercent || 0;
    const discountAmount = formData.basePrice * discountPct / 100;
    const total = formData.basePrice - discountAmount;
    return { discountAmount, total };
  })();

  if (!isOpen) return null;

  const fieldClass = 'w-full px-3 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-admin-primary-500';
  const lockedClass = `${fieldClass} bg-gray-50 text-gray-500 cursor-not-allowed`;

  return (
    <div className="fixed inset-0 z-50 overflow-y-auto">
      <div className="flex items-center justify-center min-h-screen px-4 pt-4 pb-20 text-center sm:block sm:p-0">
        <div className="fixed inset-0 transition-opacity bg-gray-500 bg-opacity-75" onClick={onClose} />

        <div className="inline-block align-bottom bg-white rounded-lg text-left overflow-hidden shadow-xl transform transition-all sm:my-8 sm:align-middle sm:max-w-2xl sm:w-full">
          <div className="bg-white px-6 pt-5 pb-4">
            <div className="flex items-center justify-between mb-4">
              <h3 className="text-lg font-medium text-gray-900">
                {isEdit
                  ? 'تعديل السعر'
                  : pinnedTrip
                    ? `إنشاء سعر للرحلة — ${pinnedTrip.trainName}`
                    : 'إنشاء سعر جديد'}
              </h3>
              <button onClick={onClose} className="text-gray-400 hover:text-gray-500">
                <X size={24} />
              </button>
            </div>

            {error && (
              <div className="mb-4 p-3 bg-red-50 border border-red-200 rounded-lg text-red-700 text-sm">
                {error}
              </div>
            )}

            {isEdit && (
              <div className="mb-4 p-3 bg-amber-50 border border-amber-200 rounded-lg text-amber-800 text-xs">
                لا يمكن تغيير نطاق السعر (المسار / المقطع / الرحلة / فئة العربة) بعد الإنشاء. إن كان النطاق خاطئاً، أنهِ صلاحية السعر الحالي وأنشئ سعراً جديداً.
              </div>
            )}

            <form onSubmit={handleSubmit} className="space-y-4">
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-2">نطاق التسعير</label>
                <div className="flex gap-3">
                  <label className="flex items-center gap-2 cursor-pointer">
                    <input
                      type="radio" name="scope" value="route"
                      checked={pricingScope === 'route'}
                      onChange={() => setPricingScope('route')}
                      disabled={scopeLocked}
                      className="text-admin-primary-600 focus:ring-admin-primary-500"
                    />
                    <span className="text-sm text-gray-700">على مستوى المسار</span>
                  </label>
                  <label className="flex items-center gap-2 cursor-pointer">
                    <input
                      type="radio" name="scope" value="segment"
                      checked={pricingScope === 'segment'}
                      onChange={() => setPricingScope('segment')}
                      disabled={scopeLocked}
                      className="text-admin-primary-600 focus:ring-admin-primary-500"
                    />
                    <span className="text-sm text-gray-700">خاص بقطعة</span>
                  </label>
                  <label className="flex items-center gap-2 cursor-pointer">
                    <input
                      type="radio" name="scope" value="trip"
                      checked={pricingScope === 'trip'}
                      onChange={() => setPricingScope('trip')}
                      disabled={scopeLocked}
                      className="text-admin-primary-600 focus:ring-admin-primary-500"
                    />
                    <span className="text-sm text-gray-700">خاص برحلة</span>
                  </label>
                </div>
              </div>

              {pricingScope === 'route' && (
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">المسار *</label>
                  <select
                    value={formData.routeId || ''}
                    onChange={(e) => setFormData({ ...formData, routeId: Number(e.target.value) || undefined })}
                    required={!isEdit}
                    disabled={scopeLocked}
                    className={scopeLocked ? lockedClass : fieldClass}
                  >
                    <option value="">اختر مساراً…</option>
                    {routes.map((route) => (
                      <option key={route.id} value={route.id}>
                        {route.nameEn} ({route.origin.nameEn} → {route.destination.nameEn})
                      </option>
                    ))}
                  </select>
                </div>
              )}

              {pricingScope === 'segment' && (
                <div className="grid grid-cols-2 gap-4">
                  <div>
                    <label className="block text-sm font-medium text-gray-700 mb-1">محطة الانطلاق *</label>
                    <select
                      value={formData.originStationId || ''}
                      onChange={(e) => setFormData({ ...formData, originStationId: Number(e.target.value) || undefined })}
                      required={!isEdit}
                      disabled={scopeLocked}
                      className={scopeLocked ? lockedClass : fieldClass}
                    >
                      <option value="">اختر محطة الانطلاق…</option>
                      {stations.map((station) => (
                        <option key={station.id} value={station.id}>
                          {station.nameEn} ({station.code})
                        </option>
                      ))}
                    </select>
                  </div>
                  <div>
                    <label className="block text-sm font-medium text-gray-700 mb-1">محطة الوصول *</label>
                    <select
                      value={formData.destinationStationId || ''}
                      onChange={(e) => setFormData({ ...formData, destinationStationId: Number(e.target.value) || undefined })}
                      required={!isEdit}
                      disabled={scopeLocked}
                      className={scopeLocked ? lockedClass : fieldClass}
                    >
                      <option value="">اختر محطة الوصول…</option>
                      {stations.map((station) => (
                        <option key={station.id} value={station.id}>
                          {station.nameEn} ({station.code})
                        </option>
                      ))}
                    </select>
                  </div>
                </div>
              )}

              {pricingScope === 'trip' && (
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">الرحلة *</label>
                  <select
                    value={formData.tripId || ''}
                    onChange={(e) => setFormData({ ...formData, tripId: Number(e.target.value) || undefined })}
                    required={!isEdit}
                    disabled={scopeLocked}
                    className={scopeLocked ? lockedClass : fieldClass}
                  >
                    <option value="">اختر رحلة…</option>
                    {trips.map((trip) => (
                      <option key={trip.id} value={trip.id}>
                        {formatTripLabel(trip)}
                      </option>
                    ))}
                  </select>
                  {pinnedTrip && (
                    <p className="text-xs text-gray-500 mt-1">
                      سعر خاص بهذه الرحلة فقط. سيتجاوز سعر المسار / المقطع عند الحجز.
                    </p>
                  )}
                </div>
              )}

              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">فئة العربة *</label>
                <select
                  value={formData.coachClass}
                  onChange={(e) => setFormData({ ...formData, coachClass: Number(e.target.value) })}
                  required
                  disabled={isEdit}
                  className={isEdit ? lockedClass : fieldClass}
                >
                  <option value={CoachClass.First}>الدرجة الأولى</option>
                  <option value={CoachClass.Second}>الدرجة الثانية</option>
                  <option value={CoachClass.Third}>الدرجة الثالثة</option>
                </select>
              </div>

              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">السعر الأساسي (جنيه سوداني) *</label>
                <input
                  type="number" step="0.01" min="0"
                  value={formData.basePrice}
                  onChange={(e) => setFormData({ ...formData, basePrice: Number(e.target.value) })}
                  required
                  className={fieldClass}
                />
              </div>

              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">نسبة الخصم (٪)</label>
                <input
                  type="number" step="1" min="0" max="100"
                  value={formData.discountPercent ?? ''}
                  onChange={(e) => setFormData({ ...formData, discountPercent: e.target.value ? Number(e.target.value) : undefined })}
                  className={fieldClass}
                />
              </div>

              {isEdit && (
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">سريان السعر حتى (اختياري)</label>
                  <input
                    type="datetime-local"
                    value={formData.effectiveTo ? formData.effectiveTo.slice(0, 16) : ''}
                    onChange={(e) => setFormData({ ...formData, effectiveTo: e.target.value || undefined })}
                    className={fieldClass}
                  />
                  <p className="text-xs text-gray-500 mt-1">اضبطه على تاريخ ماضٍ لإنهاء صلاحية هذا السعر.</p>
                </div>
              )}

              <div className="p-4 bg-blue-50 border border-blue-200 rounded-lg">
                <div className="text-sm font-medium text-gray-700 mb-2">معاينة السعر</div>
                <div className="space-y-1 text-sm">
                  <div className="flex justify-between">
                    <span>السعر الأساسي:</span>
                    <span className="font-medium">{formData.basePrice.toFixed(2)} SDG</span>
                  </div>
                  {preview.discountAmount > 0 && (
                    <div className="flex justify-between text-red-600">
                      <span>− خصم ({formData.discountPercent}٪):</span>
                      <span className="font-medium">{preview.discountAmount.toFixed(2)} SDG</span>
                    </div>
                  )}
                  <div className="flex justify-between font-bold text-lg pt-2 border-t border-blue-300">
                    <span>الإجمالي:</span>
                    <span className="text-admin-primary-600">{preview.total.toFixed(2)} SDG</span>
                  </div>
                </div>
              </div>

              <div className="flex justify-end gap-3 pt-4 border-t">
                <button
                  type="button"
                  onClick={onClose}
                  className="px-4 py-2 text-gray-700 bg-white border border-gray-300 rounded-lg hover:bg-gray-50"
                  disabled={isSubmitting}
                >إلغاء</button>
                <button
                  type="submit"
                  className="admin-button"
                  disabled={isSubmitting}
                >
                  {isSubmitting
                    ? (isEdit ? 'جاري الحفظ…' : 'جاري الإنشاء…')
                    : (isEdit ? 'حفظ التغييرات' : 'إنشاء سعر')}
                </button>
              </div>
            </form>
          </div>
        </div>
      </div>
    </div>
  );
};

export default FareModal;
