import React, { useState } from 'react';
import { Users, Train, Calendar, DollarSign, TrendingUp, MapPin, Clock, Plus, Edit, Trash2 } from 'lucide-react';
import { useLanguage } from '../contexts/LanguageContext';
import { useAuth } from '../contexts/AuthContext';

interface AdminStats {
  totalBookings: number;
  totalRevenue: number;
  totalTrains: number;
  totalUsers: number;
}

interface TrainRoute {
  id: string;
  name: string;
  from: string;
  to: string;
  departure: string;
  arrival: string;
  price: number;
  class: string;
  capacity: number;
  booked: number;
}

const AdminPanel: React.FC = () => {
  const { t } = useLanguage();
  const { user } = useAuth();
  const [activeTab, setActiveTab] = useState<'overview' | 'trains' | 'bookings' | 'users'>('overview');

  const stats: AdminStats = {
    totalBookings: 1250,
    totalRevenue: 625000,
    totalTrains: 25,
    totalUsers: 8420
  };

  const trainRoutes: TrainRoute[] = [
    {
      id: '1',
      name: 'Blue Nile Express',
      from: 'Khartoum',
      to: 'Atbara',
      departure: '08:00',
      arrival: '12:30',
      price: 500,
      class: 'Business',
      capacity: 200,
      booked: 145
    },
    {
      id: '2',
      name: 'Sudan Express',
      from: 'Atbara',
      to: 'Port Sudan',
      departure: '14:00',
      arrival: '18:30',
      price: 400,
      class: 'Economy',
      capacity: 300,
      booked: 267
    },
    {
      id: '3',
      name: 'Nile Valley Train',
      from: 'Khartoum',
      to: 'Wad Medani',
      departure: '10:15',
      arrival: '14:00',
      price: 350,
      class: 'VIP',
      capacity: 100,
      booked: 78
    }
  ];

  if (user?.role !== 'admin') {
    return (
      <div className="min-h-screen flex items-center justify-center">
        <div className="text-center">
          <h2 className="text-2xl font-bold text-gray-900 mb-4">{t('access.denied')}</h2>
          <p className="text-gray-600">{t('no.permission')}</p>
        </div>
      </div>
    );
  }

  return (
    <div className="min-h-screen bg-gray-50 py-8">
      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        {/* Header */}
        <div className="mb-8">
          <h1 className="text-3xl font-bold text-gray-900 mb-2">
            {t('admin.dashboard')}
          </h1>
          <p className="text-gray-600">
            {t('manage.train.booking.system')}
          </p>
        </div>

        {/* Tabs */}
        <div className="mb-8">
          <div className="border-b border-gray-200">
            <nav className="-mb-px flex space-x-8 rtl:space-x-reverse">
              {[
                { id: 'overview', name: t('overview'), icon: TrendingUp },
                { id: 'trains', name: t('trains'), icon: Train },
                { id: 'bookings', name: t('bookings'), icon: Calendar },
                { id: 'users', name: t('users'), icon: Users }
              ].map((tab) => (
                <button
                  key={tab.id}
                  onClick={() => setActiveTab(tab.id as any)}
                  className={`flex items-center py-4 px-1 border-b-2 font-medium text-sm ${
                    activeTab === tab.id
                      ? 'border-emerald-500 text-emerald-600'
                      : 'border-transparent text-gray-500 hover:text-gray-700 hover:border-gray-300'
                  }`}
                >
                  <tab.icon className="h-4 w-4 mr-2 rtl:mr-0 rtl:ml-2" />
                  {tab.name}
                </button>
              ))}
            </nav>
          </div>
        </div>

        {/* Overview Tab */}
        {activeTab === 'overview' && (
          <div className="space-y-8">
            {/* Stats Cards */}
            <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6">
              <div className="bg-white rounded-lg shadow-md p-6">
                <div className="flex items-center">
                  <Calendar className="h-8 w-8 text-blue-600" />
                  <div className="ml-4 rtl:ml-0 rtl:mr-4">
                    <p className="text-sm font-medium text-gray-600">{t('total.bookings')}</p>
                    <p className="text-2xl font-bold text-gray-900">{stats.totalBookings}</p>
                  </div>
                </div>
              </div>
              <div className="bg-white rounded-lg shadow-md p-6">
                <div className="flex items-center">
                  <DollarSign className="h-8 w-8 text-blue-600" />
                  <div className="ml-4 rtl:ml-0 rtl:mr-4">
                    <p className="text-sm font-medium text-gray-600">{t('total.revenue')}</p>
                    <p className="text-2xl font-bold text-gray-900">{stats.totalRevenue.toLocaleString()} {t('sdg')}</p>
                  </div>
                </div>
              </div>
              <div className="bg-white rounded-lg shadow-md p-6">
                <div className="flex items-center">
                  <Train className="h-8 w-8 text-blue-600" />
                  <div className="ml-4 rtl:ml-0 rtl:mr-4">
                    <p className="text-sm font-medium text-gray-600">{t('active.trains')}</p>
                    <p className="text-2xl font-bold text-gray-900">{stats.totalTrains}</p>
                  </div>
                </div>
              </div>
              <div className="bg-white rounded-lg shadow-md p-6">
                <div className="flex items-center">
                  <Users className="h-8 w-8 text-blue-600" />
                  <div className="ml-4 rtl:ml-0 rtl:mr-4">
                    <p className="text-sm font-medium text-gray-600">{t('total.users')}</p>
                    <p className="text-2xl font-bold text-gray-900">{stats.totalUsers}</p>
                  </div>
                </div>
              </div>
            </div>

            {/* Recent Activity */}
            <div className="bg-white rounded-lg shadow-md p-6">
              <h3 className="text-lg font-semibold text-gray-900 mb-4">{t('recent.activity')}</h3>
              <div className="space-y-4">
                {[
                  { action: t('new.booking'), details: `أحمد محمد حجز من ${t('khartoum')} إلى ${t('atbara')}`, time: `2 ${t('minutes.ago')}` },
                  { action: t('payment.received'), details: `500 ${t('sdg')} للحجز AT-ABC123456`, time: `5 ${t('minutes.ago')}` },
                  { action: t('train.schedule.updated'), details: 'تم تغيير وقت مغادرة قطار النيل الأزرق السريع', time: `1 ${t('hour.ago')}` },
                  { action: t('new.user.registration'), details: 'فاطمة الزهراء انضمت للمنصة', time: `2 ${t('hours.ago')}` }
                ].map((activity, index) => (
                  <div key={index} className="flex items-center justify-between py-3 border-b border-gray-200 last:border-b-0">
                    <div>
                      <p className="font-medium text-gray-900">{activity.action}</p>
                      <p className="text-sm text-gray-600">{activity.details}</p>
                    </div>
                    <span className="text-sm text-gray-500">{activity.time}</span>
                  </div>
                ))}
              </div>
            </div>
          </div>
        )}

        {/* Trains Tab */}
        {activeTab === 'trains' && (
          <div className="space-y-6">
            <div className="flex items-center justify-between">
              <h3 className="text-lg font-semibold text-gray-900">{t('train.routes')}</h3>
              <button className="bg-blue-600 text-white px-4 py-2 rounded-lg hover:bg-blue-700 transition-colors flex items-center">
                <Plus className="h-4 w-4 mr-2 rtl:mr-0 rtl:ml-2" />
                {t('add.route')}
              </button>
            </div>
            
            <div className="bg-white rounded-lg shadow-md overflow-hidden">
              <table className="min-w-full divide-y divide-gray-200">
                <thead className="bg-gray-50">
                  <tr>
                    <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                      {t('train')}
                    </th>
                    <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                      {t('route')}
                    </th>
                    <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                      {t('schedule')}
                    </th>
                    <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                      {t('capacity')}
                    </th>
                    <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                      {t('price')}
                    </th>
                    <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                      {t('actions')}
                    </th>
                  </tr>
                </thead>
                <tbody className="bg-white divide-y divide-gray-200">
                  {trainRoutes.map((route) => (
                    <tr key={route.id}>
                      <td className="px-6 py-4 whitespace-nowrap">
                        <div>
                          <div className="text-sm font-medium text-gray-900">{route.name}</div>
                          <div className="text-sm text-gray-500">{t(route.class.toLowerCase())}</div>
                        </div>
                      </td>
                      <td className="px-6 py-4 whitespace-nowrap">
                        <div className="flex items-center">
                          <MapPin className="h-4 w-4 text-gray-400 mr-1 rtl:mr-0 rtl:ml-1" />
                          <span className="text-sm text-gray-900">{t(route.from.toLowerCase())} → {t(route.to.toLowerCase())}</span>
                        </div>
                      </td>
                      <td className="px-6 py-4 whitespace-nowrap">
                        <div className="flex items-center">
                          <Clock className="h-4 w-4 text-gray-400 mr-1 rtl:mr-0 rtl:ml-1" />
                          <span className="text-sm text-gray-900">{route.departure} - {route.arrival}</span>
                        </div>
                      </td>
                      <td className="px-6 py-4 whitespace-nowrap">
                        <div className="text-sm text-gray-900">
                          {route.booked}/{route.capacity}
                        </div>
                        <div className="w-full bg-gray-200 rounded-full h-2">
                          <div
                            className="bg-blue-600 h-2 rounded-full"
                            style={{ width: `${(route.booked / route.capacity) * 100}%` }}
                          ></div>
                        </div>
                      </td>
                      <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
                        {route.price} {t('sdg')}
                      </td>
                      <td className="px-6 py-4 whitespace-nowrap text-sm font-medium">
                        <div className="flex space-x-2 rtl:space-x-reverse">
                          <button className="text-indigo-600 hover:text-indigo-900">
                            <Edit className="h-4 w-4" />
                          </button>
                          <button className="text-red-600 hover:text-red-900">
                            <Trash2 className="h-4 w-4" />
                          </button>
                        </div>
                      </td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
          </div>
        )}

        {/* Bookings Tab */}
        {activeTab === 'bookings' && (
          <div className="bg-white rounded-lg shadow-md p-6">
            <h3 className="text-lg font-semibold text-gray-900 mb-4">{t('recent.bookings')}</h3>
            <div className="space-y-4">
              {[
                { id: 'AT-ABC123456', passenger: 'أحمد محمد', route: `${t('khartoum')} → ${t('atbara')}`, date: '2024-01-15', status: t('confirmed') },
                { id: 'AT-DEF789012', passenger: 'فاطمة الزهراء', route: `${t('atbara')} → ${t('port.sudan')}`, date: '2024-01-14', status: t('confirmed') },
                { id: 'AT-GHI345678', passenger: 'عمر المنصوري', route: `${t('khartoum')} → ${t('wad.medani')}`, date: '2024-01-13', status: t('cancelled') }
              ].map((booking, index) => (
                <div key={index} className="flex items-center justify-between py-3 border-b border-gray-200 last:border-b-0">
                  <div>
                    <p className="font-medium text-gray-900">{booking.id}</p>
                    <p className="text-sm text-gray-600">{booking.passenger} • {booking.route}</p>
                  </div>
                  <div className="text-right">
                    <p className="text-sm text-gray-900">{booking.date}</p>
                    <span className={`inline-block px-2 py-1 text-xs font-medium rounded-full ${
                      booking.status === t('confirmed') ? 'bg-green-100 text-green-800' : 'bg-red-100 text-red-800'
                    }`}>
                      {booking.status}
                    </span>
                  </div>
                </div>
              ))}
            </div>
          </div>
        )}

        {/* Users Tab */}
        {activeTab === 'users' && (
          <div className="bg-white rounded-lg shadow-md p-6">
            <h3 className="text-lg font-semibold text-gray-900 mb-4">{t('user.management')}</h3>
            <div className="space-y-4">
              {[
                { name: 'أحمد محمد', email: 'ahmed@example.com', joinDate: '2024-01-10', trips: 5 },
                { name: 'فاطمة الزهراء', email: 'fatima@example.com', joinDate: '2024-01-08', trips: 3 },
                { name: 'عمر المنصوري', email: 'omar@example.com', joinDate: '2024-01-05', trips: 8 }
              ].map((user, index) => (
                <div key={index} className="flex items-center justify-between py-3 border-b border-gray-200 last:border-b-0">
                  <div>
                    <p className="font-medium text-gray-900">{user.name}</p>
                    <p className="text-sm text-gray-600">{user.email}</p>
                  </div>
                  <div className="text-right">
                    <p className="text-sm text-gray-900">{t('joined')} {user.joinDate}</p>
                    <p className="text-sm text-gray-600">{user.trips} {t('trips')}</p>
                  </div>
                </div>
              ))}
            </div>
          </div>
        )}
      </div>
    </div>
  );
};

export default AdminPanel;