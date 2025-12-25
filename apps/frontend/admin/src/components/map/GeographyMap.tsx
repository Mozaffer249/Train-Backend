import { useState, useEffect } from 'react';
import { GoogleMap, Marker, InfoWindow } from '@react-google-maps/api';
import { useGoogleMaps } from '../../hooks/useGoogleMaps';
import { City, Station } from '../../types/geography';
import CityModal from '../geography/CityModal';
import StationModal from '../geography/StationModal';

interface GeographyMapProps {
  cities: City[];
  stations: Station[];
  onRefresh: () => void;
}

const mapContainerStyle = {
  width: '100%',
  height: 'calc(100vh - 200px)',
};

const center = {
  lat: 15.5007,
  lng: 32.5599,
};

const GeographyMap = ({ cities, stations, onRefresh }: GeographyMapProps) => {
  const { isLoaded, loadError } = useGoogleMaps();
  
  // Map state
  const [layerVisibility, setLayerVisibility] = useState({
    cities: true,
    stations: true,
  });

  // Modal states
  const [isCityModalOpen, setIsCityModalOpen] = useState(false);
  const [selectedCity, setSelectedCity] = useState<City | null>(null);
  
  const [isStationModalOpen, setIsStationModalOpen] = useState(false);
  const [selectedStation, setSelectedStation] = useState<Station | null>(null);

  // InfoWindow state
  const [selectedMarker, setSelectedMarker] = useState<{
    entity: City | Station;
    type: 'city' | 'station';
  } | null>(null);

  const handleCityClick = (city: City) => {
    setSelectedMarker({ entity: city, type: 'city' });
  };

  const handleStationClick = (station: Station) => {
    setSelectedMarker({ entity: station, type: 'station' });
  };

  const handleEditEntity = () => {
    if (!selectedMarker) return;

    if (selectedMarker.type === 'city') {
      setSelectedCity(selectedMarker.entity as City);
      setIsCityModalOpen(true);
    } else {
      setSelectedStation(selectedMarker.entity as Station);
      setIsStationModalOpen(true);
    }
    setSelectedMarker(null);
  };

  const handleSuccess = () => {
    onRefresh();
  };

  if (loadError) {
    return (
      <div className="flex items-center justify-center h-96 bg-red-50 rounded-lg">
        <p className="text-red-800">Error loading maps: {loadError.message}</p>
      </div>
    );
  }

  if (!isLoaded) {
    return (
      <div className="flex items-center justify-center h-96 bg-gray-100 rounded-lg">
        <p className="text-gray-500">Loading map...</p>
      </div>
    );
  }

  return (
    <div className="relative">
      {/* Layer Controls */}
      <div className="absolute top-4 right-4 bg-white rounded-lg shadow-lg p-4 z-10">
        <h3 className="text-sm font-semibold text-gray-900 mb-3">Map Layers</h3>
        <div className="space-y-2">
          <label className="flex items-center gap-2 cursor-pointer">
            <input
              type="checkbox"
              checked={layerVisibility.cities}
              onChange={(e) =>
                setLayerVisibility({ ...layerVisibility, cities: e.target.checked })
              }
              className="rounded text-admin-primary-600 focus:ring-admin-primary-500"
            />
            <span className="text-sm text-gray-700">Cities</span>
            <span className="text-xs text-gray-500">({cities.length})</span>
          </label>
          <label className="flex items-center gap-2 cursor-pointer">
            <input
              type="checkbox"
              checked={layerVisibility.stations}
              onChange={(e) =>
                setLayerVisibility({ ...layerVisibility, stations: e.target.checked })
              }
              className="rounded text-admin-primary-600 focus:ring-admin-primary-500"
            />
            <span className="text-sm text-gray-700">Stations</span>
            <span className="text-xs text-gray-500">({stations.length})</span>
          </label>
        </div>
      </div>

      {/* Map */}
      <GoogleMap
        mapContainerStyle={mapContainerStyle}
        center={center}
        zoom={6}
        options={{
          mapTypeControl: true,
          fullscreenControl: true,
        }}
      >
        {/* City Markers */}
        {layerVisibility.cities &&
          cities.map((city) => (
            <Marker
              key={`city-${city.id}`}
              position={{ lat: city.latitude, lng: city.longitude }}
              icon={{
                path: google.maps.SymbolPath.CIRCLE,
                scale: 8,
                fillColor: '#10b981',
                fillOpacity: 1,
                strokeColor: '#fff',
                strokeWeight: 2,
              }}
              title={city.nameEn}
              onClick={() => handleCityClick(city)}
            />
          ))}

        {/* Station Markers */}
        {layerVisibility.stations &&
          stations.map((station) => (
            <Marker
              key={`station-${station.id}`}
              position={{ lat: station.latitude, lng: station.longitude }}
              icon={{
                path: google.maps.SymbolPath.CIRCLE,
                scale: 6,
                fillColor: '#3b82f6',
                fillOpacity: 1,
                strokeColor: '#fff',
                strokeWeight: 2,
              }}
              title={station.nameEn}
              onClick={() => handleStationClick(station)}
            />
          ))}

        {/* InfoWindow */}
        {selectedMarker && (
          <InfoWindow
            position={
              'latitude' in selectedMarker.entity
                ? { lat: selectedMarker.entity.latitude, lng: selectedMarker.entity.longitude }
                : { lat: 0, lng: 0 }
            }
            onCloseClick={() => setSelectedMarker(null)}
          >
            <div className="p-2">
              <h3 className="font-semibold text-gray-900 mb-1">
                {selectedMarker.entity.nameEn}
              </h3>
              <p className="text-sm text-gray-600 mb-1" dir="rtl">
                {selectedMarker.entity.nameAr}
              </p>
              <p className="text-xs text-gray-500 capitalize mb-2">
                {selectedMarker.type}
              </p>
              {selectedMarker.type === 'station' && 'cityName' in selectedMarker.entity && (
                <p className="text-xs text-gray-500 mb-2">
                  City: {selectedMarker.entity.cityName}
                </p>
              )}
              <button
                onClick={handleEditEntity}
                className="text-xs text-admin-primary-600 hover:text-admin-primary-800 font-medium"
              >
                Edit {selectedMarker.type}
              </button>
            </div>
          </InfoWindow>
        )}
      </GoogleMap>

      {/* Modals */}
      <CityModal
        isOpen={isCityModalOpen}
        onClose={() => setIsCityModalOpen(false)}
        onSuccess={handleSuccess}
        city={selectedCity}
      />
      <StationModal
        isOpen={isStationModalOpen}
        onClose={() => setIsStationModalOpen(false)}
        onSuccess={handleSuccess}
        station={selectedStation}
      />
    </div>
  );
};

export default GeographyMap;
