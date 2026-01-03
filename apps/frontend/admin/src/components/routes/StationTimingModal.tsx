import { useState, useEffect } from 'react';
import { X } from 'lucide-react';
import { routesApi } from '../../services/api';
import { showSuccess, showError, extractErrorMessage } from '../../utils/alerts';

interface StationTimingModalProps {
  isOpen: boolean;
  onClose: () => void;
  onSuccess: () => void;
  routeId: number;
  stationId: number;
}

const StationTimingModal = ({ isOpen, onClose, onSuccess, routeId, stationId }: StationTimingModalProps) => {
  const [formData, setFormData] = useState({
    stopOrder: 1,
    arrivalMinutesFromOrigin: 0,
    departureMinutesFromOrigin: 0,
  });
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [error, setError] = useState('');

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError('');

    if (formData.departureMinutesFromOrigin <= formData.arrivalMinutesFromOrigin) {
      setError('Departure time must be after arrival time');
      return;
    }

    setIsSubmitting(true);

    try {
      await routesApi.updateStation(routeId, stationId, formData);
      await showSuccess('Updated', 'Station timing updated successfully');
      onSuccess();
      onClose();
    } catch (error: any) {
      const errorMessage = extractErrorMessage(error);
      setError(errorMessage);
      showError('Update Failed', errorMessage);
    } finally {
      setIsSubmitting(false);
    }
  };

  if (!isOpen) return null;

  return (
    <div className="fixed inset-0 z-[60] overflow-y-auto">
      <div className="flex items-center justify-center min-h-screen px-4 pt-4 pb-20 text-center sm:block sm:p-0">
        <div className="fixed inset-0 transition-opacity bg-gray-500 bg-opacity-75" onClick={onClose} />

        <div className="inline-block align-bottom bg-white rounded-lg text-left overflow-hidden shadow-xl transform transition-all sm:my-8 sm:align-middle sm:max-w-lg sm:w-full">
          <div className="bg-white px-6 pt-5 pb-4">
            <div className="flex items-center justify-between mb-4">
              <h3 className="text-lg font-medium text-gray-900">Edit Station Timing</h3>
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
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">
                  Stop Order
                </label>
                <input
                  type="number"
                  min={1}
                  value={formData.stopOrder}
                  onChange={(e) => setFormData({ ...formData, stopOrder: Number(e.target.value) })}
                  required
                  className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-admin-primary-500"
                />
                <p className="text-xs text-gray-500 mt-1">The order in which this station appears in the route</p>
              </div>

              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">
                  Arrival Time (minutes from origin)
                </label>
                <input
                  type="number"
                  min={0}
                  value={formData.arrivalMinutesFromOrigin}
                  onChange={(e) => setFormData({ ...formData, arrivalMinutesFromOrigin: Number(e.target.value) })}
                  required
                  className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-admin-primary-500"
                />
                <p className="text-xs text-gray-500 mt-1">
                  Time in minutes from the origin station (e.g., 45 = 45 minutes after departure)
                </p>
              </div>

              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">
                  Departure Time (minutes from origin)
                </label>
                <input
                  type="number"
                  min={0}
                  value={formData.departureMinutesFromOrigin}
                  onChange={(e) => setFormData({ ...formData, departureMinutesFromOrigin: Number(e.target.value) })}
                  required
                  className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-admin-primary-500"
                />
                <p className="text-xs text-gray-500 mt-1">
                  Must be greater than arrival time (e.g., 50 = 50 minutes after origin departure)
                </p>
              </div>

              <div className="bg-blue-50 border border-blue-200 rounded-lg p-3 text-sm text-blue-700">
                <strong>Stop Duration:</strong>{' '}
                {formData.departureMinutesFromOrigin - formData.arrivalMinutesFromOrigin} minutes
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
                  {isSubmitting ? 'Updating...' : 'Update Timing'}
                </button>
              </div>
            </form>
          </div>
        </div>
      </div>
    </div>
  );
};

export default StationTimingModal;
