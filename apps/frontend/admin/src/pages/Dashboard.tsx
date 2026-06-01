import { useState, useEffect } from 'react';
import { MapPin, Route as RouteIcon, Train as TrainIcon, CalendarDays } from 'lucide-react';
import { trainsApi, routesApi, stationsApi, tripsApi } from '../services/api';
import { Trip } from '../types/infrastructure';
import { AR } from '../i18n/ar';

function fmt(iso: string): string {
  const d = new Date(iso);
  return isNaN(d.getTime()) ? '--' : d.toLocaleTimeString('ar-EG', { hour: '2-digit', minute: '2-digit' });
}

const Dashboard = () => {
  const [counts, setCounts] = useState({ trains: 0, routes: 0, stations: 0, tripsToday: 0 });
  const [todayTrips, setTodayTrips] = useState<Trip[]>([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const today = new Date().toISOString().slice(0, 10);
    Promise.all([
      trainsApi.getAll().catch(() => []),
      routesApi.getAll({ pageSize: 1000 }).catch(() => []),
      stationsApi.getAll({ pageSize: 1000 }).catch(() => []),
      tripsApi.getAll({ date: today }).catch(() => []),
    ])
      .then(([trains, routes, stations, trips]) => {
        setCounts({ trains: trains.length, routes: routes.length, stations: stations.length, tripsToday: trips.length });
        setTodayTrips(trips.slice(0, 6));
      })
      .finally(() => setLoading(false));
  }, []);

  const stats = [
    { label: AR.dashboard.trains, value: counts.trains, icon: TrainIcon, color: 'bg-admin-primary-700' },
    { label: AR.dashboard.routes, value: counts.routes, icon: RouteIcon, color: 'bg-sudan-gold-500' },
    { label: AR.dashboard.stations, value: counts.stations, icon: MapPin, color: 'bg-sudan-sand-600' },
    { label: AR.dashboard.tripsToday, value: counts.tripsToday, icon: CalendarDays, color: 'bg-sudan-red-600' },
  ];

  return (
    <div>
      <div className="mb-8">
        <h1 className="text-3xl font-bold text-gray-900">{AR.dashboard.title}</h1>
        <p className="text-gray-600 mt-2">{AR.dashboard.subtitle}</p>
      </div>

      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6 mb-8">
        {stats.map((stat) => (
          <div key={stat.label} className="admin-card">
            <div className="flex items-center justify-between">
              <div>
                <p className="text-sm text-gray-600 mb-1">{stat.label}</p>
                <p className="text-2xl font-bold text-gray-900">{loading ? '…' : stat.value}</p>
              </div>
              <div className={`${stat.color} p-3 rounded-lg`}>
                <stat.icon className="text-white" size={24} />
              </div>
            </div>
          </div>
        ))}
      </div>

      <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
        <div className="admin-card">
          <h2 className="text-xl font-bold text-gray-900 mb-4">{AR.dashboard.tripsTodayTitle}</h2>
          {loading ? (
            <p className="text-gray-500">{AR.common.loading}</p>
          ) : todayTrips.length === 0 ? (
            <p className="text-gray-500">{AR.dashboard.noTripsToday}</p>
          ) : (
            <div className="space-y-3">
              {todayTrips.map((trip) => (
                <div key={trip.id} className="flex items-center justify-between py-3 border-b border-gray-100 last:border-0">
                  <div>
                    <p className="font-medium text-gray-900">{trip.trainName}</p>
                    <p className="text-sm text-gray-600">{trip.routeName} · {fmt(trip.departureTime)}</p>
                  </div>
                  <span className="px-3 py-1 bg-admin-primary-50 text-admin-primary-800 text-sm rounded-full">
                    {AR.status[trip.status] || trip.status}
                  </span>
                </div>
              ))}
            </div>
          )}
        </div>

        <div className="admin-card">
          <h2 className="text-xl font-bold text-gray-900 mb-4">{AR.dashboard.bookingsTitle}</h2>
          <div className="flex items-center justify-center h-40 text-center">
            <p className="text-gray-500 text-sm">{AR.dashboard.bookingsPending}</p>
          </div>
        </div>
      </div>
    </div>
  );
};

export default Dashboard;
