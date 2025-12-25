import { useState, useEffect } from 'react';
import { X } from 'lucide-react';
import { EntityType, Area, Governorate, City, BoundaryData } from '../../types/geography';
import { boundaryApi } from '../../services/api';

interface BoundaryManagerProps {
  isOpen: boolean;
  onClose: () => void;
  areas: Area[];
  governorates: Governorate[];
  cities: City[];
  onStartDrawing: (entityType: EntityType, entityId: number) => void;
}

const BoundaryManager = ({
  isOpen,
  onClose,
  areas,
  governorates,
  cities,
  onStartDrawing,
}: BoundaryManagerProps) => {
  const [entityType, setEntityType] = useState<EntityType>('area');
  const [selectedEntityId, setSelectedEntityId] = useState<number | null>(null);
  const [boundaryData, setBoundaryData] = useState<BoundaryData | null>(null);
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  // Reset state when modal opens
  useEffect(() => {
    if (isOpen) {
      setEntityType('area');
      setSelectedEntityId(null);
      setBoundaryData(null);
      setError(null);
    }
  }, [isOpen]);

  // Load boundary data when entity is selected
  useEffect(() => {
    const loadBoundary = async () => {
      if (!selectedEntityId) {
        setBoundaryData(null);
        return;
      }

      setIsLoading(true);
      setError(null);

      try {
        let data: BoundaryData;
        if (entityType === 'area') {
          data = await boundaryApi.getRegionBoundary(selectedEntityId);
        } else if (entityType === 'governorate') {
          data = await boundaryApi.getStateBoundary(selectedEntityId);
        } else {
          data = await boundaryApi.getCityBoundary(selectedEntityId);
        }
        setBoundaryData(data);
      } catch (err: any) {
        setError(err.message || 'Failed to load boundary data');
        setBoundaryData(null);
      } finally {
        setIsLoading(false);
      }
    };

    loadBoundary();
  }, [selectedEntityId, entityType]);

  // Get entity options based on type
  const getEntityOptions = () => {
    if (entityType === 'area') return areas;
    if (entityType === 'governorate') return governorates;
    if (entityType === 'city') return cities;
    return [];
  };

  const handleStartDrawing = () => {
    if (selectedEntityId) {
      onStartDrawing(entityType, selectedEntityId);
      onClose();
    }
  };

  const handleClearBoundary = async () => {
    if (!selectedEntityId) return;

    if (!confirm('Are you sure you want to clear this boundary?')) return;

    setIsLoading(true);
    setError(null);

    try {
      const emptyData: BoundaryData = {
        boundaryPolygon: undefined,
        boundingBoxNorth: undefined,
        boundingBoxSouth: undefined,
        boundingBoxEast: undefined,
        boundingBoxWest: undefined,
      };

      if (entityType === 'area') {
        await boundaryApi.updateRegionBoundary(selectedEntityId, emptyData);
      } else if (entityType === 'governorate') {
        await boundaryApi.updateStateBoundary(selectedEntityId, emptyData);
      } else {
        await boundaryApi.updateCityBoundary(selectedEntityId, emptyData);
      }

      setBoundaryData(null);
      alert('Boundary cleared successfully');
    } catch (err: any) {
      setError(err.message || 'Failed to clear boundary');
    } finally {
      setIsLoading(false);
    }
  };

  if (!isOpen) return null;

  const entityOptions = getEntityOptions();
  const hasBoundary = boundaryData?.boundaryPolygon;

  return (
    <div
      className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50"
      onClick={onClose}
    >
      <div
        className="bg-white rounded-lg shadow-xl w-full max-w-lg mx-4"
        onClick={(e) => e.stopPropagation()}
      >
        {/* Header */}
        <div className="flex items-center justify-between p-6 border-b border-gray-200">
          <h2 className="text-xl font-bold text-gray-900">Manage Boundaries</h2>
          <button
            onClick={onClose}
            className="text-gray-400 hover:text-gray-600 transition-colors"
          >
            <X size={24} />
          </button>
        </div>

        {/* Content */}
        <div className="p-6 space-y-4">
          {error && (
            <div className="p-3 bg-red-50 border border-red-200 rounded-lg text-red-800 text-sm">
              {error}
            </div>
          )}

          {/* Entity Type Selector */}
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-2">
              Entity Type
            </label>
            <select
              value={entityType}
              onChange={(e) => {
                setEntityType(e.target.value as EntityType);
                setSelectedEntityId(null);
              }}
              className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-admin-primary-500"
              disabled={isLoading}
            >
              <option value="region">Region</option>
              <option value="state">State</option>
              <option value="city">City</option>
            </select>
          </div>

          {/* Entity Selector */}
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-2">
              Select {entityType.charAt(0).toUpperCase() + entityType.slice(1)}
            </label>
            <select
              value={selectedEntityId || ''}
              onChange={(e) => setSelectedEntityId(parseInt(e.target.value))}
              className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-admin-primary-500"
              disabled={isLoading}
            >
              <option value="">Select...</option>
              {entityOptions.map((entity: any) => (
                <option key={entity.id} value={entity.id}>
                  {entity.nameEn}
                </option>
              ))}
            </select>
          </div>

          {/* Boundary Status */}
          {selectedEntityId && (
            <div className="p-4 bg-gray-50 rounded-lg">
              <p className="text-sm font-medium text-gray-700 mb-2">
                Boundary Status:
              </p>
              {isLoading ? (
                <p className="text-sm text-gray-500">Loading...</p>
              ) : hasBoundary ? (
                <div className="space-y-1">
                  <p className="text-sm text-green-600 font-medium">
                    ✓ Boundary exists
                  </p>
                  {boundaryData.boundingBoxNorth && (
                    <p className="text-xs text-gray-500">
                      Bounds: {boundaryData.boundingBoxSouth?.toFixed(4)} to{' '}
                      {boundaryData.boundingBoxNorth?.toFixed(4)} (lat),{' '}
                      {boundaryData.boundingBoxWest?.toFixed(4)} to{' '}
                      {boundaryData.boundingBoxEast?.toFixed(4)} (lng)
                    </p>
                  )}
                </div>
              ) : (
                <p className="text-sm text-gray-500">No boundary defined</p>
              )}
            </div>
          )}
        </div>

        {/* Actions */}
        <div className="flex gap-3 p-6 border-t border-gray-200">
          <button
            onClick={onClose}
            className="flex-1 px-4 py-2 border border-gray-300 text-gray-700 rounded-lg hover:bg-gray-50 transition-colors"
          >
            Cancel
          </button>
          {selectedEntityId && (
            <>
              {hasBoundary && (
                <button
                  onClick={handleClearBoundary}
                  disabled={isLoading}
                  className="flex-1 px-4 py-2 bg-red-500 text-white rounded-lg hover:bg-red-600 transition-colors disabled:opacity-50"
                >
                  Clear Boundary
                </button>
              )}
              <button
                onClick={handleStartDrawing}
                disabled={isLoading}
                className="flex-1 px-4 py-2 bg-admin-primary-600 text-white rounded-lg hover:bg-admin-primary-700 transition-colors disabled:opacity-50"
              >
                {hasBoundary ? 'Redraw' : 'Draw'} Boundary
              </button>
            </>
          )}
        </div>
      </div>
    </div>
  );
};

export default BoundaryManager;
