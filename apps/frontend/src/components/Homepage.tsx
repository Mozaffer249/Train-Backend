import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { Search, Calendar, Users, MapPin, Star, Shield, Clock, Award, Phone, Mail, Headphones } from 'lucide-react';
import { useLanguage } from '../contexts/LanguageContext';

const Homepage: React.FC = () => {
  const navigate = useNavigate();
  const { t } = useLanguage();
  const [searchForm, setSearchForm] = useState({
    from: '',
    to: '',
    date: '',
    passengers: '1',
    class: 'economy'
  });

  const cities = [
    { id: 'khartoum', name: t('khartoum'), nameEn: 'Khartoum' },
    { id: 'atbara', name: t('atbara'), nameEn: 'Atbara' },
    { id: 'port.sudan', name: t('port.sudan'), nameEn: 'Port Sudan' },
    { id: 'kassala', name: t('kassala'), nameEn: 'Kassala' },
    { id: 'wad.medani', name: t('wad.medani'), nameEn: 'Wad Medani' },
    { id: 'nyala', name: t('nyala'), nameEn: 'Nyala' },
    { id: 'el.obeid', name: t('el.obeid'), nameEn: 'El Obeid' },
    { id: 'dongola', name: t('dongola'), nameEn: 'Dongola' },
    { id: 'sennar', name: t('sennar'), nameEn: 'Sennar' }
  ];

  const handleSearch = (e: React.FormEvent) => {
    e.preventDefault();
    navigate('/search', { state: searchForm });
  };

  const features = [
    {
      icon: <Shield className="h-8 w-8 text-blue-600" />,
      titleKey: 'safe.secure',
      descriptionKey: 'modern.safety.systems'
    },
    {
      icon: <Clock className="h-8 w-8 text-blue-600" />,
      titleKey: 'on.time.performance',
      descriptionKey: 'reliable.schedules'
    },
    {
      icon: <Star className="h-8 w-8 text-blue-600" />,
      titleKey: 'comfort.quality',
      descriptionKey: 'premium.seats'
    },
    {
      icon: <Award className="h-8 w-8 text-blue-600" />,
      titleKey: 'award.winning',
      descriptionKey: 'recognized.excellence'
    }
  ];

  return (
    <div className="min-h-screen bg-gray-50">
      {/* Hero Section */}
      <section className="relative bg-gradient-to-br from-blue-600 via-blue-700 to-blue-800 text-white min-h-screen flex items-center">
        <div className="absolute inset-0 bg-black/30"></div>
        <div className="absolute inset-0 bg-[url('https://images.pexels.com/photos/258510/pexels-photo-258510.jpeg')] bg-cover bg-center opacity-20"></div>
        <div className="relative w-full max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
          <div className="text-center mb-8">
            <h1 className="text-3xl sm:text-4xl md:text-5xl lg:text-6xl font-bold mb-4 leading-tight">
              {t('hero.title')}
            </h1>
            <p className="text-lg sm:text-xl md:text-2xl text-blue-100 max-w-3xl mx-auto leading-relaxed">
              {t('hero.subtitle')}
            </p>
          </div>

          {/* Search Form */}
          <div className="max-w-4xl mx-auto">
            <form onSubmit={handleSearch} className="bg-white rounded-2xl shadow-2xl p-4 sm:p-6 lg:p-8">
              <h2 className="text-xl sm:text-2xl font-bold text-gray-900 mb-6 text-center">
                {t('book.your.journey')}
              </h2>
              <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-5 gap-4 sm:gap-6">
                {/* From */}
                <div className="sm:col-span-1 lg:col-span-1">
                  <label className="block text-sm font-medium text-gray-700 mb-2">
                    <MapPin className="inline h-4 w-4 mr-1 rtl:mr-0 rtl:ml-1" />
                    {t('from')}
                  </label>
                  <select
                    value={searchForm.from}
                    onChange={(e) => setSearchForm({...searchForm, from: e.target.value})}
                    className="w-full px-3 py-3 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500 text-gray-900 text-sm sm:text-base"
                    required
                  >
                    <option value="">{t('select')}</option>
                    {cities.map(city => (
                      <option key={city.id} value={city.id}>{city.name}</option>
                    ))}
                  </select>
                </div>

                {/* To */}
                <div className="sm:col-span-1 lg:col-span-1">
                  <label className="block text-sm font-medium text-gray-700 mb-2">
                    <MapPin className="inline h-4 w-4 mr-1 rtl:mr-0 rtl:ml-1" />
                    {t('to')}
                  </label>
                  <select
                    value={searchForm.to}
                    onChange={(e) => setSearchForm({...searchForm, to: e.target.value})}
                    className="w-full px-3 py-3 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500 text-gray-900 text-sm sm:text-base"
                    required
                  >
                    <option value="">{t('select')}</option>
                    {cities.filter(city => city.id !== searchForm.from).map(city => (
                      <option key={city.id} value={city.id}>{city.name}</option>
                    ))}
                  </select>
                </div>

                {/* Date */}
                <div className="sm:col-span-2 lg:col-span-1">
                  <label className="block text-sm font-medium text-gray-700 mb-2">
                    <Calendar className="inline h-4 w-4 mr-1 rtl:mr-0 rtl:ml-1" />
                    {t('date')}
                  </label>
                  <input
                    type="date"
                    value={searchForm.date}
                    onChange={(e) => setSearchForm({...searchForm, date: e.target.value})}
                    className="w-full px-3 py-3 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500 text-gray-900 text-sm sm:text-base"
                    required
                  />
                </div>

                {/* Passengers */}
                <div className="sm:col-span-1 lg:col-span-1">
                  <label className="block text-sm font-medium text-gray-700 mb-2">
                    <Users className="inline h-4 w-4 mr-1 rtl:mr-0 rtl:ml-1" />
                    {t('passengers')}
                  </label>
                  <select
                    value={searchForm.passengers}
                    onChange={(e) => setSearchForm({...searchForm, passengers: e.target.value})}
                    className="w-full px-3 py-3 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500 text-gray-900 text-sm sm:text-base"
                  >
                    {[1,2,3,4,5,6,7,8].map(num => (
                      <option key={num} value={num}>{num}</option>
                    ))}
                  </select>
                </div>

                {/* Search Button */}
                <div className="sm:col-span-2 lg:col-span-1">
                  <label className="hidden lg:block text-sm font-medium text-transparent mb-2">.</label>
                  <button
                    type="submit"
                    className="w-full bg-blue-600 text-white px-4 py-3 rounded-lg font-medium hover:bg-blue-700 transition-colors flex items-center justify-center text-sm sm:text-base lg:mt-0 mt-4"
                  >
                    <Search className="h-5 w-5 mr-2 rtl:mr-0 rtl:ml-2" />
                    {t('search.trains')}
                  </button>
                </div>
              </div>
            </form>
          </div>
        </div>
      </section>

      {/* Quick Access Section */}
      <section className="py-8 bg-white">
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
          <div className="grid grid-cols-1 sm:grid-cols-3 gap-4">
            <div className="bg-blue-50 rounded-lg p-4 text-center hover:bg-blue-100 transition-colors cursor-pointer">
              <Calendar className="h-8 w-8 text-blue-600 mx-auto mb-2" />
              <h3 className="font-semibold text-gray-900 mb-1">{t('my.bookings')}</h3>
              <p className="text-sm text-gray-600">{t('view.manage.trips')}</p>
            </div>
            <div className="bg-green-50 rounded-lg p-4 text-center hover:bg-green-100 transition-colors cursor-pointer">
              <Clock className="h-8 w-8 text-green-600 mx-auto mb-2" />
              <h3 className="font-semibold text-gray-900 mb-1">{t('train.status')}</h3>
              <p className="text-sm text-gray-600">{t('check.live.status')}</p>
            </div>
            <div className="bg-yellow-50 rounded-lg p-4 text-center hover:bg-yellow-100 transition-colors cursor-pointer">
              <Headphones className="h-8 w-8 text-yellow-600 mx-auto mb-2" />
              <h3 className="font-semibold text-gray-900 mb-1">{t('customer.support')}</h3>
              <p className="text-sm text-gray-600">{t('24.7.support')}</p>
            </div>
          </div>
        </div>
      </section>

      {/* Features Section */}
      <section className="py-12 sm:py-16 lg:py-20 bg-gray-50">
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
          <div className="text-center mb-8 sm:mb-12 lg:mb-16">
            <h2 className="text-2xl sm:text-3xl md:text-4xl font-bold text-gray-900 mb-4">
              {t('why.choose.atbara.rail')}
            </h2>
            <p className="text-lg sm:text-xl text-gray-600 max-w-3xl mx-auto">
              {t('experience.future.travel')}
            </p>
          </div>

          <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-6 sm:gap-8">
            {features.map((feature, index) => (
              <div key={index} className="text-center group">
                <div className="bg-white rounded-xl shadow-lg p-6 sm:p-8 hover:shadow-xl transition-shadow">
                  <div className="flex justify-center mb-4">
                    {feature.icon}
                  </div>
                  <h3 className="text-lg sm:text-xl font-semibold text-gray-900 mb-3">
                    {t(feature.titleKey)}
                  </h3>
                  <p className="text-sm sm:text-base text-gray-600">
                    {t(feature.descriptionKey)}
                  </p>
                </div>
              </div>
            ))}
          </div>
        </div>
      </section>

      {/* Popular Routes Section */}
      <section className="py-12 sm:py-16 lg:py-20 bg-white">
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
          <div className="text-center mb-8 sm:mb-12 lg:mb-16">
            <h2 className="text-2xl sm:text-3xl md:text-4xl font-bold text-gray-900 mb-4">
              {t('popular.routes')}
            </h2>
            <p className="text-lg sm:text-xl text-gray-600">
              {t('discover.most.traveled')}
            </p>
          </div>

          <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-4 sm:gap-6 lg:gap-8">
            {[
              { from: 'Khartoum', to: 'Atbara', duration: '4h 30m', price: 'From 500 SDG' },
              { from: 'Khartoum', to: 'Port Sudan', duration: '8h 15m', price: 'From 800 SDG' },
              { from: 'Atbara', to: 'Kassala', duration: '3h 45m', price: 'From 400 SDG' },
              { from: 'Khartoum', to: 'Wad Medani', duration: '2h 30m', price: 'From 300 SDG' },
              { from: 'Khartoum', to: 'El Obeid', duration: '6h 00m', price: 'From 600 SDG' },
              { from: 'Atbara', to: 'Dongola', duration: '5h 15m', price: 'From 550 SDG' }
            ].map((route, index) => (
              <div key={index} className="bg-gradient-to-r from-blue-50 to-blue-100 rounded-xl p-4 sm:p-6 hover:shadow-lg transition-shadow cursor-pointer">
                <div className="flex items-center justify-between mb-4">
                  <div className="flex items-center space-x-3 rtl:space-x-reverse">
                    <div className="w-3 h-3 bg-blue-600 rounded-full"></div>
                    <span className="font-semibold text-gray-900 text-sm sm:text-base">{route.from}</span>
                  </div>
                  <div className="flex-1 h-px bg-gradient-to-r from-blue-300 to-blue-500 mx-4"></div>
                  <div className="flex items-center space-x-3 rtl:space-x-reverse">
                    <span className="font-semibold text-gray-900 text-sm sm:text-base">{route.to}</span>
                    <div className="w-3 h-3 bg-blue-600 rounded-full"></div>
                  </div>
                </div>
                <div className="flex justify-between items-center">
                  <span className="text-gray-600 text-sm sm:text-base">{route.duration}</span>
                  <span className="text-blue-600 font-semibold text-sm sm:text-base">{route.price}</span>
                </div>
              </div>
            ))}
          </div>
        </div>
      </section>

      {/* Contact Support Section */}
      <section className="py-12 bg-blue-600 text-white">
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
          <div className="text-center mb-8">
            <h2 className="text-2xl sm:text-3xl font-bold mb-4">{t('need.help')}</h2>
            <p className="text-blue-100 mb-6">{t('contact.support.team')}</p>
          </div>
          <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-4 sm:gap-6">
            <div className="bg-blue-700 rounded-lg p-4 text-center hover:bg-blue-800 transition-colors cursor-pointer">
              <Phone className="h-6 w-6 mx-auto mb-2" />
              <h3 className="font-semibold mb-1">{t('call.us')}</h3>
              <p className="text-blue-200 text-sm">+249 123 456 789</p>
            </div>
            <div className="bg-blue-700 rounded-lg p-4 text-center hover:bg-blue-800 transition-colors cursor-pointer">
              <Mail className="h-6 w-6 mx-auto mb-2" />
              <h3 className="font-semibold mb-1">{t('email.us')}</h3>
              <p className="text-blue-200 text-sm">support@atbara.sd</p>
            </div>
            <div className="bg-blue-700 rounded-lg p-4 text-center hover:bg-blue-800 transition-colors cursor-pointer sm:col-span-2 lg:col-span-1">
              <Headphones className="h-6 w-6 mx-auto mb-2" />
              <h3 className="font-semibold mb-1">{t('live.chat')}</h3>
              <p className="text-blue-200 text-sm">{t('available.24.7')}</p>
            </div>
          </div>
        </div>
      </section>
    </div>
  );
};

export default Homepage;
