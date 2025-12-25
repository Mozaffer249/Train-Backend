import { useState } from 'react';
import { Marker, InfoWindow } from '@react-google-maps/api';
import { Area, Governorate, City, Station } from '../../types/geography';

interface EntityMarkersProps {
  areas: Area[];
  governorates: Governorate[];
  cities: City[];
  stations: Station[];
  layerVisibility: {
    areas: boolean;
    governorates: boolean;
    cities: boolean;
    stations: boolean;
  };
  onMarkerClick?: (entity: Area | Governorate | City | Station, type: string) => void;
}

const EntityMarkers = ({
  areas,
  governorates,
  cities,
  stations,
  layerVisibility,
  onMarkerClick,
}: EntityMarkersProps) => {
  const [selectedMarker, setSelectedMarker] = useState<{
    entity: any;
    type: string;
  } | null>(null);

  // Custom marker icons with colors
  const getMarkerIcon = (color: string) => ({
    path: google.maps.SymbolPath.CIRCLE,
    fillColor: color,
    fillOpacity: 1,
    strokeColor: '#ffffff',
    strokeWeight: 2,
    scale: 8,
  });

  const handleMarkerClick = (entity: any, type: string) => {
    setSelectedMarker({ entity, type });
    onMarkerClick?.(entity, type);
  };

  return (
    <>
      {/* Region Markers - Purple */}
      {layerVisibility.areas &&
        areas
          .filter((r) => r.latitude && r.longitude)
          .map((area: any) => (
            <Marker
              key={`area-${area.id}`}
              position={{ lat: area.latitude, lng: area.longitude }}
              icon={getMarkerIcon('#9333ea')}
              title={area.nameEn}
              onClick={() => handleMarkerClick(area, 'area')}
            />
          ))}

      {/* Governorate Markers - Blue */}
      {layerVisibility.governorates &&
        governorates
          .filter((g) => g.latitude && g.longitude)
          .map((governorate: any) => (
            <Marker
              key={`governorate-${governorate.id}`}
              position={{ lat: governorate.latitude, lng: governorate.longitude }}
              icon={getMarkerIcon('#3b82f6')}
              title={governorate.nameEn}
              onClick={() => handleMarkerClick(governorate, 'governorate')}
            />
          ))}

      {/* City Markers - Green */}
      {layerVisibility.cities &&
        cities
          .filter((c) => c.latitude && c.longitude)
          .map((city: any) => (
            <Marker
              key={`city-${city.id}`}
              position={{ lat: city.latitude, lng: city.longitude }}
              icon={getMarkerIcon('#10b981')}
              title={city.nameEn}
              onClick={() => handleMarkerClick(city, 'city')}
            />
          ))}

      {/* Station Markers - Red */}
      {layerVisibility.stations &&
        stations
          .filter((s) => s.latitude && s.longitude)
          .map((station) => (
            <Marker
              key={`station-${station.id}`}
              position={{ lat: station.latitude!, lng: station.longitude! }}
              icon={getMarkerIcon('#ef4444')}
              title={station.nameEn}
              onClick={() => handleMarkerClick(station, 'station')}
            />
          ))}

      {/* Info Window */}
      {selectedMarker && selectedMarker.entity.latitude && selectedMarker.entity.longitude && (
        <InfoWindow
          position={{
            lat: selectedMarker.entity.latitude,
            lng: selectedMarker.entity.longitude,
          }}
          onCloseClick={() => setSelectedMarker(null)}
        >
          <div className="p-2">
            <h3 className="font-bold text-lg mb-1">
              {selectedMarker.entity.nameEn}
            </h3>
            <p className="text-sm text-gray-600 mb-1" dir="rtl">
              {selectedMarker.entity.nameAr}
            </p>
            <p className="text-xs text-gray-500 capitalize">
              {selectedMarker.type}
            </p>
            {selectedMarker.type === 'governorate' && selectedMarker.entity.areaName && (
              <p className="text-xs text-gray-500">
                Area: {selectedMarker.entity.areaName}
              </p>
            )}
            {selectedMarker.type === 'city' && (
              <>
                {selectedMarker.entity.governorateName && (
                  <p className="text-xs text-gray-500">
                    Governorate: {selectedMarker.entity.governorateName}
                  </p>
                )}
                {selectedMarker.entity.areaName && (
                  <p className="text-xs text-gray-500">
                    Area: {selectedMarker.entity.areaName}
                  </p>
                )}
              </>
            )}
            {selectedMarker.type === 'station' && (
              <>
                {selectedMarker.entity.cityName && (
                  <p className="text-xs text-gray-500">
                    City: {selectedMarker.entity.cityName}
                  </p>
                )}
                {selectedMarker.entity.stationType && (
                  <p className="text-xs text-gray-500">
                    Type: {selectedMarker.entity.stationType}
                  </p>
                )}
              </>
            )}
            <p className="text-xs text-gray-400 mt-1">
              {selectedMarker.entity.latitude.toFixed(4)}, {selectedMarker.entity.longitude.toFixed(4)}
            </p>
          </div>
        </InfoWindow>
      )}
    </>
  );
};

export default EntityMarkers;
