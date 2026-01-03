import { useState, useEffect } from 'react';
import { X } from 'lucide-react';
import { Route, RouteFormData } from '../../types/infrastructure';
import { routesApi, stationsApi } from '../../services/api';
import { Station } from '../../types/geography';
import { showSuccess, showError, extractErrorMessage } from '../../utils/alerts';

interface RouteModalProps {
  isOpen: boolean;
  onClose: () => void;
  onSuccess: () => void;
  route?: Route | null;
}

const RouteModal = ({ isOpen, onClose, onSuccess, route }: RouteModalProps) => {
  const [stations, setStations] = useState<Station[]>([]);
  const [formData, setFormData] = useState<RouteFormData>({
    nameEn: '',
    nameAr: '',
    originStationId: 0,
    destinationStationId: 0,
    distanceKm: undefined,
    isActive: true,
    maintenanceNote: '',
  });
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [error, setError] = useState('');

  useEffect(() => {
    if (isOpen) {
      loadStations();
      if (route) {
        setFormData({
          nameEn: route.nameEn,
          nameAr: route.nameAr,
          originStationId: route.origin.id,
          destinationStationId: route.destination.id,
          distanceKm: route.distanceKm,
          isActive: route.isActive,
          maintenanceNote: route.maintenanceNote || '',
        });
      } else {
        resetForm();
      }
    }
  }, [isOpen, route]);

  const loadStations = async () => {
    try {
      // Load all active stations with high pageSize for dropdown
      const data = await stationsApi.getAll({ isActive: true, pageSize: 10000 });
      setStations(data);
    } catch (error: any) {
      console.error('Failed to load stations:', error);
      showError('Loading Error', extractErrorMessage(error));
    }
  };

  const resetForm = () => {
    setFormData({
      nameEn: '',
      nameAr: '',
      originStationId: 0,
      destinationStationId: 0,
      distanceKm: undefined,
      isActive: true,
      maintenanceNote: '',
    });
    setError('');
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError('');

    if (!formData.originStationId || !formData.destinationStationId) {
      setError('Please select both origin and destination stations');
      return;
    }

    if (formData.originStationId === formData.destinationStationId) {
      setError('Origin and destination must be different stations');
      return;
    }

    setIsSubmitting(true);

    try {
      if (route) {
        await routesApi.update(route.id, formData);
        await showSuccess('Updated', 'Route updated successfully');
      } else {
        await routesApi.create(formData);
        await showSuccess('Created', 'Route created successfully');
      }
      onSuccess();
      onClose();
    } catch (error: any) {
      const errorMessage = extractErrorMessage(error);
      setError(errorMessage);
      showError('Save Failed', errorMessage);
    } finally {
      setIsSubmitting(false);
    }
  };

  if (!isOpen) return null;

  return (
    <div className="fixed inset-0 z-50 overflow-y-auto">
      <div className="flex items-center justify-center min-h-screen px-4 pt-4 pb-20 text-center sm:block sm:p-0">
        <div className="fixed inset-0 transition-opacity bg-gray-500 bg-opacity-75" onClick={onClose} />

        <div className="inline-block align-bottom bg-white rounded-lg text-left overflow-hidden shadow-xl transform transition-all sm:my-8 sm:align-middle sm:max-w-2xl sm:w-full">
          <div className="bg-white px-6 pt-5 pb-4">
            <div className="flex items-center justify-between mb-4">
              <h3 className="text-lg font-medium text-gray-900">
                {route ? 'Edit Route' : 'Create New Route'}
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

            {route && route.tripsCount > 0 && (
              <div className="mb-4 p-3 bg-yellow-50 border border-yellow-200 rounded-lg text-yellow-700 text-sm">
                <strong>Warning:</strong> This route has {route.tripsCount} trip(s). 
                You cannot change origin or destination stations.
              </div>
            )}

            <form onSubmit={handleSubmit} className="space-y-4">
              <div className="grid grid-cols-2 gap-4">
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">
                    Name (English)
                  </label>
                  <input
                    type="text"
                    value={formData.nameEn}
                    onChange={(e) => setFormData({ ...formData, nameEn: e.target.value })}
                    placeholder="Optional - auto-generated if empty"
                    className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-admin-primary-500"
                  />
                </div>

                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">
                    Name (Arabic)
                  </label>
                  <input
                    type="text"
                    value={formData.nameAr}
                    onChange={(e) => setFormData({ ...formData, nameAr: e.target.value })}
                    placeholder="Optional - auto-generated if empty"
                    dir="rtl"
                    className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-admin-primary-500"
                  />
                </div>
              </div>

              <div className="grid grid-cols-2 gap-4">
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">
                    Origin Station *
                  </label>
                  <select
                    value={formData.originStationId}
                    onChange={(e) => setFormData({ ...formData, originStationId: Number(e.target.value) })}
                    required
                    disabled={route && route.tripsCount > 0}
                    className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-admin-primary-500 disabled:bg-gray-100 disabled:cursor-not-allowed"
                  >
                    <option value={0}>Select origin...</option>
                    {stations.map((station) => (
                      <option key={station.id} value={station.id}>
                        {station.nameEn} ({station.code})
                      </option>
                    ))}
                  </select>
                  <p className="text-xs text-gray-500 mt-1">Total stations: {stations.length}</p>
                </div>

                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">
                    Destination Station *
                  </label>
                  <select
                    value={formData.destinationStationId}
                    onChange={(e) => setFormData({ ...formData, destinationStationId: Number(e.target.value) })}
                    required
                    disabled={route && route.tripsCount > 0}
                    className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-admin-primary-500 disabled:bg-gray-100 disabled:cursor-not-allowed"
                  >
                    <option value={0}>Select destination...</option>
                    {stations.map((station) => (
                      <option key={station.id} value={station.id}>
                        {station.nameEn} ({station.code})
                      </option>
                    ))}
                  </select>
                  <p className="text-xs text-gray-500 mt-1">Total stations: {stations.length}</p>
                </div>
              </div>

              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">
                  Distance (km)
                </label>
                <input
                  type="number"
                  step="0.01"
                  value={formData.distanceKm || ''}
                  onChange={(e) => setFormData({ ...formData, distanceKm: e.target.value ? Number(e.target.value) : undefined })}
                  placeholder="Auto-calculated if empty"
                  className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-admin-primary-500"
                />
                <p className="text-xs text-gray-500 mt-1">Leave empty for automatic calculation using coordinates</p>
              </div>

              <div className="flex items-center gap-2">
                <input
                  type="checkbox"
                  id="isActive"
                  checked={formData.isActive}
                  onChange={(e) => setFormData({ ...formData, isActive: e.target.checked })}
                  className="rounded border-gray-300 text-admin-primary-600 focus:ring-admin-primary-500"
                />
                <label htmlFor="isActive" className="text-sm text-gray-700">
                  Active Route
                </label>
              </div>

              {!formData.isActive && (
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">
                    Maintenance Note
                  </label>
                  <textarea
                    value={formData.maintenanceNote}
                    onChange={(e) => setFormData({ ...formData, maintenanceNote: e.target.value })}
                    rows={3}
                    maxLength={500}
                    placeholder="Reason for deactivation..."
                    className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-admin-primary-500"
                  />
                  <p className="text-xs text-gray-500 mt-1">{formData.maintenanceNote?.length || 0}/500</p>
                </div>
              )}

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
                  {isSubmitting ? 'Saving...' : route ? 'Update ..Route' : 'Create Route'}
                </button>
              </div>
            </form>
          </div>
        </div>
      </div>
    </div>
  );
};

export default RouteModal;
