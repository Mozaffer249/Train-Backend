import { useState, useEffect, useCallback } from 'react';
import { X, MapPin, Search, AlertCircle } from 'lucide-react';
import { GoogleMap, Marker, Autocomplete, Polygon } from '@react-google-maps/api';
import { useGoogleMaps } from '../../hooks/useGoogleMaps';
import { City, CityFormData, CityValidationResult } from '../../types/geography';
import { citiesApi } from '../../services/api';

interface CityModalProps {
  isOpen: boolean;
  onClose: () => void;
  onSuccess: () => void;
  city?: City | null;
}

const mapContainerStyle = {
  width: '100%',
  height: '400px',
};

const sudanCenter = {
  lat: 15.5007,
  lng: 32.5599,
};

const CityModal = ({ isOpen, onClose, onSuccess, city }: CityModalProps) => {
  const { isLoaded } = useGoogleMaps();
  const [formData, setFormData] = useState<CityFormData>({
    nameAr: '',
    nameEn: '',
    latitude: 0,
    longitude: 0,
    googlePlaceId: '',
    formattedAddress: '',
  });
  const [mapCenter, setMapCenter] = useState(sudanCenter);
  const [markerPosition, setMarkerPosition] = useState<google.maps.LatLngLiteral | null>(null);
  const [boundaryPaths, setBoundaryPaths] = useState<google.maps.LatLngLiteral[]>([]);
  const [autocomplete, setAutocomplete] = useState<google.maps.places.Autocomplete | null>(null);
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [isValidatingLocation, setIsValidatingLocation] = useState(false);
  const [validationResult, setValidationResult] = useState<CityValidationResult | null>(null);

  const isEditMode = !!city;

  // Set form data when editing
  useEffect(() => {
    if (city) {
      setFormData({
        nameAr: city.nameAr,
        nameEn: city.nameEn,
        latitude: city.latitude,
        longitude: city.longitude,
        googlePlaceId: city.googlePlaceId,
        formattedAddress: city.formattedAddress,
        boundaryPolygon: city.boundaryPolygon,
        boundingBoxNorth: city.boundingBoxNorth,
        boundingBoxSouth: city.boundingBoxSouth,
        boundingBoxEast: city.boundingBoxEast,
        boundingBoxWest: city.boundingBoxWest,
      });
      setMarkerPosition({ lat: city.latitude, lng: city.longitude });
      setMapCenter({ lat: city.latitude, lng: city.longitude });
      
      // Draw existing boundary if available
      if (city.boundaryPolygon) {
        drawBoundaryPolygon(city.boundaryPolygon);
      }
    } else {
      setFormData({
        nameAr: '',
        nameEn: '',
        latitude: 0,
        longitude: 0,
        googlePlaceId: '',
        formattedAddress: '',
        boundaryPolygon: '',
        boundingBoxNorth: undefined,
        boundingBoxSouth: undefined,
        boundingBoxEast: undefined,
        boundingBoxWest: undefined,
      });
      setMarkerPosition(null);
      setMapCenter(sudanCenter);
      setBoundaryPaths([]);
    }
    setError(null);
    setValidationResult(null);
  }, [city, isOpen]);

  const drawBoundaryPolygon = (polygonJson: string) => {
    try {
      const geoJson = JSON.parse(polygonJson);
      const coordinates = geoJson.coordinates[0].map((coord: [number, number]) => ({
        lat: coord[1],
        lng: coord[0]
      }));
      setBoundaryPaths(coordinates);
    } catch (err) {
      console.error('Error parsing boundary polygon:', err);
      setBoundaryPaths([]);
    }
  };

  const validateAndSetLocation = async (lat: number, lng: number, placeData?: any) => {
    setIsValidatingLocation(true);
    setError(null);
    setValidationResult(null);

    try {
      const validation = await citiesApi.validateLocation(lat, lng);
      console.log('🔍 Validation Response:', validation);
      console.log('📍 Boundary Data:', {
        polygon: validation.suggestedData?.boundaryPolygon,
        north: validation.suggestedData?.boundingBoxNorth,
        south: validation.suggestedData?.boundingBoxSouth,
        east: validation.suggestedData?.boundingBoxEast,
        west: validation.suggestedData?.boundingBoxWest,
      });
      setValidationResult(validation);

      if (!validation.isValid) {
        // Duplicate detected
        setError(validation.message);
        setMarkerPosition({ lat, lng });
        setFormData({
          ...formData,
          latitude: lat,
          longitude: lng,
        });
        setBoundaryPaths([]);
      } else {
        // Valid location - auto-fill with suggested data
        setMarkerPosition({ lat, lng });
        setFormData({
          ...formData,
          nameEn: validation.suggestedData?.nameEn || placeData?.name || formData.nameEn,
          nameAr: formData.nameAr, // User must fill Arabic name
          latitude: lat,
          longitude: lng,
          googlePlaceId: validation.suggestedData?.googlePlaceId || placeData?.place_id || '',
          formattedAddress: validation.suggestedData?.formattedAddress || placeData?.formatted_address || '',
          boundaryPolygon: validation.suggestedData?.boundaryPolygon,
          boundingBoxNorth: validation.suggestedData?.boundingBoxNorth,
          boundingBoxSouth: validation.suggestedData?.boundingBoxSouth,
          boundingBoxEast: validation.suggestedData?.boundingBoxEast,
          boundingBoxWest: validation.suggestedData?.boundingBoxWest,
        });
        setMapCenter({ lat, lng });
        
        // Draw boundary on map
        if (validation.suggestedData?.boundaryPolygon) {
          drawBoundaryPolygon(validation.suggestedData.boundaryPolygon);
        }
      }
    } catch (err: any) {
      console.error('Validation error:', err);
      // If validation fails, still allow setting location but show warning
      setMarkerPosition({ lat, lng });
      setFormData({
        ...formData,
        latitude: lat,
        longitude: lng,
        nameEn: placeData?.name || formData.nameEn,
        googlePlaceId: placeData?.place_id || '',
        formattedAddress: placeData?.formatted_address || '',
      });
      setMapCenter({ lat, lng });
      setBoundaryPaths([]);
    } finally {
      setIsValidatingLocation(false);
    }
  };

  const onPlaceChanged = () => {
    if (autocomplete !== null) {
      const place = autocomplete.getPlace();
      if (place.geometry && place.geometry.location) {
        const lat = place.geometry.location.lat();
        const lng = place.geometry.location.lng();
        validateAndSetLocation(lat, lng, place);
      }
    }
  };

  const handleMapClick = useCallback((event: google.maps.MapMouseEvent) => {
    if (event.latLng) {
      const lat = event.latLng.lat();
      const lng = event.latLng.lng();
      validateAndSetLocation(lat, lng);
    }
  }, []);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError(null);

    // Validation
    if (!formData.nameAr.trim()) {
      setError('Arabic name is required');
      return;
    }
    if (!formData.nameEn.trim()) {
      setError('English name is required');
      return;
    }
    if (!formData.latitude || !formData.longitude) {
      setError('Please select a location on the map');
      return;
    }

    setIsSubmitting(true);

    console.log('📤 Submitting formData:', formData);
    console.log('🗺️ Boundary fields in formData:', {
      boundaryPolygon: formData.boundaryPolygon,
      boundingBoxNorth: formData.boundingBoxNorth,
      boundingBoxSouth: formData.boundingBoxSouth,
      boundingBoxEast: formData.boundingBoxEast,
      boundingBoxWest: formData.boundingBoxWest,
    });

    try {
      if (isEditMode && city) {
        await citiesApi.update(city.id, formData);
      } else {
        await citiesApi.create(formData);
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
        className="bg-white rounded-lg shadow-xl w-full max-w-4xl max-h-[90vh] overflow-y-auto"
        onClick={(e) => e.stopPropagation()}
      >
        {/* Header */}
        <div className="flex items-center justify-between p-6 border-b border-gray-200 sticky top-0 bg-white z-10">
          <h2 className="text-xl font-bold text-gray-900">
            {isEditMode ? 'Edit City' : 'Add New City'}
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

          {isValidatingLocation && (
            <div className="mb-4 p-3 bg-blue-50 border border-blue-200 rounded-lg text-blue-800 text-sm flex items-center gap-2">
              <div className="animate-spin rounded-full h-4 w-4 border-b-2 border-blue-800"></div>
              Validating location...
            </div>
          )}

          {validationResult && validationResult.isValid && validationResult.distanceKm && (
            <div className="mb-4 p-3 bg-yellow-50 border border-yellow-200 rounded-lg text-yellow-800 text-sm">
              <AlertCircle className="inline mr-2" size={16} />
              {validationResult.message}
            </div>
          )}

          <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
            {/* Left Column - Form Fields */}
            <div className="space-y-4">
              <h3 className="text-lg font-semibold text-gray-900 mb-4">City Information</h3>

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
                  placeholder="Name in English"
                  disabled={isSubmitting}
                  required
                />
              </div>

              {/* Coordinates Display */}
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
                        setMapCenter({ lat, lng: formData.longitude });
                      }
                    }}
                    className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-admin-primary-500"
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
                        setMapCenter({ lat: formData.latitude, lng });
                      }
                    }}
                    className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-admin-primary-500"
                    placeholder="32.5599"
                    required
                  />
                </div>
              </div>

              {/* Formatted Address */}
              {formData.formattedAddress && (
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">
                    Address
                  </label>
                  <p className="text-sm text-gray-600 p-2 bg-gray-50 rounded">
                    {formData.formattedAddress}
                  </p>
                </div>
              )}
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
                        placeholder="Search for a place..."
                        className="w-full pl-10 pr-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-admin-primary-500"
                      />
                    </div>
                  </Autocomplete>

                  {/* Map */}
                  <div className="border border-gray-300 rounded-lg overflow-hidden">
                    <GoogleMap
                      mapContainerStyle={mapContainerStyle}
                      center={mapCenter}
                      zoom={12}
                      onClick={handleMapClick}
                      options={{
                        mapTypeControl: false,
                        streetViewControl: false,
                      }}
                    >
                      {markerPosition && <Marker position={markerPosition} />}
                    </GoogleMap>
                  </div>

                  <p className="text-xs text-gray-500">
                    💡 Click on the map or search for a place to set the location
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
              disabled={
                isSubmitting || 
                isValidatingLocation ||
                !formData.latitude || 
                !formData.longitude ||
                (validationResult && !validationResult.isValid)
              }
              className="flex-1 px-4 py-2 bg-admin-primary-600 text-white rounded-lg hover:bg-admin-primary-700 transition-colors disabled:opacity-50"
            >
              {isSubmitting ? 'Saving...' : 
               isValidatingLocation ? 'Validating...' :
               isEditMode ? 'Update City' : 'Create City'}
            </button>
          </div>
        </form>
      </div>
    </div>
  );
};

export default CityModal;
