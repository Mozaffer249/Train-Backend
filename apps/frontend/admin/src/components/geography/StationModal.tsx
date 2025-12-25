import { useState, useEffect, useCallback } from 'react';
import { X, MapPin, Search } from 'lucide-react';
import { GoogleMap, Marker, Autocomplete, Circle } from '@react-google-maps/api';
import { useGoogleMaps } from '../../hooks/useGoogleMaps';
import { Station, StationFormData, City } from '../../types/geography';
import { stationsApi, citiesApi } from '../../services/api';

interface StationModalProps {
  isOpen: boolean;
  onClose: () => void;
  onSuccess: () => void;
  station?: Station | null;
}

const mapContainerStyle = {
  width: '100%',
  height: '400px',
};

const sudanCenter = {
  lat: 15.5007,
  lng: 32.5599,
};

const StationModal = ({ isOpen, onClose, onSuccess, station }: StationModalProps) => {
  const { isLoaded } = useGoogleMaps();
  const [formData, setFormData] = useState<StationFormData>({
    code: '',
    nameAr: '',
    nameEn: '',
    cityId: 0,
    latitude: 0,
    longitude: 0,
    stationType: 'train_station',
    serviceRadiusKm: 5,
    googlePlaceId: '',
    formattedAddress: '',
  });
  const [cities, setCities] = useState<City[]>([]);
  const [mapCenter, setMapCenter] = useState(sudanCenter);
  const [markerPosition, setMarkerPosition] = useState<google.maps.LatLngLiteral | null>(null);
  const [autocomplete, setAutocomplete] = useState<google.maps.places.Autocomplete | null>(null);
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [isLoadingCities, setIsLoadingCities] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const isEditMode = !!station;

  // Load cities on mount
  useEffect(() => {
    if (isOpen) {
      loadCities();
    }
  }, [isOpen]);

  // Set form data when editing or city changes
  useEffect(() => {
    if (station) {
      setFormData({
        code: station.code,
        nameAr: station.nameAr,
        nameEn: station.nameEn,
        cityId: station.cityId,
        latitude: station.latitude,
        longitude: station.longitude,
        stationType: station.stationType || 'train_station',
        serviceRadiusKm: station.serviceRadiusKm || 5,
        googlePlaceId: station.googlePlaceId,
        formattedAddress: station.formattedAddress,
      });
      setMarkerPosition({ lat: station.latitude, lng: station.longitude });
      setMapCenter({ lat: station.latitude, lng: station.longitude });
    } else {
      setFormData({
        code: '',
        nameAr: '',
        nameEn: '',
        cityId: 0,
        latitude: 0,
        longitude: 0,
        stationType: 'train_station',
        serviceRadiusKm: 5,
        googlePlaceId: '',
        formattedAddress: '',
      });
      setMarkerPosition(null);
      setMapCenter(sudanCenter);
    }
    setError(null);
  }, [station, isOpen]);

  // Update map center when city is selected
  useEffect(() => {
    if (formData.cityId && cities.length > 0) {
      const selectedCity = cities.find(c => c.id === formData.cityId);
      if (selectedCity) {
        setMapCenter({ lat: selectedCity.latitude, lng: selectedCity.longitude });
      }
    }
  }, [formData.cityId, cities]);

  const loadCities = async () => {
    setIsLoadingCities(true);
    try {
      const data = await citiesApi.getAll();
      setCities(data);
    } catch (err: any) {
      setError('Failed to load cities: ' + err.message);
    } finally {
      setIsLoadingCities(false);
    }
  };

  const onPlaceChanged = () => {
    if (autocomplete !== null) {
      const place = autocomplete.getPlace();
      if (place.geometry && place.geometry.location) {
        const lat = place.geometry.location.lat();
        const lng = place.geometry.location.lng();
        
        setMarkerPosition({ lat, lng });
        setMapCenter({ lat, lng });
        setFormData({
          ...formData,
          latitude: lat,
          longitude: lng,
          nameEn: place.name || formData.nameEn,
          googlePlaceId: place.place_id || '',
          formattedAddress: place.formatted_address || '',
        });
      }
    }
  };

  const handleMapClick = useCallback((event: google.maps.MapMouseEvent) => {
    if (event.latLng) {
      const lat = event.latLng.lat();
      const lng = event.latLng.lng();
      setMarkerPosition({ lat, lng });
      setFormData((prev) => ({
        ...prev,
        latitude: lat,
        longitude: lng,
      }));
    }
  }, []);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError(null);

    // Validation
    if (!formData.code.trim()) {
      setError('Station code is required');
      return;
    }
    if (!formData.nameAr.trim()) {
      setError('Arabic name is required');
      return;
    }
    if (!formData.nameEn.trim()) {
      setError('English name is required');
      return;
    }
    if (!formData.cityId) {
      setError('Please select a city');
      return;
    }
    if (!formData.latitude || !formData.longitude) {
      setError('Please select a location on the map');
      return;
    }

    setIsSubmitting(true);

    try {
      if (isEditMode && station) {
        await stationsApi.update(station.id, formData);
      } else {
        await stationsApi.create(formData);
      }
      onSuccess();
      onClose();
    } catch (err: any) {
      setError(err.message || 'An error occurred');
    } finally {
      setIsSubmitting(false);
    }
  };

  const handleClose = () => {
    if (!isSubmitting) {
      onClose();
    }
  };

  if (!isOpen) return null;

  return (
    <div
      className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50 p-4"
      onClick={handleClose}
      role="dialog"
      aria-modal="true"
    >
      <div
        className="bg-white rounded-lg shadow-xl w-full max-w-5xl max-h-[90vh] overflow-y-auto"
        onClick={(e) => e.stopPropagation()}
      >
        {/* Header */}
        <div className="flex items-center justify-between p-6 border-b border-gray-200 sticky top-0 bg-white z-10">
          <h2 className="text-xl font-bold text-gray-900">
            {isEditMode ? 'Edit Station' : 'Add New Station'}
          </h2>
          <button
            onClick={handleClose}
            disabled={isSubmitting}
            className="text-gray-400 hover:text-gray-600 transition-colors"
            aria-label="Close modal"
          >
            <X size={24} />
          </button>
        </div>

        {/* Form */}
        <form onSubmit={handleSubmit} className="p-6">
          {error && (
            <div className="mb-4 p-3 bg-red-50 border border-red-200 rounded-lg text-red-800 text-sm">
              {error}
            </div>
          )}

          <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
            {/* Left Column - Form Fields */}
            <div className="space-y-4">
              <h3 className="text-lg font-semibold text-gray-900 mb-4">Station Information</h3>

              {/* Station Code */}
              <div>
                <label htmlFor="code" className="block text-sm font-medium text-gray-700 mb-1">
                  Station Code <span className="text-red-500">*</span>
                </label>
                <input
                  id="code"
                  type="text"
                  value={formData.code}
                  onChange={(e) => setFormData({ ...formData, code: e.target.value.toUpperCase() })}
                  className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-admin-primary-500"
                  placeholder="KHR"
                  disabled={isSubmitting || isEditMode}
                  required
                  maxLength={20}
                />
              </div>

              {/* Arabic Name */}
              <div>
                <label htmlFor="nameAr" className="block text-sm font-medium text-gray-700 mb-1">
                  Name (Arabic) <span className="text-red-500">*</span>
                </label>
                <input
                  id="nameAr"
                  type="text"
                  value={formData.nameAr}
                  onChange={(e) => setFormData({ ...formData, nameAr: e.target.value })}
                  className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-admin-primary-500"
                  placeholder="الاسم بالعربية"
                  disabled={isSubmitting}
                  required
                  dir="rtl"
                />
              </div>

              {/* English Name */}
              <div>
                <label htmlFor="nameEn" className="block text-sm font-medium text-gray-700 mb-1">
                  Name (English) <span className="text-red-500">*</span>
                </label>
                <input
                  id="nameEn"
                  type="text"
                  value={formData.nameEn}
                  onChange={(e) => setFormData({ ...formData, nameEn: e.target.value })}
                  className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-admin-primary-500"
                  placeholder="Station Name"
                  disabled={isSubmitting}
                  required
                />
              </div>

              {/* City Selection */}
              <div>
                <label htmlFor="cityId" className="block text-sm font-medium text-gray-700 mb-1">
                  City <span className="text-red-500">*</span>
                </label>
                <select
                  id="cityId"
                  value={formData.cityId}
                  onChange={(e) => setFormData({ ...formData, cityId: parseInt(e.target.value) })}
                  className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-admin-primary-500"
                  disabled={isSubmitting || isLoadingCities}
                  required
                >
                  <option value={0}>Select a city...</option>
                  {cities.map((city) => (
                    <option key={city.id} value={city.id}>
                      {city.nameEn} - {city.nameAr}
                    </option>
                  ))}
                </select>
                {isLoadingCities && (
                  <p className="text-sm text-gray-500 mt-1">Loading cities...</p>
                )}
              </div>

              {/* Station Type */}
              <div>
                <label htmlFor="stationType" className="block text-sm font-medium text-gray-700 mb-1">
                  Station Type
                </label>
                <select
                  id="stationType"
                  value={formData.stationType}
                  onChange={(e) => setFormData({ ...formData, stationType: e.target.value })}
                  className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-admin-primary-500"
                  disabled={isSubmitting}
                >
                  <option value="train_station">Train Station</option>
                  <option value="bus_station">Bus Station</option>
                  <option value="terminal">Terminal</option>
                </select>
              </div>

              {/* Service Radius */}
              <div>
                <label htmlFor="serviceRadiusKm" className="block text-sm font-medium text-gray-700 mb-1">
                  Service Radius (km)
                </label>
                <input
                  id="serviceRadiusKm"
                  type="number"
                  step="0.1"
                  value={formData.serviceRadiusKm || ''}
                  onChange={(e) => setFormData({ ...formData, serviceRadiusKm: parseFloat(e.target.value) })}
                  className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-admin-primary-500"
                  placeholder="5.0"
                  disabled={isSubmitting}
                />
              </div>

              {/* Coordinates */}
              <div className="grid grid-cols-2 gap-3">
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">
                    Latitude <span className="text-red-500">*</span>
                  </label>
                  <input
                    type="number"
                    step="any"
                    value={formData.latitude || ''}
                    onChange={(e) => {
                      const lat = parseFloat(e.target.value);
                      setFormData({ ...formData, latitude: lat });
                      if (formData.longitude) {
                        setMarkerPosition({ lat, lng: formData.longitude });
                      }
                    }}
                    className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-admin-primary-500 text-sm"
                    placeholder="15.5007"
                    required
                  />
                </div>
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">
                    Longitude <span className="text-red-500">*</span>
                  </label>
                  <input
                    type="number"
                    step="any"
                    value={formData.longitude || ''}
                    onChange={(e) => {
                      const lng = parseFloat(e.target.value);
                      setFormData({ ...formData, longitude: lng });
                      if (formData.latitude) {
                        setMarkerPosition({ lat: formData.latitude, lng });
                      }
                    }}
                    className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-admin-primary-500 text-sm"
                    placeholder="32.5599"
                    required
                  />
                </div>
              </div>
            </div>

            {/* Right Column - Map */}
            <div className="space-y-4">
              <h3 className="text-lg font-semibold text-gray-900 mb-4">
                <MapPin className="inline mr-2" size={20} />
                Select Location
              </h3>

              {isLoaded ? (
                <div className="space-y-3">
                  {/* Search Box */}
                  <Autocomplete
                    onLoad={setAutocomplete}
                    onPlaceChanged={onPlaceChanged}
                  >
                    <div className="relative">
                      <Search className="absolute left-3 top-1/2 transform -translate-y-1/2 text-gray-400" size={18} />
                      <input
                        type="text"
                        placeholder="Search for a station location..."
                        className="w-full pl-10 pr-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-admin-primary-500"
                      />
                    </div>
                  </Autocomplete>

                  {/* Map */}
                  <div className="border border-gray-300 rounded-lg overflow-hidden">
                    <GoogleMap
                      mapContainerStyle={mapContainerStyle}
                      center={mapCenter}
                      zoom={13}
                      onClick={handleMapClick}
                      options={{
                        mapTypeControl: false,
                        streetViewControl: false,
                      }}
                    >
                      {markerPosition && (
                        <>
                          <Marker position={markerPosition} />
                          {formData.serviceRadiusKm && (
                            <Circle
                              center={markerPosition}
                              radius={formData.serviceRadiusKm * 1000}
                              options={{
                                fillColor: '#3b82f6',
                                fillOpacity: 0.1,
                                strokeColor: '#3b82f6',
                                strokeOpacity: 0.5,
                                strokeWeight: 2,
                              }}
                            />
                          )}
                        </>
                      )}
                    </GoogleMap>
                  </div>

                  <p className="text-xs text-gray-500">
                    💡 Click on the map or search to set the station location
                  </p>
                </div>
              ) : (
                <div className="flex items-center justify-center h-[400px] bg-gray-100 rounded-lg">
                  <p className="text-gray-500">Loading map...</p>
                </div>
              )}
            </div>
          </div>

          {/* Actions */}
          <div className="flex gap-3 mt-6 pt-6 border-t border-gray-200">
            <button
              type="button"
              onClick={handleClose}
              disabled={isSubmitting}
              className="flex-1 px-4 py-2 border border-gray-300 text-gray-700 rounded-lg hover:bg-gray-50 transition-colors disabled:opacity-50"
            >
              Cancel
            </button>
            <button
              type="submit"
              disabled={isSubmitting || isLoadingCities || !formData.latitude || !formData.longitude}
              className="flex-1 px-4 py-2 bg-admin-primary-600 text-white rounded-lg hover:bg-admin-primary-700 transition-colors disabled:opacity-50"
            >
              {isSubmitting ? 'Saving...' : isEditMode ? 'Update Station' : 'Create Station'}
            </button>
          </div>
        </form>
      </div>
    </div>
  );
};

export default StationModal;
