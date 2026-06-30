import { Wifi, WifiOff } from 'lucide-react';
import { useLanguage } from '../contexts/LanguageContext';
import { useNetworkStatus } from '../hooks/useNetworkStatus';

export default function ConnectionBanner() {
  const { t } = useLanguage();
  const { isOnline } = useNetworkStatus();

  if (isOnline) return null;

  return (
    <div
      role="status"
      className="bg-amber-600 text-white text-sm px-4 py-2 flex items-center justify-center gap-2"
    >
      <WifiOff className="h-4 w-4 flex-shrink-0" />
      <span>{t('network.offline.banner')}</span>
    </div>
  );
}

/** Small inline indicator for pages that had to fall back to cached data. */
export function CachedDataNotice() {
  const { t } = useLanguage();
  return (
    <div className="mb-4 flex items-start gap-2 rounded-lg border border-amber-200 bg-amber-50 px-4 py-3 text-sm text-amber-900">
      <Wifi className="h-4 w-4 mt-0.5 flex-shrink-0 text-amber-700" />
      <p>{t('network.cached.data.notice')}</p>
    </div>
  );
}
