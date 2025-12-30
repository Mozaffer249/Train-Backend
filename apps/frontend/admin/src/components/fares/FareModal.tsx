import { useState, useEffect } from 'react';
import { X } from 'lucide-react';
import { FareFormData, CoachClass } from '../../types/infrastructure';
import { faresApi, routesApi, stationsApi } from '../../services/api';
import { Route } from '../../types/infrastructure';
import { Station } from '../../types/geography';

interface FareModalProps {
  isOpen: boolean;
  onClose: () => void;
  onSuccess: () => void;
}

type PricingScope = 'route' | 'segment' | 'trip';

const FareModal = ({ isOpen, onClose, onSuccess }: FareModalProps) => {
  const [routes, setRoutes] = useState<Route[]>([]);
  const [stations, setStations] = useState<Station[]>([]);
  const [pricingScope, setPricingScope] = useState<PricingScope>('route');
  const [formData, setFormData] = useState<FareFormData>({
    routeId: undefined,
    originStationId: undefined,
    destinationStationId: undefined,
    tripId: undefined,
    coachClass: CoachClass.Second,
    basePrice: 0,
    pricePerKm: undefined,
    vatRate: 0.15,
    discountPercent: undefined,
  });
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [error, setError] = useState('');

  useEffect(() => {
    if (isOpen) {
      loadRoutes();
      loadStations();
      resetForm();
    }
  }, [isOpen]);

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

  const resetForm = () => {
    setFormData({
      routeId: undefined,
      originStationId: undefined,
      destinationStationId: undefined,
      tripId: undefined,
      coachClass: CoachClass.Second,
      basePrice: 0,
      pricePerKm: undefined,
      vatRate: 0.15,
      discountPercent: undefined,
    });
    setPricingScope('route');
    setError('');
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError('');

    // Validate based on scope
    if (pricingScope === 'route' && !formData.routeId) {
      setError('Please select a route');
      return;
    }

    if (pricingScope === 'segment' && (!formData.originStationId || !formData.destinationStationId)) {
      setError('Please select both origin and destination stations');
      return;
    }

    if (pricingScope === 'segment' && formData.originStationId === formData.destinationStationId) {
      setError('Origin and destination must be different');
      return;
    }

    if (formData.basePrice <= 0) {
      setError('Base price must be greater than 0');
      return;
    }

    setIsSubmitting(true);

    try {
      // Clear unused fields based on scope
      const submitData: FareFormData = {
        ...formData,
        routeId: pricingScope === 'route' ? formData.routeId : undefined,
        originStationId: pricingScope === 'segment' ? formData.originStationId : undefined,
        destinationStationId: pricingScope === 'segment' ? formData.destinationStationId : undefined,
        tripId: pricingScope === 'trip' ? formData.tripId : undefined,
      };

      await faresApi.create(submitData);
      onSuccess();
      onClose();
    } catch (error: any) {
      setError(error.message || 'Failed to create fare');
    } finally {
      setIsSubmitting(false);
    }
  };

  const calculateFinalPrice = () => {
    const discount = formData.discountPercent || 0;
    const finalPrice = formData.basePrice * (1 - discount / 100);
    const totalWithVat = finalPrice * (1 + formData.vatRate);
    return { finalPrice, totalWithVat };
  };

  const { finalPrice, totalWithVat } = calculateFinalPrice();

  if (!isOpen) return null;

  return (
    <div className="fixed inset-0 z-50 overflow-y-auto">
      <div className="flex items-center justify-center min-h-screen px-4 pt-4 pb-20 text-center sm:block sm:p-0">
        <div className="fixed inset-0 transition-opacity bg-gray-500 bg-opacity-75" onClick={onClose} />

        <div className="inline-block align-bottom bg-white rounded-lg text-left overflow-hidden shadow-xl transform transition-all sm:my-8 sm:align-middle sm:max-w-2xl sm:w-full">
          <div className="bg-white px-6 pt-5 pb-4">
            <div className="flex items-center justify-between mb-4">
              <h3 className="text-lg font-medium text-gray-900">Create New Fare</h3>
              <button onClick={onClose} className="text-gray-400 hover:text-gray-500">
                <X size={24} />
              </button>
            </div>

            {error && (
              <div className="mb-4 p-3 bg-red-50 border border-red-200 rounded-lg text-red-700 text-sm">
                {error}
              </div>
            )}

            <form onSubmit={handleSubmit} className="space-y-4">
              {/* Pricing Scope */}
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-2">Pricing Scope</label>
                <div className="flex gap-3">
                  <label className="flex items-center gap-2 cursor-pointer">
                    <input
                      type="radio"
                      name="scope"
                      value="route"
                      checked={pricingScope === 'route'}
                      onChange={() => setPricingScope('route')}
                      className="text-admin-primary-600 focus:ring-admin-primary-500"
                    />
                    <span className="text-sm text-gray-700">Route-level</span>
                  </label>
                  <label className="flex items-center gap-2 cursor-pointer">
                    <input
                      type="radio"
                      name="scope"
                      value="segment"
                      checked={pricingScope === 'segment'}
                      onChange={() => setPricingScope('segment')}
                      className="text-admin-primary-600 focus:ring-admin-primary-500"
                    />
                    <span className="text-sm text-gray-700">Segment-specific</span>
                  </label>
                </div>
              </div>

              {/* Route Selection (for route-level pricing) */}
              {pricingScope === 'route' && (
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">Route *</label>
                  <select
                    value={formData.routeId || ''}
                    onChange={(e) => setFormData({ ...formData, routeId: Number(e.target.value) || undefined })}
                    required
                    className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-admin-primary-500"
                  >
                    <option value="">Select route...</option>
                    {routes.map((route) => (
                      <option key={route.id} value={route.id}>
                        {route.nameEn} ({route.origin.nameEn} → {route.destination.nameEn})
                      </option>
                    ))}
                  </select>
                </div>
              )}

              {/* Segment Selection (for segment-specific pricing) */}
              {pricingScope === 'segment' && (
                <div className="grid grid-cols-2 gap-4">
                  <div>
                    <label className="block text-sm font-medium text-gray-700 mb-1">Origin Station *</label>
                    <select
                      value={formData.originStationId || ''}
                      onChange={(e) => setFormData({ ...formData, originStationId: Number(e.target.value) || undefined })}
                      required
                      className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-admin-primary-500"
                    >
                      <option value="">Select origin...</option>
                      {stations.map((station) => (
                        <option key={station.id} value={station.id}>
                          {station.nameEn} ({station.code})
                        </option>
                      ))}
                    </select>
                  </div>
                  <div>
                    <label className="block text-sm font-medium text-gray-700 mb-1">Destination Station *</label>
                    <select
                      value={formData.destinationStationId || ''}
                      onChange={(e) => setFormData({ ...formData, destinationStationId: Number(e.target.value) || undefined })}
                      required
                      className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-admin-primary-500"
                    >
                      <option value="">Select destination...</option>
                      {stations.map((station) => (
                        <option key={station.id} value={station.id}>
                          {station.nameEn} ({station.code})
                        </option>
                      ))}
                    </select>
                  </div>
                </div>
              )}

              {/* Coach Class */}
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">Coach Class *</label>
                <select
                  value={formData.coachClass}
                  onChange={(e) => setFormData({ ...formData, coachClass: Number(e.target.value) })}
                  required
                  className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-admin-primary-500"
                >
                  <option value={CoachClass.First}>First Class</option>
                  <option value={CoachClass.Second}>Second Class</option>
                  <option value={CoachClass.Third}>Third Class</option>
                </select>
              </div>

              {/* Pricing */}
              <div className="grid grid-cols-2 gap-4">
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">Base Price (SDG) *</label>
                  <input
                    type="number"
                    step="0.01"
                    min="0"
                    value={formData.basePrice}
                    onChange={(e) => setFormData({ ...formData, basePrice: Number(e.target.value) })}
                    required
                    className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-admin-primary-500"
                  />
                </div>

                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">Price per km (optional)</label>
                  <input
                    type="number"
                    step="0.01"
                    min="0"
                    value={formData.pricePerKm || ''}
                    onChange={(e) => setFormData({ ...formData, pricePerKm: e.target.value ? Number(e.target.value) : undefined })}
                    className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-admin-primary-500"
                  />
                </div>
              </div>

              <div className="grid grid-cols-2 gap-4">
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">VAT Rate *</label>
                  <input
                    type="number"
                    step="0.01"
                    min="0"
                    max="1"
                    value={formData.vatRate}
                    onChange={(e) => setFormData({ ...formData, vatRate: Number(e.target.value) })}
                    required
                    className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-admin-primary-500"
                  />
                  <p className="text-xs text-gray-500 mt-1">0.15 = 15% VAT</p>
                </div>

                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">Discount %</label>
                  <input
                    type="number"
                    step="1"
                    min="0"
                    max="100"
                    value={formData.discountPercent || ''}
                    onChange={(e) => setFormData({ ...formData, discountPercent: e.target.value ? Number(e.target.value) : undefined })}
                    className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-admin-primary-500"
                  />
                </div>
              </div>

              {/* Price Preview */}
              <div className="p-4 bg-blue-50 border border-blue-200 rounded-lg">
                <div className="text-sm font-medium text-gray-700 mb-2">Price Preview</div>
                <div className="space-y-1 text-sm">
                  <div className="flex justify-between">
                    <span>Base Price:</span>
                    <span className="font-medium">{formData.basePrice.toFixed(2)} SDG</span>
                  </div>
                  {formData.discountPercent && formData.discountPercent > 0 && (
                    <div className="flex justify-between text-red-600">
                      <span>Discount ({formData.discountPercent}%):</span>
                      <span className="font-medium">-{(formData.basePrice * formData.discountPercent / 100).toFixed(2)} SDG</span>
                    </div>
                  )}
                  <div className="flex justify-between font-medium">
                    <span>Final Price:</span>
                    <span>{finalPrice.toFixed(2)} SDG</span>
                  </div>
                  <div className="flex justify-between text-gray-600">
                    <span>VAT ({(formData.vatRate * 100).toFixed(0)}%):</span>
                    <span>+{(finalPrice * formData.vatRate).toFixed(2)} SDG</span>
                  </div>
                  <div className="flex justify-between font-bold text-lg pt-2 border-t border-blue-300">
                    <span>Total with VAT:</span>
                    <span className="text-admin-primary-600">{totalWithVat.toFixed(2)} SDG</span>
                  </div>
                </div>
              </div>

              <div className="flex justify-end gap-3 pt-4 border-t">
                <button
                  type="button"
                  onClick={onClose}
                  className="px-4 py-2 text-gray-700 bg-white border border-gray-300 rounded-lg hover:bg-gray-50"
                  disabled={isSubmitting}
                >
                  Cancel
                </button>
                <button
                  type="submit"
                  className="admin-button"
                  disabled={isSubmitting}
                >
                  {isSubmitting ? 'Creating...' : 'Create Fare'}
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
