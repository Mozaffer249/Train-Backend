import { useState, useEffect } from 'react';
import { Search, Plus, Edit, MapIcon } from 'lucide-react';
import { GeographyTab, City, Station } from '../types/geography';
import { citiesApi, stationsApi } from '../services/api';
import CityModal from '../components/geography/CityModal';
import StationModal from '../components/geography/StationModal';
import GeographyMap from '../components/map/GeographyMap';

const GeographyPage = () => {
  const [activeTab, setActiveTab] = useState<GeographyTab>('cities');
  
  // Cities state
  const [cities, setCities] = useState<City[]>([]);
  const [filteredCities, setFilteredCities] = useState<City[]>([]);
  const [citySearch, setCitySearch] = useState('');
  const [isLoadingCities, setIsLoadingCities] = useState(false);
  
  // Stations state
  const [stations, setStations] = useState<Station[]>([]);
  const [filteredStations, setFilteredStations] = useState<Station[]>([]);
  const [stationSearch, setStationSearch] = useState('');
  const [stationCityFilter, setStationCityFilter] = useState<number>(0);
  const [isLoadingStations, setIsLoadingStations] = useState(false);
  
  // Modal states
  const [isCityModalOpen, setIsCityModalOpen] = useState(false);
  const [selectedCity, setSelectedCity] = useState<City | null>(null);
  
  const [isStationModalOpen, setIsStationModalOpen] = useState(false);
  const [selectedStation, setSelectedStation] = useState<Station | null>(null);

  // Load data on mount and tab change
  useEffect(() => {
    if (activeTab === 'cities') {
      loadCities();
    } else if (activeTab === 'stations') {
      loadStations();
      if (cities.length === 0) loadCities();
    } else if (activeTab === 'map') {
      loadCities();
      loadStations();
    }
  }, [activeTab]);

  // Filter cities by search
  useEffect(() => {
    if (citySearch.trim()) {
      setFilteredCities(
        cities.filter(
          (c) =>
            c.nameAr.includes(citySearch) ||
            c.nameEn.toLowerCase().includes(citySearch.toLowerCase())
        )
      );
    } else {
      setFilteredCities(cities);
    }
  }, [citySearch, cities]);

  // Filter stations by search and city
  useEffect(() => {
    let filtered = stations;
    
    if (stationCityFilter) {
      filtered = filtered.filter((s) => s.cityId === stationCityFilter);
    }
    
    if (stationSearch.trim()) {
      filtered = filtered.filter(
        (s) =>
          s.nameAr.includes(stationSearch) ||
          s.nameEn.toLowerCase().includes(stationSearch.toLowerCase()) ||
          s.code.toLowerCase().includes(stationSearch.toLowerCase())
      );
    }
    
    setFilteredStations(filtered);
  }, [stationSearch, stationCityFilter, stations]);

  // Data loading functions
  const loadCities = async () => {
    setIsLoadingCities(true);
    try {
      const data = await citiesApi.getAll();
      setCities(data);
      setFilteredCities(data);
    } catch (error) {
      console.error('Failed to load cities:', error);
    } finally {
      setIsLoadingCities(false);
    }
  };

  const loadStations = async () => {
    setIsLoadingStations(true);
    try {
      const data = await stationsApi.getAll();
      setStations(data);
      setFilteredStations(data);
    } catch (error) {
      console.error('Failed to load stations:', error);
    } finally {
      setIsLoadingStations(false);
    }
  };

  // Modal handlers
  const handleAddCity = () => {
    setSelectedCity(null);
    setIsCityModalOpen(true);
  };

  const handleEditCity = (city: City) => {
    setSelectedCity(city);
    setIsCityModalOpen(true);
  };

  const handleCitySuccess = () => {
    loadCities();
  };

  const handleAddStation = () => {
    setSelectedStation(null);
    setIsStationModalOpen(true);
  };

  const handleEditStation = (station: Station) => {
    setSelectedStation(station);
    setIsStationModalOpen(true);
  };

  const handleStationSuccess = () => {
    loadStations();
  };

  return (
    <div className="p-6">
      <div className="mb-6">
        <h1 className="text-3xl font-bold text-gray-900">Geography Management</h1>
        <p className="text-gray-600 mt-2">Manage cities and stations for the train network</p>
      </div>

      {/* Tabs */}
      <div className="border-b border-gray-200 mb-6">
        <nav className="-mb-px flex space-x-8">
          <button
            onClick={() => setActiveTab('cities')}
            className={`py-4 px-1 border-b-2 font-medium text-sm transition-colors ${
              activeTab === 'cities'
                ? 'border-admin-primary-600 text-admin-primary-600'
                : 'border-transparent text-gray-500 hover:text-gray-700 hover:border-gray-300'
            }`}
          >
            Cities ({cities.length})
          </button>
          <button
            onClick={() => setActiveTab('stations')}
            className={`py-4 px-1 border-b-2 font-medium text-sm transition-colors ${
              activeTab === 'stations'
                ? 'border-admin-primary-600 text-admin-primary-600'
                : 'border-transparent text-gray-500 hover:text-gray-700 hover:border-gray-300'
            }`}
          >
            Stations ({stations.length})
          </button>
          <button
            onClick={() => setActiveTab('map')}
            className={`py-4 px-1 border-b-2 font-medium text-sm transition-colors ${
              activeTab === 'map'
                ? 'border-admin-primary-600 text-admin-primary-600'
                : 'border-transparent text-gray-500 hover:text-gray-700 hover:border-gray-300'
            }`}
          >
            <MapIcon className="inline mr-2" size={16} />
            Map View
          </button>
        </nav>
      </div>

      {/* Cities Tab */}
      {activeTab === 'cities' && (
        <div className="space-y-4">
          <div className="flex flex-col sm:flex-row gap-4 items-start sm:items-center justify-between">
            <div className="relative flex-1 max-w-md">
              <Search className="absolute left-3 top-1/2 transform -translate-y-1/2 text-gray-400" size={20} />
              <input
                type="text"
                placeholder="Search cities..."
                value={citySearch}
                onChange={(e) => setCitySearch(e.target.value)}
                className="w-full pl-10 pr-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-admin-primary-500"
              />
            </div>
            <button onClick={handleAddCity} className="admin-button flex items-center gap-2">
              <Plus size={20} />
              Add City
            </button>
          </div>

          <div className="admin-card">
            <div className="overflow-x-auto">
              <table className="w-full">
                <thead className="bg-gray-50 border-b border-gray-200">
                  <tr>
                    <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                      ID
                    </th>
                    <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                      Name (Arabic)
                    </th>
                    <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                      Name (English)
                    </th>
                    <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                      Coordinates
                    </th>
                    <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                      Actions
                    </th>
                  </tr>
                </thead>
                <tbody className="bg-white divide-y divide-gray-200">
                  {isLoadingCities ? (
                    <tr>
                      <td colSpan={5} className="px-6 py-8 text-center text-gray-500">
                        Loading cities...
                      </td>
                    </tr>
                  ) : filteredCities.length === 0 ? (
                    <tr>
                      <td colSpan={5} className="px-6 py-8 text-center text-gray-500">
                        No cities found
                      </td>
                    </tr>
                  ) : (
                    filteredCities.map((city) => (
                      <tr key={city.id} className="hover:bg-gray-50">
                        <td className="px-6 py-4 whitespace-nowrap">
                          <div className="font-medium text-gray-900">#{city.id}</div>
                        </td>
                        <td className="px-6 py-4 whitespace-nowrap text-gray-900" dir="rtl">
                          {city.nameAr}
                        </td>
                        <td className="px-6 py-4 whitespace-nowrap text-gray-900">
                          {city.nameEn}
                        </td>
                        <td className="px-6 py-4 whitespace-nowrap text-gray-600 text-sm">
                          {city.latitude.toFixed(4)}, {city.longitude.toFixed(4)}
                        </td>
                        <td className="px-6 py-4 whitespace-nowrap text-sm">
                          <button
                            onClick={() => handleEditCity(city)}
                            className="text-admin-primary-600 hover:text-admin-primary-800"
                            title="Edit city"
                          >
                            <Edit size={18} />
                          </button>
                        </td>
                      </tr>
                    ))
                  )}
                </tbody>
              </table>
            </div>
          </div>
        </div>
      )}

      {/* Stations Tab */}
      {activeTab === 'stations' && (
        <div className="space-y-4">
          <div className="flex flex-col sm:flex-row gap-4 items-start sm:items-center justify-between">
            <div className="flex gap-4 flex-1">
              <div className="relative flex-1 max-w-md">
                <Search className="absolute left-3 top-1/2 transform -translate-y-1/2 text-gray-400" size={20} />
                <input
                  type="text"
                  placeholder="Search stations..."
                  value={stationSearch}
                  onChange={(e) => setStationSearch(e.target.value)}
                  className="w-full pl-10 pr-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-admin-primary-500"
                />
              </div>
              <div className="relative">
                <select
                  value={stationCityFilter}
                  onChange={(e) => setStationCityFilter(parseInt(e.target.value))}
                  className="px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-admin-primary-500"
                >
                  <option value={0}>All Cities</option>
                  {cities.map((city) => (
                    <option key={city.id} value={city.id}>
                      {city.nameEn}
                    </option>
                  ))}
                </select>
              </div>
            </div>
            <button onClick={handleAddStation} className="admin-button flex items-center gap-2">
              <Plus size={20} />
              Add Station
            </button>
          </div>

          <div className="admin-card">
            <div className="overflow-x-auto">
              <table className="w-full">
                <thead className="bg-gray-50 border-b border-gray-200">
                  <tr>
                    <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                      Code
                    </th>
                    <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                      Name (Arabic)
                    </th>
                    <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                      Name (English)
                    </th>
                    <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                      City
                    </th>
                    <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                      Coordinates
                    </th>
                    <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                      Actions
                    </th>
                  </tr>
                </thead>
                <tbody className="bg-white divide-y divide-gray-200">
                  {isLoadingStations ? (
                    <tr>
                      <td colSpan={6} className="px-6 py-8 text-center text-gray-500">
                        Loading stations...
                      </td>
                    </tr>
                  ) : filteredStations.length === 0 ? (
                    <tr>
                      <td colSpan={6} className="px-6 py-8 text-center text-gray-500">
                        No stations found
                      </td>
                    </tr>
                  ) : (
                    filteredStations.map((station) => (
                      <tr key={station.id} className="hover:bg-gray-50">
                        <td className="px-6 py-4 whitespace-nowrap">
                          <div className="font-medium text-gray-900">{station.code}</div>
                        </td>
                        <td className="px-6 py-4 whitespace-nowrap text-gray-900" dir="rtl">
                          {station.nameAr}
                        </td>
                        <td className="px-6 py-4 whitespace-nowrap text-gray-900">
                          {station.nameEn}
                        </td>
                        <td className="px-6 py-4 whitespace-nowrap text-gray-600">
                          {station.cityName || 'N/A'}
                        </td>
                        <td className="px-6 py-4 whitespace-nowrap text-gray-600 text-sm">
                          {station.latitude.toFixed(4)}, {station.longitude.toFixed(4)}
                        </td>
                        <td className="px-6 py-4 whitespace-nowrap text-sm">
                          <button
                            onClick={() => handleEditStation(station)}
                            className="text-admin-primary-600 hover:text-admin-primary-800"
                            title="Edit station"
                          >
                            <Edit size={18} />
                          </button>
                        </td>
                      </tr>
                    ))
                  )}
                </tbody>
              </table>
            </div>
          </div>
        </div>
      )}

      {/* Map Tab */}
      {activeTab === 'map' && (
        <GeographyMap
          cities={cities}
          stations={stations}
          onRefresh={() => {
            loadCities();
            loadStations();
          }}
        />
      )}

      {/* Modals */}
      <CityModal
        isOpen={isCityModalOpen}
        onClose={() => setIsCityModalOpen(false)}
        onSuccess={handleCitySuccess}
        city={selectedCity}
        existingCities={cities}
      />
      <StationModal
        isOpen={isStationModalOpen}
        onClose={() => setIsStationModalOpen(false)}
        onSuccess={handleStationSuccess}
        station={selectedStation}
      />
    </div>
  );
};

export default GeographyPage;
