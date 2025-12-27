import { useLoadScript } from '@react-google-maps/api';

const libraries: ('places' | 'drawing' | 'geometry')[] = ['places', 'drawing', 'geometry'];

export const useGoogleMaps = () => {
  const { isLoaded, loadError } = useLoadScript({
    googleMapsApiKey: import.meta.env.VITE_GOOGLE_MAPS_API_KEY || 'AIzaSyBwgPpXdlERV4l4OfuahpcTksHME8HU6H0',
    libraries,
  });

  return { isLoaded, loadError };
};
