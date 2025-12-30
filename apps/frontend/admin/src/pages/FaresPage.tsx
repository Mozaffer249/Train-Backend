import { useState, useEffect } from 'react';
import { Plus, DollarSign } from 'lucide-react';
import { Fare } from '../types/infrastructure';
import { faresApi, routesApi } from '../services/api';
import { Route } from '../types/infrastructure';
import FilterDropdown from '../components/common/FilterDropdown';
import FareModal from '../components/fares/FareModal';
import { CoachClassLabels } from '../types/infrastructure';

const FaresPage = () => {
  const [fares, setFares] = useState<Fare[]>([]);
  const [routes, setRoutes] = useState<Route[]>([]);
  const [isLoading, setIsLoading] = useState(false);
  const [isFareModalOpen, setIsFareModalOpen] = useState(false);

  // Filters
  const [routeFilter, setRouteFilter] = useState<number>(0);
  const [coachClassFilter, setCoachClassFilter] = useState<number>(0);

  useEffect(() => {
    loadRoutes();
  }, []);

  useEffect(() => {
    loadFares();
  }, [routeFilter, coachClassFilter]);

  const loadRoutes = async () => {
    try {
      const data = await routesApi.getAll({ isActive: true, pageSize: 1000 });
      setRoutes(data);
    } catch (error) {
      console.error('Failed to load routes:', error);
    }
  };

  const loadFares = async () => {
    setIsLoading(true);
    try {
      const params: any = {};
      if (routeFilter) params.routeId = routeFilter;
      if (coachClassFilter) params.coachClass = coachClassFilter;

      const data = await faresApi.getAll(params);
      setFares(data);
    } catch (error) {
      console.error('Failed to load fares:', error);
    } finally {
      setIsLoading(false);
    }
  };

  const formatDate = (dateString: string) => {
    return new Date(dateString).toLocaleDateString('en-US', {
      year: 'numeric',
      month: 'short',
      day: 'numeric',
    });
  };

  const getFareScope = (fare: Fare) => {
    if (fare.tripId) return 'Trip-specific';
    if (fare.originStationId && fare.destinationStationId) return 'Segment';
    if (fare.routeId) return 'Route-level';
    return 'Unknown';
  };

  return (
    <div className="p-6">
      <div className="mb-6">
        <h1 className="text-3xl font-bold text-gray-900 flex items-center gap-2">
          <DollarSign size={32} />
          Fares Management
        </h1>
        <p className="text-gray-600 mt-2">Configure pricing for routes, segments, and trips</p>
      </div>

      {/* Filters */}
      <div className="space-y-4 mb-6">
        <div className="flex flex-wrap gap-4 items-end">
          <FilterDropdown
            label="Route"
            value={routeFilter}
            onChange={(val) => setRouteFilter(Number(val))}
            options={[
              { value: 0, label: 'All Routes' },
              ...routes.map((r) => ({
                value: r.id,
                label: `${r.nameEn} (${r.origin.nameEn} → ${r.destination.nameEn})`,
              })),
            ]}
            className="min-w-[250px]"
          />

          <FilterDropdown
            label="Coach Class"
            value={coachClassFilter}
            onChange={(val) => setCoachClassFilter(Number(val))}
            options={[
              { value: 0, label: 'All Classes' },
              { value: 1, label: 'First Class' },
              { value: 2, label: 'Second Class' },
              { value: 3, label: 'Third Class' },
            ]}
            className="min-w-[150px]"
          />

          <button
            onClick={() => setIsFareModalOpen(true)}
            className="admin-button flex items-center gap-2 ml-auto"
          >
            <Plus size={20} />
            Create Fare
          </button>
        </div>
      </div>

      {/* Fares Table */}
      <div className="admin-card">
        <div className="overflow-x-auto">
          <table className="w-full">
            <thead className="bg-gray-50 border-b border-gray-200">
              <tr>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                  Scope
                </th>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                  Class
                </th>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                  Base Price
                </th>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                  Price/km
                </th>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                  VAT
                </th>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                  Discount
                </th>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                  Final Price
                </th>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                  Effective Period
                </th>
              </tr>
            </thead>
            <tbody className="bg-white divide-y divide-gray-200">
              {isLoading ? (
                <tr>
                  <td colSpan={8} className="px-6 py-8 text-center text-gray-500">
                    Loading fares...
                  </td>
                </tr>
              ) : fares.length === 0 ? (
                <tr>
                  <td colSpan={8} className="px-6 py-8 text-center text-gray-500">
                    No fares found
                  </td>
                </tr>
              ) : (
                fares.map((fare) => (
                  <tr key={fare.id} className="hover:bg-gray-50">
                    <td className="px-6 py-4 whitespace-nowrap">
                      <span className="text-sm font-medium text-gray-900">{getFareScope(fare)}</span>
                    </td>
                    <td className="px-6 py-4 whitespace-nowrap">
                      <span className="px-2 py-1 text-xs font-medium rounded bg-blue-100 text-blue-800">
                        {fare.coachClass}
                      </span>
                    </td>
                    <td className="px-6 py-4 whitespace-nowrap text-gray-900">
                      {fare.basePrice.toFixed(2)} {fare.currency}
                    </td>
                    <td className="px-6 py-4 whitespace-nowrap text-gray-600">
                      {fare.pricePerKm ? `${fare.pricePerKm.toFixed(2)} ${fare.currency}` : 'N/A'}
                    </td>
                    <td className="px-6 py-4 whitespace-nowrap text-gray-600">
                      {(fare.vatRate * 100).toFixed(0)}%
                    </td>
                    <td className="px-6 py-4 whitespace-nowrap text-gray-600">
                      {fare.discountPercent ? `${fare.discountPercent.toFixed(0)}%` : '-'}
                    </td>
                    <td className="px-6 py-4 whitespace-nowrap">
                      <div>
                        <div className="font-medium text-gray-900">
                          {fare.finalPrice.toFixed(2)} {fare.currency}
                        </div>
                        <div className="text-xs text-gray-500">
                          +VAT: {fare.totalWithVat.toFixed(2)} {fare.currency}
                        </div>
                      </div>
                    </td>
                    <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-600">
                      <div>
                        <div>From: {formatDate(fare.effectiveFrom)}</div>
                        {fare.effectiveTo && <div>To: {formatDate(fare.effectiveTo)}</div>}
                      </div>
                    </td>
                  </tr>
                ))
              )}
            </tbody>
          </table>
        </div>
      </div>

      <FareModal
        isOpen={isFareModalOpen}
        onClose={() => setIsFareModalOpen(false)}
        onSuccess={() => {
          setIsFareModalOpen(false);
          loadFares();
        }}
      />
    </div>
  );
};

export default FaresPage;
