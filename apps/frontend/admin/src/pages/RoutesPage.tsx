import { useState, useEffect } from 'react';
import { Search, Plus, Edit, Eye, Trash2 } from 'lucide-react';
import { Route } from '../types/infrastructure';
import { routesApi, stationsApi } from '../services/api';
import { Station } from '../types/geography';
import StatusBadge from '../components/common/StatusBadge';
import Pagination from '../components/common/Pagination';
import FilterDropdown from '../components/common/FilterDropdown';
import RouteModal from '../components/routes/RouteModal';
import RouteDetailModal from '../components/routes/RouteDetailModal';
import { showSuccess, showError, showConfirm, extractErrorMessage } from '../utils/alerts';

const RoutesPage = () => {
  const [routes, setRoutes] = useState<Route[]>([]);
  const [stations, setStations] = useState<Station[]>([]);
  const [isLoading, setIsLoading] = useState(false);

  // Filters
  const [searchTerm, setSearchTerm] = useState('');
  const [originFilter, setOriginFilter] = useState<number>(0);
  const [destinationFilter, setDestinationFilter] = useState<number>(0);
  const [statusFilter, setStatusFilter] = useState<string>('all');

  // Pagination
  const [pageNumber, setPageNumber] = useState(1);
  const [pageSize, setPageSize] = useState(20);
  const [totalRoutes, setTotalRoutes] = useState(0);

  // Modals
  const [isRouteModalOpen, setIsRouteModalOpen] = useState(false);
  const [selectedRoute, setSelectedRoute] = useState<Route | null>(null);
  const [isDetailModalOpen, setIsDetailModalOpen] = useState(false);
  const [detailRouteId, setDetailRouteId] = useState<number | null>(null);

  // Load stations for filters
  useEffect(() => {
    loadStations();
  }, []);

  // Load routes when filters change
  useEffect(() => {
    loadRoutes();
  }, [originFilter, destinationFilter, statusFilter, pageNumber, pageSize]);

  const loadStations = async () => {
    try {
      // Load all active stations for filter dropdown
      const data = await stationsApi.getAll({ isActive: true, pageSize: 10000 });
      setStations(data);
    } catch (error: any) {
      console.error('Failed to load stations:', error);
      showError('Loading Error', extractErrorMessage(error));
    }
  };

  const loadRoutes = async () => {
    setIsLoading(true);
    try {
      const params: any = {
        pageNumber,
        pageSize,
      };

      if (originFilter) params.originStationId = originFilter;
      if (destinationFilter) params.destinationStationId = destinationFilter;
      if (statusFilter !== 'all') params.isActive = statusFilter === 'active';

      const data = await routesApi.getAll(params);
      setRoutes(data);
      setTotalRoutes(data.length); // Backend should return total count
    } catch (error: any) {
      console.error('Failed to load routes:', error);
      showError('Loading Error', extractErrorMessage(error));
    } finally {
      setIsLoading(false);
    }
  };

  const handleDelete = async (id: number) => {
    const confirmed = await showConfirm(
      'Delete Route?',
      'This action cannot be undone. All intermediate stations will be removed.',
      'Yes, delete it'
    );
    
    if (!confirmed) return;

    try {
      await routesApi.delete(id);
      await showSuccess('Deleted!', 'Route has been deleted successfully');
      loadRoutes();
    } catch (error: any) {
      console.error('Failed to delete route:', error);
      showError('Delete Failed', extractErrorMessage(error));
    }
  };

  const handleAddRoute = () => {
    setSelectedRoute(null);
    setIsRouteModalOpen(true);
  };

  const handleEditRoute = (route: Route) => {
    setSelectedRoute(route);
    setIsRouteModalOpen(true);
  };

  const handleViewDetails = (routeId: number) => {
    setDetailRouteId(routeId);
    setIsDetailModalOpen(true);
  };

  // Filter routes by search term (client-side)
  const filteredRoutes = routes.filter((route) => {
    if (!searchTerm.trim()) return true;
    const search = searchTerm.toLowerCase();
    return (
      route.nameEn.toLowerCase().includes(search) ||
      route.nameAr.includes(searchTerm) ||
      route.origin.nameEn.toLowerCase().includes(search) ||
      route.destination.nameEn.toLowerCase().includes(search)
    );
  });

  return (
    <div className="p-6">
      <div className="mb-6">
        <h1 className="text-3xl font-bold text-gray-900">إدارة المسارات</h1>
        <p className="text-gray-600 mt-2">إدارة مسارات القطارات ومحطاتها الوسيطة</p>
      </div>

      {/* Filters */}
      <div className="space-y-4 mb-6">
        <div className="flex flex-wrap gap-4 items-end">
          <div className="relative flex-1 min-w-[250px]">
            <Search className="absolute left-3 top-1/2 transform -translate-y-1/2 text-gray-400" size={20} />
            <input
              type="text"
              placeholder="ابحث عن مسار…"
              value={searchTerm}
              onChange={(e) => setSearchTerm(e.target.value)}
              className="w-full pl-10 pr-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-admin-primary-500"
            />
          </div>

          <FilterDropdown
            label="Origin Station"
            value={originFilter}
            onChange={(val) => {
              setOriginFilter(Number(val));
              setPageNumber(1);
            }}
            options={[
              { value: 0, label: 'All Origins' },
              ...stations.map((s) => ({ value: s.id, label: s.nameEn })),
            ]}
            className="min-w-[180px]"
          />

          <FilterDropdown
            label="Destination Station"
            value={destinationFilter}
            onChange={(val) => {
              setDestinationFilter(Number(val));
              setPageNumber(1);
            }}
            options={[
              { value: 0, label: 'All Destinations' },
              ...stations.map((s) => ({ value: s.id, label: s.nameEn })),
            ]}
            className="min-w-[180px]"
          />

          <FilterDropdown
            label="الحالة"
            value={statusFilter}
            onChange={(val) => {
              setStatusFilter(String(val));
              setPageNumber(1);
            }}
            options={[
              { value: 'all', label: 'All Status' },
              { value: 'active', label: 'نشط' },
              { value: 'inactive', label: 'غير نشط' },
            ]}
            className="min-w-[130px]"
          />

          <button
            onClick={handleAddRoute}
            className="admin-button flex items-center gap-2 ml-auto"
          >
            <Plus size={20} />إضافة مسار</button>
        </div>
      </div>

      {/* Routes Table */}
      <div className="admin-card">
        <div className="overflow-x-auto">
          <table className="w-full">
            <thead className="bg-gray-50 border-b border-gray-200">
              <tr>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                  Route Name
                </th>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                  Origin
                </th>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                  Destination
                </th>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                  Distance (km)
                </th>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                  Stops
                </th>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">الحالة</th>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">إجراءات</th>
              </tr>
            </thead>
            <tbody className="bg-white divide-y divide-gray-200">
              {isLoading ? (
                <tr>
                  <td colSpan={7} className="px-6 py-8 text-center text-gray-500">
                    Loading routes...
                  </td>
                </tr>
              ) : filteredRoutes.length === 0 ? (
                <tr>
                  <td colSpan={7} className="px-6 py-8 text-center text-gray-500">
                    No routes found
                  </td>
                </tr>
              ) : (
                filteredRoutes.map((route) => (
                  <tr key={route.id} className="hover:bg-gray-50">
                    <td className="px-6 py-4">
                      <div className="font-medium text-gray-900">{route.nameEn}</div>
                      <div className="text-sm text-gray-500" dir="rtl">{route.nameAr}</div>
                    </td>
                    <td className="px-6 py-4 whitespace-nowrap text-gray-900">
                      {route.origin.nameEn}
                    </td>
                    <td className="px-6 py-4 whitespace-nowrap text-gray-900">
                      {route.destination.nameEn}
                    </td>
                    <td className="px-6 py-4 whitespace-nowrap text-gray-600">
                      {route.distanceKm ? route.distanceKm.toFixed(2) : 'N/A'}
                    </td>
                    <td className="px-6 py-4 whitespace-nowrap text-gray-600">
                      {route.intermediateStops.length} stops
                    </td>
                    <td className="px-6 py-4 whitespace-nowrap">
                      <StatusBadge
                        isActive={route.isActive}
                        maintenanceNote={route.maintenanceNote}
                      />
                    </td>
                    <td className="px-6 py-4 whitespace-nowrap text-sm">
                      <div className="flex items-center gap-2">
                        <button
                          onClick={() => handleViewDetails(route.id)}
                          className="text-blue-600 hover:text-blue-800"
                          title="View details"
                        >
                          <Eye size={18} />
                        </button>
                        <button
                          onClick={() => handleEditRoute(route)}
                          className="text-admin-primary-600 hover:text-admin-primary-800"
                          title="Edit route"
                        >
                          <Edit size={18} />
                        </button>
                        <button
                          onClick={() => handleDelete(route.id)}
                          className="text-red-600 hover:text-red-800"
                          title="Delete route"
                        >
                          <Trash2 size={18} />
                        </button>
                      </div>
                    </td>
                  </tr>
                ))
              )}
            </tbody>
          </table>
        </div>
        <Pagination
          currentPage={pageNumber}
          pageSize={pageSize}
          totalItems={totalRoutes}
          onPageChange={setPageNumber}
          onPageSizeChange={(size) => {
            setPageSize(size);
            setPageNumber(1);
          }}
        />
      </div>

      {/* Modals */}
      <RouteModal
        isOpen={isRouteModalOpen}
        onClose={() => setIsRouteModalOpen(false)}
        onSuccess={() => {
          setIsRouteModalOpen(false);
          loadRoutes();
        }}
        route={selectedRoute}
      />

      {detailRouteId && (
        <RouteDetailModal
          isOpen={isDetailModalOpen}
          onClose={() => {
            setIsDetailModalOpen(false);
            setDetailRouteId(null);
          }}
          onRefresh={loadRoutes}
          routeId={detailRouteId}
        />
      )}
    </div>
  );
};

export default RoutesPage;
