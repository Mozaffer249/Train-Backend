import { Map, Layers, Plus, Edit, Upload } from 'lucide-react';
import { EntityType, Area, Governorate, City } from '../../types/geography';

interface MapControlSidebarProps {
  activeEntityType: EntityType;
  onEntityTypeChange: (type: EntityType) => void;
  selectedParentId?: number;
  onParentChange: (parentId: number) => void;
  areas: Area[];
  governorates: Governorate[];
  cities: City[];
  layerVisibility: {
    areas: boolean;
    governorates: boolean;
    cities: boolean;
    stations: boolean;
  };
  onLayerToggle: (layer: keyof typeof layerVisibility) => void;
  onManageBoundaries: () => void;
  onBulkImport: () => void;
  onClickToAdd: () => void;
  isAddMode: boolean;
}

const MapControlSidebar = ({
  activeEntityType,
  onEntityTypeChange,
  selectedParentId,
  onParentChange,
  regions,
  states,
  cities,
  layerVisibility,
  onLayerToggle,
  onManageBoundaries,
  onBulkImport,
  onClickToAdd,
  isAddMode,
}: MapControlSidebarProps) => {
  // Get parent options based on entity type
  const getParentOptions = () => {
    if (activeEntityType === 'area') return [];
    if (activeEntityType === 'governorate') return areas;
    if (activeEntityType === 'city') return governorates;
    if (activeEntityType === 'station') return cities;
    return [];
  };

  const parentOptions = getParentOptions();
  const showParentSelector = parentOptions.length > 0;

  return (
    <div className="absolute top-4 right-4 w-80 bg-white rounded-lg shadow-lg p-4 z-10 max-h-[calc(100vh-8rem)] overflow-y-auto">
      {/* Header */}
      <div className="flex items-center gap-2 mb-4 pb-3 border-b">
        <Map className="text-admin-primary-600" size={20} />
        <h3 className="font-bold text-gray-900">Map Controls</h3>
      </div>

      {/* Entity Type Selector */}
      <div className="mb-4">
        <label className="block text-sm font-medium text-gray-700 mb-2">
          Entity Type
        </label>
        <div className="space-y-2">
          {(['area', 'governorate', 'city', 'station'] as EntityType[]).map((type) => (
            <label key={type} className="flex items-center gap-2 cursor-pointer">
              <input
                type="radio"
                name="entityType"
                value={type}
                checked={activeEntityType === type}
                onChange={(e) => onEntityTypeChange(e.target.value as EntityType)}
                className="text-admin-primary-600 focus:ring-admin-primary-500"
              />
              <span className="text-sm text-gray-700 capitalize">{type}</span>
            </label>
          ))}
        </div>
      </div>

      {/* Parent Selector */}
      {showParentSelector && (
        <div className="mb-4">
          <label className="block text-sm font-medium text-gray-700 mb-2">
            Select Parent {activeEntityType === 'governorate' ? 'Region' : activeEntityType === 'city' ? 'State' : 'City'}
          </label>
          <select
            value={selectedParentId || ''}
            onChange={(e) => onParentChange(parseInt(e.target.value))}
            className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-admin-primary-500 text-sm"
          >
            <option value="">Select...</option>
            {parentOptions.map((option: any) => (
              <option key={option.id} value={option.id}>
                {option.nameEn}
              </option>
            ))}
          </select>
        </div>
      )}

      {/* Layer Visibility */}
      <div className="mb-4 pb-4 border-b">
        <div className="flex items-center gap-2 mb-2">
          <Layers size={16} className="text-gray-600" />
          <label className="block text-sm font-medium text-gray-700">
            Layer Visibility
          </label>
        </div>
        <div className="space-y-2">
          {Object.entries(layerVisibility).map(([layer, visible]) => (
            <label key={layer} className="flex items-center gap-2 cursor-pointer">
              <input
                type="checkbox"
                checked={visible}
                onChange={() => onLayerToggle(layer as keyof typeof layerVisibility)}
                className="text-admin-primary-600 focus:ring-admin-primary-500 rounded"
              />
              <span className="text-sm text-gray-700 capitalize">{layer}</span>
            </label>
          ))}
        </div>
      </div>

      {/* Action Buttons */}
      <div className="space-y-2">
        <button
          onClick={onClickToAdd}
          className={`w-full flex items-center justify-center gap-2 px-4 py-2 rounded-lg transition-colors ${
            isAddMode
              ? 'bg-admin-primary-600 text-white hover:bg-admin-primary-700'
              : 'bg-gray-100 text-gray-700 hover:bg-gray-200'
          }`}
        >
          <Plus size={18} />
          <span className="text-sm font-medium">
            {isAddMode ? 'Click Map to Add' : 'Enable Click-to-Add'}
          </span>
        </button>

        <button
          onClick={onManageBoundaries}
          className="w-full flex items-center justify-center gap-2 px-4 py-2 bg-blue-100 text-blue-700 rounded-lg hover:bg-blue-200 transition-colors"
        >
          <Edit size={18} />
          <span className="text-sm font-medium">Manage Boundaries</span>
        </button>

        {activeEntityType === 'station' && (
          <button
            onClick={onBulkImport}
            className="w-full flex items-center justify-center gap-2 px-4 py-2 bg-green-100 text-green-700 rounded-lg hover:bg-green-200 transition-colors"
          >
            <Upload size={18} />
            <span className="text-sm font-medium">Bulk Import Stations</span>
          </button>
        )}
      </div>

      {/* Help Text */}
      <div className="mt-4 pt-4 border-t">
        <p className="text-xs text-gray-500">
          {isAddMode ? (
            <>
              <span className="font-medium text-admin-primary-600">Click-to-Add Mode Active:</span> Click anywhere on the map to add a new {activeEntityType}
            </>
          ) : (
            'Select an entity type and enable click-to-add to place markers on the map'
          )}
        </p>
      </div>
    </div>
  );
};

export default MapControlSidebar;
