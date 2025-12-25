import { Polygon } from '@react-google-maps/api';
import { Area, Governorate, City, BoundaryData } from '../../types/geography';

interface BoundaryPolygonsProps {
  areas: (Region & { boundaryData?: BoundaryData })[];
  governorates: (State & { boundaryData?: BoundaryData })[];
  cities: (City & { boundaryData?: BoundaryData })[];
  layerVisibility: {
    areas: boolean;
    governorates: boolean;
    cities: boolean;
  };
  onPolygonClick?: (entity: any, type: string) => void;
}

const BoundaryPolygons = ({
  regions,
  states,
  cities,
  layerVisibility,
  onPolygonClick,
}: BoundaryPolygonsProps) => {
  // Parse GeoJSON polygon string to Google Maps LatLng format
  const parseGeoJsonToPath = (geoJsonString?: string): google.maps.LatLngLiteral[] => {
    if (!geoJsonString) return [];
    
    try {
      const geoJson = JSON.parse(geoJsonString);
      if (geoJson.type === 'Polygon' && geoJson.coordinates && geoJson.coordinates[0]) {
        // GeoJSON format is [lng, lat], Google Maps expects {lat, lng}
        return geoJson.coordinates[0].map((coord: number[]) => ({
          lat: coord[1],
          lng: coord[0],
        }));
      }
    } catch (error) {
      console.error('Failed to parse GeoJSON:', error);
    }
    
    return [];
  };

  const polygonOptions = {
    strokeWeight: 2,
    fillOpacity: 0.3,
    clickable: true,
    draggable: false,
    editable: false,
    geodesic: false,
  };

  return (
    <>
      {/* Region Boundaries - Purple */}
      {layerVisibility.regions &&
        regions
          .filter((r: any) => r.boundaryData?.boundaryPolygon)
          .map((region: any) => {
            const path = parseGeoJsonToPath(region.boundaryData.boundaryPolygon);
            if (path.length === 0) return null;

            return (
              <Polygon
                key={`region-boundary-${region.id}`}
                paths={path}
                options={{
                  ...polygonOptions,
                  strokeColor: '#9333ea',
                  fillColor: '#9333ea',
                }}
                onClick={() => onPolygonClick?.(region, 'area')}
              />
            );
          })}

      {/* State Boundaries - Blue */}
      {layerVisibility.states &&
        states
          .filter((s: any) => s.boundaryData?.boundaryPolygon)
          .map((state: any) => {
            const path = parseGeoJsonToPath(state.boundaryData.boundaryPolygon);
            if (path.length === 0) return null;

            return (
              <Polygon
                key={`state-boundary-${state.id}`}
                paths={path}
                options={{
                  ...polygonOptions,
                  strokeColor: '#3b82f6',
                  fillColor: '#3b82f6',
                }}
                onClick={() => onPolygonClick?.(state, 'governorate')}
              />
            );
          })}

      {/* City Boundaries - Green */}
      {layerVisibility.cities &&
        cities
          .filter((c: any) => c.boundaryData?.boundaryPolygon)
          .map((city: any) => {
            const path = parseGeoJsonToPath(city.boundaryData.boundaryPolygon);
            if (path.length === 0) return null;

            return (
              <Polygon
                key={`city-boundary-${city.id}`}
                paths={path}
                options={{
                  ...polygonOptions,
                  strokeColor: '#10b981',
                  fillColor: '#10b981',
                }}
                onClick={() => onPolygonClick?.(city, 'city')}
              />
            );
          })}
    </>
  );
};

export default BoundaryPolygons;
