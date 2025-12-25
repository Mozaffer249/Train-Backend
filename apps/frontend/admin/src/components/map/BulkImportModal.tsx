import { useGovernorate, useEffect } from 'react';
import { X, ChevronRight, ChevronLeft } from 'lucide-react';
import { City, Station } from '../../types/geography';
import { spatialApi, stationsApi } from '../../services/api';

interface BulkImportModalProps {
  isOpen: boolean;
  onClose: () => void;
  cities: City[];
  existingStations: Station[];
  onImportComplete: () => void;
}

type ImportStep = 1 | 2 | 3;

interface GoogleStation {
  name: string;
  address: string;
  latitude: number;
  longitude: number;
  placeId: string;
  types: string[];
}

const BulkImportModal = ({
  isOpen,
  onClose,
  cities,
  existingStations,
  onImportComplete,
}: BulkImportModalProps) => {
  const [step, setStep] = useState<ImportStep>(1);
  const [selectedCityId, setSelectedCityId] = useState<number | null>(null);
  const [radiusKm, setRadiusKm] = useState(25);
  const [stationType, setStationType] = useState('train_station');
  const [searchResults, setSearchResults] = useState<GoogleStation[]>([]);
  const [selectedStations, setSelectedStations] = useState<Set<string>>(new Set());
  const [isSearching, setIsSearching] = useState(false);
  const [isImporting, setIsImporting] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [importProgress, setImportProgress] = useState({ current: 0, total: 0 });

  // Reset state when modal opens
  useEffect(() => {
    if (isOpen) {
      setStep(1);
      setSelectedCityId(null);
      setRadiusKm(25);
      setStationType('train_station');
      setSearchResults([]);
      setSelectedStations(new Set());
      setError(null);
    }
  }, [isOpen]);

  const handleSearch = async () => {
    if (!selectedCityId) return;

    const city = cities.find((c) => c.id === selectedCityId) as any;
    if (!city?.latitude || !city?.longitude) {
      setError('Selected city does not have coordinates');
      return;
    }

    setIsSearching(true);
    setError(null);

    try {
      const results = await spatialApi.getNearbyStations(
        city.latitude,
        city.longitude,
        radiusKm
      );

      // Transform results to our format
      const stations: GoogleStation[] = results.map((r: any) => ({
        name: r.name,
        address: r.vicinity || r.formatted_address || '',
        latitude: r.geometry?.location?.lat || 0,
        longitude: r.geometry?.location?.lng || 0,
        placeId: r.place_id,
        types: r.types || [],
      }));

      setSearchResults(stations);
      setStep(2);
    } catch (err: any) {
      setError(err.message || 'Failed to search for stations');
    } finally {
      setIsSearching(false);
    }
  };

  const handleToggleStation = (placeId: string) => {
    const newSelected = new Set(selectedStations);
    if (newSelected.has(placeId)) {
      newSelected.delete(placeId);
    } else {
      newSelected.add(placeId);
    }
    setSelectedStations(newSelected);
  };

  const handleSelectAll = () => {
    if (selectedStations.size === searchResults.length) {
      setSelectedStations(new Set());
    } else {
      setSelectedStations(new Set(searchResults.map((s) => s.placeId)));
    }
  };

  const isStationDuplicate = (station: GoogleStation) => {
    return existingStations.some(
      (existing) =>
        existing.nameEn.toLowerCase() === station.name.toLowerCase() &&
        existing.cityId === selectedCityId
    );
  };

  const handleImport = async () => {
    if (!selectedCityId) return;

    const stationsToImport = searchResults.filter((s) =>
      selectedStations.has(s.placeId)
    );

    if (stationsToImport.length === 0) {
      setError('No stations selected for import');
      return;
    }

    setIsImporting(true);
    setError(null);
    setImportProgress({ current: 0, total: stationsToImport.length });
    setStep(3);

    try {
      const stationData = stationsToImport.map((station) => ({
        nameEn: station.name,
        nameAr: station.name, // Fallback, could be enhanced with translation
        cityId: selectedCityId!,
        latitude: station.latitude,
        longitude: station.longitude,
        stationType: stationType,
      }));

      // Import in batches
      for (let i = 0; i < stationData.length; i++) {
        try {
          await stationsApi.create(stationData[i]);
          setImportProgress({ current: i + 1, total: stationData.length });
        } catch (err) {
          console.error(`Failed to import station ${stationData[i].nameEn}:`, err);
        }
      }

      alert(`Successfully imported ${importProgress.current} out of ${importProgress.total} stations`);
      onImportComplete();
      onClose();
    } catch (err: any) {
      setError(err.message || 'Failed to import stations');
    } finally {
      setIsImporting(false);
    }
  };

  if (!isOpen) return null;

  const selectedCity = cities.find((c) => c.id === selectedCityId);

  return (
    <div
      className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50"
      onClick={onClose}
    >
      <div
        className="bg-white rounded-lg shadow-xl w-full max-w-3xl mx-4 max-h-[90vh] flex flex-col"
        onClick={(e) => e.stopPropagation()}
      >
        {/* Header */}
        <div className="flex items-center justify-between p-6 border-b border-gray-200">
          <div>
            <h2 className="text-xl font-bold text-gray-900">Bulk Import Stations</h2>
            <p className="text-sm text-gray-500 mt-1">
              Step {step} of 3: {step === 1 ? 'Configure' : step === 2 ? 'Preview' : 'Import'}
            </p>
          </div>
          <button
            onClick={onClose}
            disabled={isImporting}
            className="text-gray-400 hover:text-gray-600 transition-colors"
          >
            <X size={24} />
          </button>
        </div>

        {/* Content */}
        <div className="flex-1 overflow-y-auto p-6">
          {error && (
            <div className="mb-4 p-3 bg-red-50 border border-red-200 rounded-lg text-red-800 text-sm">
              {error}
            </div>
          )}

          {/* Step 1: Configure Search */}
          {step === 1 && (
            <div className="space-y-4">
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-2">
                  Select City <span className="text-red-500">*</span>
                </label>
                <select
                  value={selectedCityId || ''}
                  onChange={(e) => setSelectedCityId(parseInt(e.target.value))}
                  className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-admin-primary-500"
                >
                  <option value="">Select a city...</option>
                  {cities.map((city) => (
                    <option key={city.id} value={city.id}>
                      {city.nameEn}
                    </option>
                  ))}
                </select>
              </div>

              <div>
                <label className="block text-sm font-medium text-gray-700 mb-2">
                  Search Radius (km)
                </label>
                <input
                  type="number"
                  value={radiusKm}
                  onChange={(e) => setRadiusKm(parseInt(e.target.value))}
                  min="1"
                  max="50"
                  className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-admin-primary-500"
                />
              </div>

              <div>
                <label className="block text-sm font-medium text-gray-700 mb-2">
                  Station Type
                </label>
                <select
                  value={stationType}
                  onChange={(e) => setStationType(e.target.value)}
                  className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-admin-primary-500"
                >
                  <option value="train_station">Train Station</option>
                  <option value="bus_station">Bus Station</option>
                  <option value="transit_station">Transit Station</option>
                  <option value="subway_station">Subway Station</option>
                </select>
              </div>
            </div>
          )}

          {/* Step 2: Preview Results */}
          {step === 2 && (
            <div>
              <div className="flex items-center justify-between mb-4">
                <p className="text-sm text-gray-600">
                  Found {searchResults.length} stations near {selectedCity?.nameEn}
                </p>
                <button
                  onClick={handleSelectAll}
                  className="text-sm text-admin-primary-600 hover:text-admin-primary-700 font-medium"
                >
                  {selectedStations.size === searchResults.length ? 'Deselect All' : 'Select All'}
                </button>
              </div>

              <div className="space-y-2 max-h-96 overflow-y-auto">
                {searchResults.map((station) => {
                  const isDuplicate = isStationDuplicate(station);
                  return (
                    <label
                      key={station.placeId}
                      className={`flex items-start gap-3 p-3 border rounded-lg cursor-pointer transition-colors ${
                        isDuplicate
                          ? 'bg-yellow-50 border-yellow-200'
                          : selectedStations.has(station.placeId)
                          ? 'bg-admin-primary-50 border-admin-primary-200'
                          : 'border-gray-200 hover:bg-gray-50'
                      }`}
                    >
                      <input
                        type="checkbox"
                        checked={selectedStations.has(station.placeId)}
                        onChange={() => handleToggleStation(station.placeId)}
                        disabled={isDuplicate}
                        className="mt-1 text-admin-primary-600 focus:ring-admin-primary-500"
                      />
                      <div className="flex-1 min-w-0">
                        <p className="font-medium text-gray-900">{station.name}</p>
                        <p className="text-sm text-gray-600 truncate">{station.address}</p>
                        <p className="text-xs text-gray-400 mt-1">
                          {station.latitude.toFixed(4)}, {station.longitude.toFixed(4)}
                        </p>
                        {isDuplicate && (
                          <p className="text-xs text-yellow-700 mt-1">⚠️ Already exists</p>
                        )}
                      </div>
                    </label>
                  );
                })}
              </div>
            </div>
          )}

          {/* Step 3: Import Progress */}
          {step === 3 && (
            <div className="text-center py-8">
              <div className="mb-4">
                <div className="w-full bg-gray-200 rounded-full h-2 mb-2">
                  <div
                    className="bg-admin-primary-600 h-2 rounded-full transition-all duration-300"
                    style={{
                      width: `${(importProgress.current / importProgress.total) * 100}%`,
                    }}
                  />
                </div>
                <p className="text-sm text-gray-600">
                  Importing {importProgress.current} of {importProgress.total} stations...
                </p>
              </div>
              {isImporting && (
                <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-admin-primary-600 mx-auto" />
              )}
            </div>
          )}
        </div>

        {/* Footer Actions */}
        <div className="flex gap-3 p-6 border-t border-gray-200">
          {step === 1 && (
            <>
              <button
                onClick={onClose}
                className="flex-1 px-4 py-2 border border-gray-300 text-gray-700 rounded-lg hover:bg-gray-50 transition-colors"
              >
                Cancel
              </button>
              <button
                onClick={handleSearch}
                disabled={!selectedCityId || isSearching}
                className="flex-1 px-4 py-2 bg-admin-primary-600 text-white rounded-lg hover:bg-admin-primary-700 transition-colors disabled:opacity-50 flex items-center justify-center gap-2"
              >
                {isSearching ? 'Searching...' : 'Search Stations'}
                <ChevronRight size={18} />
              </button>
            </>
          )}

          {step === 2 && (
            <>
              <button
                onClick={() => setStep(1)}
                className="flex-1 px-4 py-2 border border-gray-300 text-gray-700 rounded-lg hover:bg-gray-50 transition-colors flex items-center justify-center gap-2"
              >
                <ChevronLeft size={18} />
                Back
              </button>
              <button
                onClick={handleImport}
                disabled={selectedStations.size === 0}
                className="flex-1 px-4 py-2 bg-green-600 text-white rounded-lg hover:bg-green-700 transition-colors disabled:opacity-50"
              >
                Import {selectedStations.size} Station{selectedStations.size !== 1 ? 's' : ''}
              </button>
            </>
          )}
        </div>
      </div>
    </div>
  );
};

export default BulkImportModal;
