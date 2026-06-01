import { useState, useEffect } from 'react';
import { useNavigate, Link } from 'react-router-dom';
import { Search, Calendar, Users, MapPin, Star, Shield, Clock, Award, Phone, Mail, Headphones } from 'lucide-react';
import { useLanguage } from '../contexts/LanguageContext';
import { catalogApi } from '../services/api';
import type { StationDto } from '../types/api';

export default function Homepage() {
  const navigate = useNavigate();
  const { t } = useLanguage();

  const [stations, setStations] = useState<StationDto[]>([]);
  const [stationsLoading, setStationsLoading] = useState(true);
  const [searchForm, setSearchForm] = useState({
    fromId: '',
    toId: '',
    date: '',
    passengers: '1',
    class: 'economy',
  });

  useEffect(() => {
    catalogApi
      .getStations({ isActive: true, pageSize: 200 })
      .then((data) => setStations(data || []))
      .catch(() => setStations([]))
      .finally(() => setStationsLoading(false));
  }, []);

  const stationName = (s: StationDto) => s.nameAr || s.nameEn;
  const stationLabel = (s: StationDto) => `${stationName(s)} — ${s.cityName}`;

  const handleSearch = (e: React.FormEvent) => {
    e.preventDefault();
    const origin = stations.find((s) => String(s.id) === searchForm.fromId);
    const destination = stations.find((s) => String(s.id) === searchForm.toId);
    navigate('/search', {
      state: {
        originStationId: Number(searchForm.fromId),
        destinationStationId: Number(searchForm.toId),
        originName: origin ? stationName(origin) : '',
        destinationName: destination ? stationName(destination) : '',
        date: searchForm.date,
        passengers: Number(searchForm.passengers),
        class: searchForm.class,
      },
    });
  };

  const features = [
    { icon: <Shield className="h-8 w-8 text-sudan-green-700" />, titleKey: 'safe.secure', descriptionKey: 'modern.safety.systems' },
    { icon: <Clock className="h-8 w-8 text-sudan-green-700" />, titleKey: 'on.time.performance', descriptionKey: 'reliable.schedules' },
    { icon: <Star className="h-8 w-8 text-sudan-green-700" />, titleKey: 'comfort.quality', descriptionKey: 'premium.seats' },
    { icon: <Award className="h-8 w-8 text-sudan-green-700" />, titleKey: 'award.winning', descriptionKey: 'recognized.excellence' },
  ];

  const selectClass =
    'w-full px-3 py-3 border border-gray-300 rounded-lg focus:ring-2 focus:ring-sudan-green-500 focus:border-sudan-green-500 text-gray-900 text-sm sm:text-base disabled:bg-gray-100';

  return (
    <div className="min-h-screen bg-sudan-sand-50">
      {/* Hero — Sudanese gradient + Meroë / palm / Nile-wave SVG motif */}
      <section className="relative overflow-hidden text-white py-16 lg:py-24 bg-gradient-to-br from-sudan-green-800 via-sudan-sand-600 to-sudan-green-700">
        <div className="absolute inset-0 opacity-25" aria-hidden="true">
          <svg viewBox="0 0 1200 400" preserveAspectRatio="xMidYMid slice" className="h-full w-full">
            {/* Meroë pyramid silhouettes */}
            <g fill="#3d2a12">
              <polygon points="60,320 130,210 200,320" />
              <polygon points="170,320 240,180 310,320" />
              <polygon points="290,320 350,230 410,320" />
              <polygon points="780,320 850,200 920,320" />
              <polygon points="900,320 970,240 1040,320" />
              <polygon points="1020,320 1080,225 1140,320" />
            </g>
            {/* Palm tree silhouettes */}
            <g fill="#064e2a" opacity="0.85">
              <rect x="490" y="240" width="6" height="80" />
              <path d="M493,240 q-40,-15 -55,-50 q35,5 55,30 q20,-25 55,-30 q-15,35 -55,50 z" />
              <rect x="640" y="220" width="6" height="100" />
              <path d="M643,220 q-45,-18 -60,-60 q40,6 60,35 q20,-29 60,-35 q-15,42 -60,60 z" />
            </g>
            {/* Nile wave at the bottom */}
            <path d="M0,360 Q150,335 300,360 T600,360 T900,360 T1200,360 L1200,400 L0,400 Z" fill="#1B4D6B" opacity="0.55" />
            <path d="M0,380 Q150,360 300,380 T600,380 T900,380 T1200,380 L1200,400 L0,400 Z" fill="#1B4D6B" opacity="0.75" />
          </svg>
        </div>
        <div className="absolute inset-0 bg-black/20" aria-hidden="true" />

        <div className="relative w-full max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
          <div className="text-center mb-8">
            <h1 className="text-3xl sm:text-4xl md:text-5xl font-bold mb-4 leading-tight drop-shadow">{t('hero.title')}</h1>
            <p className="text-lg sm:text-xl text-sudan-sand-100 max-w-3xl mx-auto">{t('hero.subtitle')}</p>
          </div>

          <div className="max-w-6xl mx-auto">
            <form onSubmit={handleSearch} className="bg-white rounded-2xl shadow-2xl p-4 sm:p-6 lg:p-8 border-t-4 border-sudan-gold-400">
              <h2 className="text-xl sm:text-2xl font-bold text-gray-900 mb-6 text-center">{t('book.your.journey')}</h2>
              <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-7 gap-4 sm:gap-6">
                <div className="lg:col-span-2">
                  <label className="block text-sm font-medium text-gray-700 mb-2">
                    <MapPin className="inline h-4 w-4 ms-0 me-1" />
                    {t('from')}
                  </label>
                  <select
                    value={searchForm.fromId}
                    onChange={(e) => setSearchForm({ ...searchForm, fromId: e.target.value })}
                    className={selectClass}
                    required
                    disabled={stationsLoading}
                  >
                    <option value="">{stationsLoading ? t('loading') : t('select')}</option>
                    {stations.map((s) => (
                      <option key={s.id} value={s.id}>{stationLabel(s)}</option>
                    ))}
                  </select>
                </div>

                <div className="lg:col-span-2">
                  <label className="block text-sm font-medium text-gray-700 mb-2">
                    <MapPin className="inline h-4 w-4 ms-0 me-1" />
                    {t('to')}
                  </label>
                  <select
                    value={searchForm.toId}
                    onChange={(e) => setSearchForm({ ...searchForm, toId: e.target.value })}
                    className={selectClass}
                    required
                    disabled={stationsLoading}
                  >
                    <option value="">{stationsLoading ? t('loading') : t('select')}</option>
                    {stations.filter((s) => String(s.id) !== searchForm.fromId).map((s) => (
                      <option key={s.id} value={s.id}>{stationLabel(s)}</option>
                    ))}
                  </select>
                </div>

                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-2">
                    <Calendar className="inline h-4 w-4 ms-0 me-1" />
                    {t('date')}
                  </label>
                  <input
                    type="date"
                    value={searchForm.date}
                    onChange={(e) => setSearchForm({ ...searchForm, date: e.target.value })}
                    className={selectClass}
                    required
                  />
                </div>

                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-2">
                    <Users className="inline h-4 w-4 ms-0 me-1" />
                    {t('passengers')}
                  </label>
                  <input
                    type="number"
                    min={1}
                    max={20}
                    step={1}
                    value={searchForm.passengers}
                    onChange={(e) => {
                      const raw = e.target.value.replace(/\D/g, '');
                      setSearchForm({ ...searchForm, passengers: raw || '1' });
                    }}
                    className={selectClass}
                    inputMode="numeric"
                  />
                </div>

                <div>
                  <label className="hidden lg:block text-sm font-medium text-transparent mb-2">.</label>
                  <button
                    type="submit"
                    className="w-full bg-sudan-green-700 text-white px-4 py-3 rounded-lg font-medium hover:bg-sudan-green-800 transition-colors flex items-center justify-center text-sm sm:text-base mt-4 lg:mt-0"
                  >
                    <Search className="h-5 w-5 ms-0 me-2" />
                    {t('search.trains')}
                  </button>
                </div>
              </div>
            </form>
          </div>
        </div>
      </section>

      <section className="py-8 bg-white">
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
          <div className="grid grid-cols-1 sm:grid-cols-3 gap-4">
            <Link to="/dashboard" className="bg-sudan-green-50 rounded-lg p-4 text-center hover:bg-sudan-green-100 transition-colors">
              <Calendar className="h-8 w-8 text-sudan-green-700 mx-auto mb-2" />
              <h3 className="font-semibold text-gray-900 mb-1">{t('my.bookings')}</h3>
              <p className="text-sm text-gray-600">{t('view.manage.trips')}</p>
            </Link>
            <div className="bg-sudan-gold-50 rounded-lg p-4 text-center">
              <Clock className="h-8 w-8 text-sudan-gold-600 mx-auto mb-2" />
              <h3 className="font-semibold text-gray-900 mb-1">{t('train.status')}</h3>
              <p className="text-sm text-gray-600">{t('check.live.status')}</p>
            </div>
            <div className="bg-sudan-sand-100 rounded-lg p-4 text-center">
              <Headphones className="h-8 w-8 text-sudan-sand-700 mx-auto mb-2" />
              <h3 className="font-semibold text-gray-900 mb-1">{t('customer.support')}</h3>
              <p className="text-sm text-gray-600">{t('24.7.support')}</p>
            </div>
          </div>
        </div>
      </section>

      <section className="py-12 sm:py-16 bg-sudan-sand-50">
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
          <div className="text-center mb-8 sm:mb-12">
            <h2 className="text-2xl sm:text-3xl md:text-4xl font-bold text-gray-900 mb-4">{t('why.choose.atbara.rail')}</h2>
            <p className="text-lg sm:text-xl text-gray-600 max-w-3xl mx-auto">{t('experience.future.travel')}</p>
            <div className="mx-auto mt-4 h-1 w-24 bg-sudan-gold-400 rounded-full"></div>
          </div>
          <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-6 sm:gap-8">
            {features.map((feature) => (
              <div key={feature.titleKey} className="text-center">
                <div className="bg-white rounded-xl shadow-lg p-6 sm:p-8 hover:shadow-xl transition-shadow border-t-2 border-sudan-green-700">
                  <div className="flex justify-center mb-4">{feature.icon}</div>
                  <h3 className="text-lg sm:text-xl font-semibold text-gray-900 mb-3">{t(feature.titleKey)}</h3>
                  <p className="text-sm sm:text-base text-gray-600">{t(feature.descriptionKey)}</p>
                </div>
              </div>
            ))}
          </div>
        </div>
      </section>

      <section className="py-12 bg-sudan-green-800 text-white">
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
          <div className="text-center mb-8">
            <h2 className="text-2xl sm:text-3xl font-bold mb-4">{t('need.help')}</h2>
            <p className="text-sudan-sand-100 mb-6">{t('contact.support.team')}</p>
          </div>
          <div className="grid grid-cols-1 sm:grid-cols-3 gap-4 sm:gap-6">
            <div className="bg-sudan-green-900/50 rounded-lg p-4 text-center">
              <Phone className="h-6 w-6 mx-auto mb-2 text-sudan-gold-300" />
              <h3 className="font-semibold mb-1">{t('call.us')}</h3>
              <p className="text-sudan-sand-100 text-sm">+249 123 456 789</p>
            </div>
            <div className="bg-sudan-green-900/50 rounded-lg p-4 text-center">
              <Mail className="h-6 w-6 mx-auto mb-2 text-sudan-gold-300" />
              <h3 className="font-semibold mb-1">{t('email.us')}</h3>
              <p className="text-sudan-sand-100 text-sm">support@sudantrains.sd</p>
            </div>
            <div className="bg-sudan-green-900/50 rounded-lg p-4 text-center">
              <Headphones className="h-6 w-6 mx-auto mb-2 text-sudan-gold-300" />
              <h3 className="font-semibold mb-1">{t('live.chat')}</h3>
              <p className="text-sudan-sand-100 text-sm">{t('available.24.7')}</p>
            </div>
          </div>
        </div>
      </section>
    </div>
  );
}
