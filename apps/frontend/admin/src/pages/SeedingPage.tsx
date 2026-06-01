import { useState } from 'react';
import { Database, Download, MapPin, Globe, AlertCircle, CheckCircle } from 'lucide-react';

interface SeedingResult {
  added: number;
  skipped: number;
  failed: number;
  errors: string[];
  warnings: string[];
}

interface CompleteSeedingResult {
  regions: SeedingResult;
  states: SeedingResult;
  cities: SeedingResult;
  totalAdded: number;
  totalSkipped: number;
  totalFailed: number;
}

const SeedingPage = () => {
  const [isSeedingAll, setIsSeedingAll] = useState(false);
  const [isSeedingRegions, setIsSeedingRegions] = useState(false);
  const [isSeedingStates, setIsSeedingStates] = useState(false);
  const [isSeedingCities, setIsSeedingCities] = useState(false);
  const [result, setResult] = useState<CompleteSeedingResult | null>(null);
  const [error, setError] = useState<string | null>(null);

  const API_BASE_URL = import.meta.env.VITE_API_URL || 'http://localhost:8080';

  const seedAll = async () => {
    setIsSeedingAll(true);
    setError(null);
    setResult(null);

    try {
      const token = localStorage.getItem('admin_token');
      const response = await fetch(`${API_BASE_URL}/Api/V1/Admin/Seeding/Geography`, {
        method: 'POST',
        headers: {
          'Authorization': `Bearer ${token}`,
          'Content-Type': 'application/json',
        },
      });

      if (!response.ok) {
        throw new Error(`Seeding failed: ${response.statusText}`);
      }

      const data = await response.json();
      setResult(data.data);
    } catch (err: any) {
      setError(err.message || 'Failed to seed geography data');
    } finally {
      setIsSeedingAll(false);
    }
  };

  const seedRegions = async () => {
    setIsSeedingRegions(true);
    setError(null);

    try {
      const token = localStorage.getItem('admin_token');
      const response = await fetch(`${API_BASE_URL}/Api/V1/Admin/Seeding/Regions`, {
        method: 'POST',
        headers: {
          'Authorization': `Bearer ${token}`,
          'Content-Type': 'application/json',
        },
      });

      if (!response.ok) {
        throw new Error(`Region seeding failed: ${response.statusText}`);
      }

      const data = await response.json();
      alert(`Regions seeded: ${data.data.added} added, ${data.data.skipped} skipped, ${data.data.failed} failed`);
    } catch (err: any) {
      setError(err.message || 'Failed to seed regions');
    } finally {
      setIsSeedingRegions(false);
    }
  };

  const seedStates = async () => {
    setIsSeedingStates(true);
    setError(null);

    try {
      const token = localStorage.getItem('admin_token');
      const response = await fetch(`${API_BASE_URL}/Api/V1/Admin/Seeding/States`, {
        method: 'POST',
        headers: {
          'Authorization': `Bearer ${token}`,
          'Content-Type': 'application/json',
        },
      });

      if (!response.ok) {
        throw new Error(`State seeding failed: ${response.statusText}`);
      }

      const data = await response.json();
      alert(`States seeded: ${data.data.added} added, ${data.data.skipped} skipped, ${data.data.failed} failed`);
    } catch (err: any) {
      setError(err.message || 'Failed to seed states');
    } finally {
      setIsSeedingStates(false);
    }
  };

  const seedCities = async () => {
    setIsSeedingCities(true);
    setError(null);

    try {
      const token = localStorage.getItem('admin_token');
      const response = await fetch(`${API_BASE_URL}/Api/V1/Admin/Seeding/Cities`, {
        method: 'POST',
        headers: {
          'Authorization': `Bearer ${token}`,
          'Content-Type': 'application/json',
        },
      });

      if (!response.ok) {
        throw new Error(`City seeding failed: ${response.statusText}`);
      }

      const data = await response.json();
      alert(`Cities seeded: ${data.data.added} added, ${data.data.skipped} skipped, ${data.data.failed} failed`);
    } catch (err: any) {
      setError(err.message || 'Failed to seed cities');
    } finally {
      setIsSeedingCities(false);
    }
  };

  return (
    <div>
      {/* Header */}
      <div className="mb-8">
        <h1 className="text-3xl font-bold text-gray-900">استيراد بيانات الجغرافيا</h1>
        <p className="text-gray-600 mt-2">استيراد بيانات الجغرافيا الرسمية للسودان من Google API</p>
      </div>

      {/* Warning Notice */}
      <div className="mb-6 admin-card bg-blue-50 border-blue-200">
        <div className="flex items-start gap-3">
          <AlertCircle className="text-blue-600 flex-shrink-0 mt-0.5" size={20} />
          <div>
            <h3 className="font-semibold text-blue-900 mb-1">معلومات هامة</h3>
            <ul className="text-sm text-blue-800 space-y-1">
              <li>• Requires Google API key configured in backend (appsettings.json)</li>
              <li>• Google Geocoding API will be called for each location</li>
              <li>• Seeding is safe - existing data will be skipped (no duplicates)</li>
              <li>• SuperAdmin role required to perform seeding operations</li>
            </ul>
          </div>
        </div>
      </div>

      {error && (
        <div className="mb-6 admin-card bg-red-50 border-red-200">
          <div className="flex items-start gap-3">
            <AlertCircle className="text-red-600 flex-shrink-0 mt-0.5" size={20} />
            <div>
              <h3 className="font-semibold text-red-900 mb-1">خطأ</h3>
              <p className="text-sm text-red-800">{error}</p>
            </div>
          </div>
        </div>
      )}

      {/* Seeding Actions */}
      <div className="grid grid-cols-1 md:grid-cols-2 gap-6 mb-8">
        {/* Seed All */}
        <div className="admin-card">
          <div className="flex items-start gap-4">
            <div className="p-3 bg-admin-primary-100 rounded-lg">
              <Globe className="text-admin-primary-600" size={24} />
            </div>
            <div className="flex-1">
              <h3 className="text-lg font-semibold text-gray-900 mb-2">استيراد كل بيانات الجغرافيا</h3>
              <p className="text-sm text-gray-600 mb-4">
                Import all geographic data (regions, states, and cities) from Google API in correct hierarchical order.
              </p>
              <button
                onClick={seedAll}
                disabled={isSeedingAll}
                className="admin-button flex items-center gap-2 w-full justify-center"
              >
                <Download size={20} />
                {isSeedingAll ? 'Seeding All...' : 'Seed All Geography'}
              </button>
            </div>
          </div>
        </div>

        {/* Individual Seeding Options */}
        <div className="admin-card">
          <div className="flex items-start gap-4">
            <div className="p-3 bg-purple-100 rounded-lg">
              <MapPin className="text-purple-600" size={24} />
            </div>
            <div className="flex-1">
              <h3 className="text-lg font-semibold text-gray-900 mb-2">استيراد فردي</h3>
              <p className="text-sm text-gray-600 mb-4">
                Seed specific entity types independently (useful for testing or partial updates).
              </p>
              <div className="space-y-2">
                <button
                  onClick={seedRegions}
                  disabled={isSeedingRegions}
                  className="w-full px-4 py-2 border border-gray-300 text-gray-700 rounded-lg hover:bg-gray-50 transition-colors disabled:opacity-50"
                >
                  {isSeedingRegions ? 'Seeding...' : 'Seed Regions Only'}
                </button>
                <button
                  onClick={seedStates}
                  disabled={isSeedingStates}
                  className="w-full px-4 py-2 border border-gray-300 text-gray-700 rounded-lg hover:bg-gray-50 transition-colors disabled:opacity-50"
                >
                  {isSeedingStates ? 'Seeding...' : 'Seed States Only'}
                </button>
                <button
                  onClick={seedCities}
                  disabled={isSeedingCities}
                  className="w-full px-4 py-2 border border-gray-300 text-gray-700 rounded-lg hover:bg-gray-50 transition-colors disabled:opacity-50"
                >
                  {isSeedingCities ? 'Seeding...' : 'Seed Cities Only'}
                </button>
              </div>
            </div>
          </div>
        </div>
      </div>

      {/* Results */}
      {result && (
        <div className="admin-card">
          <div className="flex items-start gap-3 mb-6">
            <CheckCircle className="text-green-600 flex-shrink-0 mt-0.5" size={24} />
            <div>
              <h2 className="text-xl font-bold text-gray-900 mb-1">اكتمل الاستيراد</h2>
              <p className="text-gray-600">تم استيراد بيانات الجغرافيا من Google API</p>
            </div>
          </div>

          {/* Summary Cards */}
          <div className="grid grid-cols-1 md:grid-cols-3 gap-4 mb-6">
            <div className="p-4 bg-green-50 border border-green-200 rounded-lg">
              <div className="text-2xl font-bold text-green-900">{result.totalAdded}</div>
              <div className="text-sm text-green-700">إجمالي المضاف</div>
            </div>
            <div className="p-4 bg-yellow-50 border border-yellow-200 rounded-lg">
              <div className="text-2xl font-bold text-yellow-900">{result.totalSkipped}</div>
              <div className="text-sm text-yellow-700">إجمالي المتخطّى</div>
            </div>
            <div className="p-4 bg-red-50 border border-red-200 rounded-lg">
              <div className="text-2xl font-bold text-red-900">{result.totalFailed}</div>
              <div className="text-sm text-red-700">إجمالي الفشل</div>
            </div>
          </div>

          {/* Detailed Results */}
          <div className="space-y-4">
            {/* Regions */}
            <div className="border border-gray-200 rounded-lg p-4">
              <h3 className="font-semibold text-gray-900 mb-2">المناطق</h3>
              <div className="grid grid-cols-3 gap-4 text-sm">
                <div>
                  <span className="text-gray-600">Added:</span>{' '}
                  <span className="font-medium text-green-700">{result.regions.added}</span>
                </div>
                <div>
                  <span className="text-gray-600">Skipped:</span>{' '}
                  <span className="font-medium text-yellow-700">{result.regions.skipped}</span>
                </div>
                <div>
                  <span className="text-gray-600">Failed:</span>{' '}
                  <span className="font-medium text-red-700">{result.regions.failed}</span>
                </div>
              </div>
              {result.regions.errors.length > 0 && (
                <div className="mt-2 text-sm text-red-600">
                  <div className="font-medium">Errors:</div>
                  <ul className="list-disc list-inside">
                    {result.regions.errors.map((err, idx) => (
                      <li key={idx}>{err}</li>
                    ))}
                  </ul>
                </div>
              )}
            </div>

            {/* States */}
            <div className="border border-gray-200 rounded-lg p-4">
              <h3 className="font-semibold text-gray-900 mb-2">الولايات</h3>
              <div className="grid grid-cols-3 gap-4 text-sm">
                <div>
                  <span className="text-gray-600">Added:</span>{' '}
                  <span className="font-medium text-green-700">{result.states.added}</span>
                </div>
                <div>
                  <span className="text-gray-600">Skipped:</span>{' '}
                  <span className="font-medium text-yellow-700">{result.states.skipped}</span>
                </div>
                <div>
                  <span className="text-gray-600">Failed:</span>{' '}
                  <span className="font-medium text-red-700">{result.states.failed}</span>
                </div>
              </div>
              {result.states.errors.length > 0 && (
                <div className="mt-2 text-sm text-red-600">
                  <div className="font-medium">Errors:</div>
                  <ul className="list-disc list-inside">
                    {result.states.errors.map((err, idx) => (
                      <li key={idx}>{err}</li>
                    ))}
                  </ul>
                </div>
              )}
              {result.states.warnings.length > 0 && (
                <div className="mt-2 text-sm text-yellow-600">
                  <div className="font-medium">Warnings:</div>
                  <ul className="list-disc list-inside">
                    {result.states.warnings.map((warn, idx) => (
                      <li key={idx}>{warn}</li>
                    ))}
                  </ul>
                </div>
              )}
            </div>

            {/* Cities */}
            <div className="border border-gray-200 rounded-lg p-4">
              <h3 className="font-semibold text-gray-900 mb-2">المدن</h3>
              <div className="grid grid-cols-3 gap-4 text-sm">
                <div>
                  <span className="text-gray-600">Added:</span>{' '}
                  <span className="font-medium text-green-700">{result.cities.added}</span>
                </div>
                <div>
                  <span className="text-gray-600">Skipped:</span>{' '}
                  <span className="font-medium text-yellow-700">{result.cities.skipped}</span>
                </div>
                <div>
                  <span className="text-gray-600">Failed:</span>{' '}
                  <span className="font-medium text-red-700">{result.cities.failed}</span>
                </div>
              </div>
              {result.cities.errors.length > 0 && (
                <div className="mt-2 text-sm text-red-600">
                  <div className="font-medium">Errors:</div>
                  <ul className="list-disc list-inside">
                    {result.cities.errors.map((err, idx) => (
                      <li key={idx}>{err}</li>
                    ))}
                  </ul>
                </div>
              )}
              {result.cities.warnings.length > 0 && (
                <div className="mt-2 text-sm text-yellow-600">
                  <div className="font-medium">Warnings:</div>
                  <ul className="list-disc list-inside">
                    {result.cities.warnings.map((warn, idx) => (
                      <li key={idx}>{warn}</li>
                    ))}
                  </ul>
                </div>
              )}
            </div>
          </div>
        </div>
      )}

      {/* Configuration Instructions */}
      <div className="mt-8 admin-card bg-gray-50">
        <div className="flex items-start gap-3">
          <Database className="text-gray-600 flex-shrink-0 mt-0.5" size={20} />
          <div>
            <h3 className="font-semibold text-gray-900 mb-2">إعداد Google API</h3>
            <div className="text-sm text-gray-700 space-y-2">
              <p>To enable Google API seeding, configure your backend:</p>
              <pre className="bg-white p-3 rounded border border-gray-300 overflow-x-auto">
                {`{
  "Google": {
    "ApiKey": "YOUR_GOOGLE_API_KEY",
    "EnableSeeding": true,
    "DefaultCountry": "Sudan",
    "RateLimitPerMinute": 50
  }
}`}
              </pre>
              <p className="mt-2">
                <strong>Note:</strong> Get your API key from{' '}
                <a
                  href="https://console.cloud.google.com/apis/credentials"
                  target="_blank"
                  rel="noopener noreferrer"
                  className="text-admin-primary-600 hover:underline"
                >
                  Google Cloud Console
                </a>
              </p>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
};

export default SeedingPage;
