// Read-only offline cache for weak-network environments.
// Data is stored locally for display only — booking still requires live API.

const PREFIX = 'st_customer_';
const TTL_MS = 24 * 60 * 60 * 1000; // 24 hours

interface CacheEntry<T> {
  data: T;
  savedAt: number;
}

function read<T>(key: string): T | null {
  try {
    const raw = localStorage.getItem(PREFIX + key);
    if (!raw) return null;
    const entry = JSON.parse(raw) as CacheEntry<T>;
    if (Date.now() - entry.savedAt > TTL_MS) {
      localStorage.removeItem(PREFIX + key);
      return null;
    }
    return entry.data;
  } catch {
    return null;
  }
}

function write<T>(key: string, data: T): void {
  try {
    const entry: CacheEntry<T> = { data, savedAt: Date.now() };
    localStorage.setItem(PREFIX + key, JSON.stringify(entry));
  } catch {
    // Quota exceeded or private mode — ignore silently.
  }
}

export interface CachedSearchParams {
  originStationId: number;
  destinationStationId: number;
  originName: string;
  destinationName: string;
  date: string;
  passengers: number;
  class: string;
}

export const offlineCache = {
  getStations: () => read<import('../types/api').StationDto[]>('stations'),
  setStations: (data: import('../types/api').StationDto[]) => write('stations', data),

  getLastSearch: () => read<CachedSearchParams>('last_search'),
  setLastSearch: (data: CachedSearchParams) => write('last_search', data),

  getSearchResults: (cacheKey: string) => read<unknown[]>('search_' + cacheKey),
  setSearchResults: (cacheKey: string, data: unknown[]) => write('search_' + cacheKey, data),

  getBookings: () => read<import('../types/api').BookingDto[]>('bookings'),
  setBookings: (data: import('../types/api').BookingDto[]) => write('bookings', data),
};

export function searchCacheKey(params: CachedSearchParams): string {
  return `${params.originStationId}_${params.destinationStationId}_${params.date}_${params.class}`;
}
