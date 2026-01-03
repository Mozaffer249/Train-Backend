import { useState, useEffect } from 'react';
import { X, Plus, Edit, Trash2, Clock } from 'lucide-react';
import { Route, RouteStationFormData } from '../../types/infrastructure';
import { routesApi, stationsApi } from '../../services/api';
import { Station } from '../../types/geography';
import StationTimingModal from './StationTimingModal';
import { showSuccess, showError, showConfirm, extractErrorMessage } from '../../utils/alerts';

interface RouteDetailModalProps {
  isOpen: boolean;
  onClose: () => void;
  onRefresh: () => void;
  routeId: number;
}

const RouteDetailModal = ({ isOpen, onClose, onRefresh, routeId }: RouteDetailModalProps) => {
  const [route, setRoute] = useState<Route | null>(null);
  const [stations, setStations] = useState<Station[]>([]);
  const [isLoading, setIsLoading] = useState(false);
  const [isAddingStation, setIsAddingStation] = useState(false);
  const [newStation, setNewStation] = useState({
    stationId: 0,
    stopOrder: 1,
    arrivalMinutesFromOrigin: 0,
    departureMinutesFromOrigin: 0,
  });
  const [editingStation, setEditingStation] = useState<{ routeId: number; stationId: number } | null>(null);

  useEffect(() => {
    if (isOpen && routeId) {
      loadRoute();
      loadStations();
    }
  }, [isOpen, routeId]);

  const loadRoute = async () => {
    setIsLoading(true);
    try {
      const data = await routesApi.getById(routeId);
      setRoute(data);
    } catch (error: any) {
      console.error('Failed to load route:', error);
      showError('Loading Error', extractErrorMessage(error));
    } finally {
      setIsLoading(false);
    }
  };

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

  const handleAddStation = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!newStation.stationId) {
      showError('Validation Error', 'Please select a station');
      return;
    }

    try {
      await routesApi.addStation(routeId, newStation);
      setIsAddingStation(false);
      setNewStation({ stationId: 0, stopOrder: 1, arrivalMinutesFromOrigin: 0, departureMinutesFromOrigin: 0 });
      await showSuccess('Station Added', 'Intermediate station added successfully');
      loadRoute();
      onRefresh();
    } catch (error: any) {
      showError('Add Failed', extractErrorMessage(error));
    }
  };

  const handleRemoveStation = async (stationId: number) => {
    const confirmed = await showConfirm(
      'Remove Station?',
      'This will remove the station from the route. Remaining stations will be resequenced.',
      'Yes, remove it'
    );
    
    if (!confirmed) return;

    try {
      await routesApi.removeStation(routeId, stationId);
      await showSuccess('Removed', 'Station removed from route');
      loadRoute();
      onRefresh();
    } catch (error: any) {
      showError('Remove Failed', extractErrorMessage(error));
    }
  };

  const formatTimeOffset = (offset: string) => {
    // TimeSpan format "HH:MM:SS" -> convert to minutes for display
    const parts = offset.split(':');
    const hours = parseInt(parts[0]);
    const minutes = parseInt(parts[1]);
    return `${hours}h ${minutes}m`;
  };

  if (!isOpen) return null;

  return (
    <>
      <div className="fixed inset-0 z-50 overflow-y-auto">
        <div className="flex items-center justify-center min-h-screen px-4 pt-4 pb-20 text-center sm:block sm:p-0">
          <div className="fixed inset-0 transition-opacity bg-gray-500 bg-opacity-75" onClick={onClose} />

          <div className="inline-block align-bottom bg-white rounded-lg text-left overflow-hidden shadow-xl transform transition-all sm:my-8 sm:align-middle sm:max-w-4xl sm:w-full">
            <div className="bg-white px-6 pt-5 pb-4">
              <div className="flex items-center justify-between mb-4">
                <h3 className="text-lg font-medium text-gray-900">Route Details</h3>
                <button onClick={onClose} className="text-gray-400 hover:text-gray-500">
                  <X size={24} />
                </button>
              </div>

              {isLoading ? (
                <div className="py-8 text-center text-gray-500">Loading...</div>
              ) : route ? (
                <div className="space-y-6">
                  {/* Route Info */}
                  <div className="grid grid-cols-2 gap-4 p-4 bg-gray-50 rounded-lg">
                    <div>
                      <p className="text-sm text-gray-500">Route Name</p>
                      <p className="font-medium">{route.nameEn}</p>
                      <p className="text-sm text-gray-600" dir="rtl">{route.nameAr}</p>
                    </div>
                    <div>
                      <p className="text-sm text-gray-500">Distance</p>
                      <p className="font-medium">{route.distanceKm ? `${route.distanceKm.toFixed(2)} km` : 'N/A'}</p>
                    </div>
                    <div>
                      <p className="text-sm text-gray-500">Origin</p>
                      <p className="font-medium">{route.origin.nameEn}</p>
                    </div>
                    <div>
                      <p className="text-sm text-gray-500">Destination</p>
                      <p className="font-medium">{route.destination.nameEn}</p>
                    </div>
                  </div>

                  {/* Intermediate Stations */}
                  <div>
                    <div className="flex items-center justify-between mb-3">
                      <h4 className="font-medium text-gray-900">Intermediate Stations</h4>
                      <button
                        onClick={() => setIsAddingStation(true)}
                        className="text-sm admin-button flex items-center gap-1"
                      >
                        <Plus size={16} />
                        Add Station
                      </button>
                    </div>

                    {isAddingStation && (
                      <form onSubmit={handleAddStation} className="mb-4 p-4 border border-gray-200 rounded-lg space-y-3">
                        <div className="grid grid-cols-2 gap-3">
                          <div>
                            <label className="block text-sm font-medium text-gray-700 mb-1">Station</label>
                            <select
                              value={newStation.stationId}
                              onChange={(e) => setNewStation({ ...newStation, stationId: Number(e.target.value) })}
                              required
                              className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-admin-primary-500"
                            >
                              <option value={0}>Select station...</option>
                              {stations
                                .filter(s => 
                                  route && 
                                  s.id !== route.origin.id && 
                                  s.id !== route.destination.id
                                )
                                .map((station) => (
                                  <option key={station.id} value={station.id}>
                                    {station.nameEn} ({station.code})
                                  </option>
                                ))}
                            </select>
                            <p className="text-xs text-gray-500 mt-1">
                              Showing {stations.filter(s => route && s.id !== route.origin.id && s.id !== route.destination.id).length} available stations
                            </p>
                          </div>
                          <div>
                            <label className="block text-sm font-medium text-gray-700 mb-1">Stop Order</label>
                            <input
                              type="number"
                              min={1}
                              value={newStation.stopOrder}
                              onChange={(e) => setNewStation({ ...newStation, stopOrder: Number(e.target.value) })}
                              required
                              className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-admin-primary-500"
                            />
                          </div>
                          <div>
                            <label className="block text-sm font-medium text-gray-700 mb-1">Arrival (minutes)</label>
                            <input
                              type="number"
                              min={0}
                              value={newStation.arrivalMinutesFromOrigin}
                              onChange={(e) => setNewStation({ ...newStation, arrivalMinutesFromOrigin: Number(e.target.value) })}
                              required
                              className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-admin-primary-500"
                            />
                          </div>
                          <div>
                            <label className="block text-sm font-medium text-gray-700 mb-1">Departure (minutes)</label>
                            <input
                              type="number"
                              min={0}
                              value={newStation.departureMinutesFromOrigin}
                              onChange={(e) => setNewStation({ ...newStation, departureMinutesFromOrigin: Number(e.target.value) })}
                              required
                              className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-admin-primary-500"
                            />
                          </div>
                        </div>
                        <div className="flex gap-2">
                          <button type="submit" className="admin-button text-sm">Add</button>
                          <button
                            type="button"
                            onClick={() => setIsAddingStation(false)}
                            className="px-3 py-1 text-sm text-gray-700 bg-white border border-gray-300 rounded-lg hover:bg-gray-50"
                          >
                            Cancel
                          </button>
                        </div>
                      </form>
                    )}

                    {route.intermediateStops.length === 0 ? (
                      <p className="text-sm text-gray-500 text-center py-4">No intermediate stations</p>
                    ) : (
                      <div className="border border-gray-200 rounded-lg overflow-hidden">
                        <table className="w-full">
                          <thead className="bg-gray-50">
                            <tr>
                              <th className="px-4 py-2 text-left text-xs font-medium text-gray-500">Order</th>
                              <th className="px-4 py-2 text-left text-xs font-medium text-gray-500">Station</th>
                              <th className="px-4 py-2 text-left text-xs font-medium text-gray-500">Arrival</th>
                              <th className="px-4 py-2 text-left text-xs font-medium text-gray-500">Departure</th>
                              <th className="px-4 py-2 text-left text-xs font-medium text-gray-500">Actions</th>
                            </tr>
                          </thead>
                          <tbody className="divide-y divide-gray-200">
                            {route.intermediateStops.map((stop) => (
                              <tr key={stop.id}>
                                <td className="px-4 py-3 text-sm">{stop.stopOrder}</td>
                                <td className="px-4 py-3 text-sm font-medium">{stop.stationName}</td>
                                <td className="px-4 py-3 text-sm text-gray-600">
                                  <Clock size={14} className="inline mr-1" />
                                  {formatTimeOffset(stop.arrivalOffset)}
                                </td>
                                <td className="px-4 py-3 text-sm text-gray-600">
                                  <Clock size={14} className="inline mr-1" />
                                  {formatTimeOffset(stop.departureOffset)}
                                </td>
                                <td className="px-4 py-3 text-sm">
                                  <div className="flex gap-2">
                                    <button
                                      onClick={() => setEditingStation({ routeId, stationId: stop.stationId })}
                                      className="text-admin-primary-600 hover:text-admin-primary-800"
                                      title="Edit timing"
                                    >
                                      <Edit size={16} />
                                    </button>
                                    <button
                                      onClick={() => handleRemoveStation(stop.stationId)}
                                      className="text-red-600 hover:text-red-800"
                                      title="Remove station"
                                    >
                                      <Trash2 size={16} />
                                    </button>
                                  </div>
                                </td>
                              </tr>
                            ))}
                          </tbody>
                        </table>
                      </div>
                    )}
                  </div>

                  <div className="flex justify-end pt-4 border-t">
                    <button onClick={onClose} className="admin-button">
                      Close
                    </button>
                  </div>
                </div>
              ) : (
                <p className="text-center text-gray-500 py-8">Route not found</p>
              )}
            </div>
          </div>
        </div>
      </div>

      {editingStation && (
        <StationTimingModal
          isOpen={true}
          onClose={() => setEditingStation(null)}
          onSuccess={() => {
            setEditingStation(null);
            loadRoute();
            onRefresh();
          }}
          routeId={editingStation.routeId}
          stationId={editingStation.stationId}
        />
      )}
    </>
  );
};

export default RouteDetailModal;
