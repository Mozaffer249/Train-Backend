import { useGovernorate, useCallback } from 'react';
import { DrawingManager } from '@react-google-maps/api';
import { BoundaryData } from '../../types/geography';

interface BoundaryDrawerProps {
  isDrawing: boolean;
  entityType: 'area' | 'governorate' | 'city';
  onPolygonComplete: (boundaryData: BoundaryData) => void;
  onCancel: () => void;
}

const BoundaryDrawer = ({
  isDrawing,
  entityType,
  onPolygonComplete,
  onCancel,
}: BoundaryDrawerProps) => {
  const [drawingManager, setDrawingManager] = useState<google.maps.drawing.DrawingManager | null>(null);

  const handlePolygonComplete = useCallback(
    (polygon: google.maps.Polygon) => {
      const path = polygon.getPath();
      const coordinates: number[][] = [];

      // Extract coordinates from polygon path
      for (let i = 0; i < path.getLength(); i++) {
        const point = path.getAt(i);
        coordinates.push([point.lng(), point.lat()]);
      }

      // Close the polygon by adding first point at the end
      if (coordinates.length > 0) {
        coordinates.push(coordinates[0]);
      }

      // Create GeoJSON
      const geoJson = {
        type: 'Polygon',
        coordinates: [coordinates],
      };

      // Calculate bounding box
      const lats = coordinates.map((c) => c[1]);
      const lngs = coordinates.map((c) => c[0]);

      const boundaryData: BoundaryData = {
        boundaryPolygon: JSON.stringify(geoJson),
        boundingBoxNorth: Math.max(...lats),
        boundingBoxSouth: Math.min(...lats),
        boundingBoxEast: Math.max(...lngs),
        boundingBoxWest: Math.min(...lngs),
      };

      // Remove the polygon from map
      polygon.setMap(null);

      // Call parent handler
      onPolygonComplete(boundaryData);
    },
    [onPolygonComplete]
  );

  // Get color based on entity type
  const getColor = () => {
    switch (entityType) {
      case 'area':
        return '#9333ea'; // Purple
      case 'governorate':
        return '#3b82f6'; // Blue
      case 'city':
        return '#10b981'; // Green
      default:
        return '#6b7280'; // Gray
    }
  };

  if (!isDrawing) return null;

  return (
    <>
      <DrawingManager
        onLoad={setDrawingManager}
        onPolygonComplete={handlePolygonComplete}
        options={{
          drawingMode: google.maps.drawing.OverlayType.POLYGON,
          drawingControl: true,
          drawingControlOptions: {
            position: google.maps.ControlPosition.TOP_CENTER,
            drawingModes: [google.maps.drawing.OverlayType.POLYGON],
          },
          polygonOptions: {
            strokeColor: getColor(),
            strokeOpacity: 1,
            strokeWeight: 2,
            fillColor: getColor(),
            fillOpacity: 0.3,
            clickable: true,
            editable: false,
            draggable: false,
          },
        }}
      />

      {/* Cancel Button Overlay */}
      <div className="absolute top-4 left-1/2 transform -translate-x-1/2 z-10">
        <div className="bg-white rounded-lg shadow-lg p-4 flex items-center gap-4">
          <p className="text-sm font-medium text-gray-700">
            Click on the map to draw {entityType} boundary
          </p>
          <button
            onClick={onCancel}
            className="px-4 py-2 bg-red-500 text-white rounded hover:bg-red-600 transition-colors text-sm font-medium"
          >
            Cancel Drawing
          </button>
        </div>
      </div>
    </>
  );
};

export default BoundaryDrawer;
