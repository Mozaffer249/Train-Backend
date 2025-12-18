import React, { useState, useEffect } from 'react';
import { useLocation, useNavigate } from 'react-router-dom';
import { Clock, MapPin, Users, Filter, SortAsc, Train, Star, Wifi, Coffee, Zap, Shield } from 'lucide-react';
import { useLanguage } from '../contexts/LanguageContext';

interface Train {
  id: string;
  name: string;
  departure: string;
  arrival: string;
  duration: string;
  price: number;
  class: string;
  availability: number;
  amenities: string[];
}

const SearchResults: React.FC = () => {
  const location = useLocation();
  const navigate = useNavigate();
  const { t } = useLanguage();
  const [trains, setTrains] = useState<Train[]>([]);
  const [loading, setLoading] = useState(true);
  const [sortBy, setSortBy] = useState('time');
  const [filters, setFilters] = useState({
    class: '',
    priceRange: [0, 500],
    timeRange: 'all'
  });

  const searchParams = location.state;

  useEffect(() => {
    // Mock API call
    setTimeout(() => {
      const mockTrains: Train[] = [
        {
          id: '1',
          name: 'Nile Valley Express',
          departure: '08:00',
          arrival: '12:30',
          duration: '4h 30m',
          price: 150,
          class: 'Business',
          availability: 45,
          amenities: ['WiFi', 'Meals', 'Power Outlets']
        },
        {
          id: '2',
          name: 'Sudan Express',
          departure: '10:15',
          arrival: '14:45',
          duration: '4h 30m',
          price: 120,
          class: 'Economy',
          availability: 23,
          amenities: ['WiFi', 'Snacks']
        },
        {
          id: '3',
          name: 'Royal Sudan Train',
          departure: '14:00',
          arrival: '18:30',
          duration: '4h 30m',
          price: 250,
          class: 'VIP',
          availability: 12,
          amenities: ['WiFi', 'Meals', 'Power Outlets', 'Lounge Access']
        },
        {
          id: '4',
          name: 'Desert Express',
          departure: '18:30',
          arrival: '23:00',
          duration: '4h 30m',
          price: 135,
          class: 'Economy',
          availability: 67,
          amenities: ['WiFi', 'Snacks', 'Power Outlets']
        }
      ];
      setTrains(mockTrains);
      setLoading(false);
    }, 1000);
  }, []);

  const handleBooking = (train: Train) => {
    navigate('/book', { state: { train, searchParams } });
  };

  const sortedTrains = [...trains].sort((a, b) => {
    switch (sortBy) {
      case 'price':
        return a.price - b.price;
      case 'time':
        return a.departure.localeCompare(b.departure);
      case 'duration':
        return a.duration.localeCompare(b.duration);
      default:
        return 0;
    }
  });

  const filteredTrains = sortedTrains.filter(train => {
    if (filters.class && train.class !== filters.class) return false;
    if (train.price < filters.priceRange[0] || train.price > filters.priceRange[1]) return false;
    return true;
  });

  if (loading) {
    return (
      <div className="min-h-screen flex items-center justify-center">
        <div className="text-center">
          <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-emerald-600 mx-auto mb-4"></div>
          <p className="text-gray-600">{t('loading')}</p>
        </div>
      </div>
    );
  }

  return (
    <div className="min-h-screen bg-gray-50 py-4 sm:py-8">
      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        {/* Search Summary */}
        <div className="bg-white rounded-lg shadow-md p-4 sm:p-6 mb-4 sm:mb-8">
          <div className="flex flex-col md:flex-row md:items-center md:justify-between">
            <div className="flex flex-col sm:flex-row sm:items-center space-y-2 sm:space-y-0 sm:space-x-4 rtl:space-x-reverse mb-4 md:mb-0">
              <div className="flex items-center space-x-2 rtl:space-x-reverse text-sm sm:text-base">
                <MapPin className="h-5 w-5 text-blue-600" />
                <span className="font-medium">{searchParams?.from}</span>
                <span className="text-gray-400">→</span>
                <span className="font-medium">{searchParams?.to}</span>
              </div>
              <div className="flex items-center space-x-2 rtl:space-x-reverse text-sm sm:text-base">
                <Clock className="h-5 w-5 text-gray-400" />
                <span className="text-gray-600">{searchParams?.date}</span>
              </div>
              <div className="flex items-center space-x-2 rtl:space-x-reverse text-sm sm:text-base">
                <Users className="h-5 w-5 text-gray-400" />
                <span className="text-gray-600">{searchParams?.passengers} passengers</span>
              </div>
            </div>
            <button
              onClick={() => navigate('/')}
              className="bg-blue-600 text-white px-4 py-2 rounded-lg hover:bg-blue-700 transition-colors text-sm sm:text-base w-full sm:w-auto"
            >
              {t('modify.search')}
            </button>
          </div>
        </div>

        <div className="grid grid-cols-1 lg:grid-cols-4 gap-8">
          {/* Filters Sidebar */}
          <div className="lg:col-span-1 order-2 lg:order-1">
            <div className="bg-white rounded-lg shadow-md p-4 sm:p-6 lg:sticky lg:top-24">
              <h3 className="text-base sm:text-lg font-semibold mb-4 flex items-center">
                <Filter className="h-5 w-5 mr-2 rtl:mr-0 rtl:ml-2" />
                {t('filters')}
              </h3>
              
              {/* Class Filter */}
              <div className="mb-4 sm:mb-6">
                <label className="block text-sm font-medium text-gray-700 mb-2">
                  {t('class')}
                </label>
                <select
                  value={filters.class}
                  onChange={(e) => setFilters({...filters, class: e.target.value})}
                  className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500 text-sm sm:text-base"
                >
                  <option value="">{t('all.classes')}</option>
                  <option value="Economy">{t('economy')}</option>
                  <option value="Business">{t('business')}</option>
                  <option value="VIP">{t('vip')}</option>
                </select>
              </div>

              {/* Price Range */}
              <div className="mb-4 sm:mb-6">
                <label className="block text-sm font-medium text-gray-700 mb-2">
                  {t('price.range.sdg')}
                </label>
                <div className="grid grid-cols-2 gap-2">
                  <input
                    type="number"
                    placeholder={t('min')}
                    value={filters.priceRange[0]}
                    onChange={(e) => setFilters({...filters, priceRange: [parseInt(e.target.value) || 0, filters.priceRange[1]]})}
                    className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500 text-sm"
                  />
                  <input
                    type="number"
                    placeholder={t('max')}
                    value={filters.priceRange[1]}
                    onChange={(e) => setFilters({...filters, priceRange: [filters.priceRange[0], parseInt(e.target.value) || 500]})}
                    className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500 text-sm"
                  />
                </div>
              </div>

              {/* Sort By */}
              <div className="mb-4 sm:mb-6">
                <label className="block text-sm font-medium text-gray-700 mb-2">
                  <SortAsc className="inline h-4 w-4 mr-1 rtl:mr-0 rtl:ml-1" />
                  {t('sort.by')}
                </label>
                <select
                  value={sortBy}
                  onChange={(e) => setSortBy(e.target.value)}
                  className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500 text-sm sm:text-base"
                >
                  <option value="time">{t('departure.time')}</option>
                  <option value="price">{t('price')}</option>
                  <option value="duration">{t('duration')}</option>
                </select>
              </div>
            </div>
          </div>

          {/* Results */}
          <div className="lg:col-span-3 order-1 lg:order-2">
            <div className="mb-4 sm:mb-6">
              <h2 className="text-xl sm:text-2xl font-bold text-gray-900 mb-2">
                {t('available.trains')}
              </h2>
              <p className="text-sm sm:text-base text-gray-600">
                {filteredTrains.length} {t('trains.found')}
              </p>
            </div>

            {filteredTrains.length === 0 ? (
              <div className="text-center py-8 sm:py-12">
                <Train className="h-12 w-12 text-gray-400 mx-auto mb-4" />
                <p className="text-gray-500">{t('no.trains')}</p>
              </div>
            ) : (
              <div className="space-y-3 sm:space-y-4">
                {filteredTrains.map((train) => (
                  <div key={train.id} className="bg-white rounded-lg shadow-md p-4 sm:p-6 hover:shadow-lg transition-shadow">
                    <div className="flex flex-col lg:flex-row lg:items-center lg:justify-between">
                      <div className="flex-1 mb-4 lg:mb-0">
                        <div className="flex flex-col sm:flex-row sm:items-center mb-3">
                          <h3 className="text-base sm:text-lg font-semibold text-gray-900 mb-1 sm:mb-0 sm:mr-3 rtl:mr-0 rtl:ml-3">
                            {train.name}
                          </h3>
                          <span className={`px-2 py-1 rounded-full text-xs font-medium ${
                            train.class === 'VIP' ? 'bg-yellow-100 text-yellow-800' :
                            train.class === 'Business' ? 'bg-blue-100 text-blue-800' :
                            'bg-gray-100 text-gray-800'
                          }`}>
                            {train.class}
                          </span>
                        </div>
                        
                        <div className="grid grid-cols-1 sm:grid-cols-3 gap-3 sm:gap-4 mb-4">
                          <div className="flex items-center space-x-2 rtl:space-x-reverse">
                            <Clock className="h-4 w-4 text-gray-400 flex-shrink-0" />
                            <div>
                              <p className="text-sm text-gray-600">{t('departure')}</p>
                              <p className="font-medium">{train.departure}</p>
                            </div>
                          </div>
                          <div className="flex items-center space-x-2 rtl:space-x-reverse">
                            <Clock className="h-4 w-4 text-gray-400 flex-shrink-0" />
                            <div>
                              <p className="text-sm text-gray-600">{t('arrival')}</p>
                              <p className="font-medium">{train.arrival}</p>
                            </div>
                          </div>
                          <div className="flex items-center space-x-2 rtl:space-x-reverse">
                            <Clock className="h-4 w-4 text-gray-400 flex-shrink-0" />
                            <div>
                              <p className="text-sm text-gray-600">{t('duration')}</p>
                              <p className="font-medium">{train.duration}</p>
                            </div>
                          </div>
                        </div>

                        <div className="flex flex-wrap gap-1 sm:gap-2 mb-3 sm:mb-4">
                          {train.amenities.map((amenity, index) => (
                            <span key={index} className="bg-blue-50 text-blue-700 px-2 py-1 rounded-md text-xs flex items-center">
                              {amenity === 'WiFi' && <Wifi className="h-3 w-3 mr-1 rtl:mr-0 rtl:ml-1" />}
                              {amenity === 'Meals' && <Coffee className="h-3 w-3 mr-1 rtl:mr-0 rtl:ml-1" />}
                              {amenity === 'Power Outlets' && <Zap className="h-3 w-3 mr-1 rtl:mr-0 rtl:ml-1" />}
                              {amenity === 'Lounge Access' && <Shield className="h-3 w-3 mr-1 rtl:mr-0 rtl:ml-1" />}
                              <span className="hidden sm:inline">{amenity}</span>
                            </span>
                          ))}
                        </div>

                        <p className="text-xs sm:text-sm text-gray-600">
                          {train.availability} {t('seats.available')}
                        </p>
                      </div>

                      <div className="flex flex-row sm:flex-col items-center sm:items-end justify-between sm:justify-start space-x-4 sm:space-x-0 sm:space-y-2">
                        <div className="text-left sm:text-right">
                          <p className="text-xl sm:text-2xl font-bold text-gray-900">
                            {train.price} {t('sdg')}
                          </p>
                          <p className="text-sm text-gray-600">{t('per.person')}</p>
                        </div>
                        <button
                          onClick={() => handleBooking(train)}
                          className="bg-blue-600 text-white px-4 sm:px-6 py-2 rounded-lg font-medium hover:bg-blue-700 transition-colors text-sm sm:text-base whitespace-nowrap"
                        >
                          {t('book.now')}
                        </button>
                      </div>
                    </div>
                  </div>
                ))}
              </div>
            )}
          </div>
        </div>
      </div>
    </div>
  );
};

export default SearchResults;