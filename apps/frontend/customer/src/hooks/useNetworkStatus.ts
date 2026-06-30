import { useEffect, useState } from 'react';

export type NetworkQuality = 'online' | 'offline';

export function useNetworkStatus() {
  const [quality, setQuality] = useState<NetworkQuality>(
    typeof navigator !== 'undefined' && navigator.onLine ? 'online' : 'offline',
  );

  useEffect(() => {
    const onOnline = () => setQuality('online');
    const onOffline = () => setQuality('offline');
    window.addEventListener('online', onOnline);
    window.addEventListener('offline', onOffline);
    return () => {
      window.removeEventListener('online', onOnline);
      window.removeEventListener('offline', onOffline);
    };
  }, []);

  return {
    quality,
    isOnline: quality === 'online',
  };
}
