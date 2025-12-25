 import { useJsApiLoader  } from "@react-google-maps/api";
import { createContext, useContext } from "react";
const libraries = ["places", "geometry"];
const MapContext = createContext({ isLoaded: false });
export function MapProvider({ children }: { children: React.ReactNode }) {
  const { isLoaded } = useJsApiLoader({
    id: "google-map-script",
    googleMapsApiKey: import.meta.env.VITE_GOOGLE_MAPS_API_KEY,
    libraries,
  });
  return (
    <MapContext.Provider value={{ isLoaded }}>{children}</MapContext.Provider>
  );
}
export const useMapContext = () => {
  return useContext(MapContext);
};