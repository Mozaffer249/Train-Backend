import React, { useState } from 'react';
import { Calendar, Download, X, Clock, MapPin, Train, QrCode, Star } from 'lucide-react';
import { useLanguage } from '../contexts/LanguageContext';
import { useAuth } from '../contexts/AuthContext';

interface Trip {
  id: string;
  bookingRef: string;
  trainName: string;
  from: string;
  to: string;
  date: string;
  departure: string;
  arrival: string;
  class: string;
  seat: string;
  price: number;
  status: 'upcoming' | 'completed' | 'cancelled';
}

const Dashboard: React.FC = () => {
  const { t } = useLanguage();
  const { user } = useAuth();
  const [activeTab, setActiveTab] = useState<'upcoming' | 'past'>('upcoming');
  const [selectedTrip, setSelectedTrip] = useState<Trip | null>(null);

  const trips: Trip[] = [
    {
      id: '1',
      bookingRef: 'AT-ABC123456',
      trainName: 'Blue Nile Express',
      from: 'Khartoum',
      to: 'Atbara',
      date: '2024-01-15',
      departure: '08:00',
      arrival: '12:30',
      class: 'Business',
      seat: '12A',
      price: 500,
      status: 'upcoming'
    },
    {
      id: '2',
      bookingRef: 'AT-DEF789012',
      trainName: 'Sudan Express',
      from: 'Atbara',
      to: 'Port Sudan',
      date: '2024-01-10',
      departure: '14:00',
      arrival: '18:30',
      class: 'Economy',
      seat: '23B',
      price: 400,
      status: 'completed'
    },
    {
      id: '3',
      bookingRef: 'AT-GHI345678',
      trainName: 'Nile Valley Train',
      from: 'Khartoum',
      to: 'Wad Medani',
      date: '2024-01-08',
      departure: '10:15',
      arrival: '14:00',
      class: 'VIP',
      seat: '5A',
      price: 350,
      status: 'completed'
    }
  ];

  const upcomingTrips = trips.filter(trip => trip.status === 'upcoming');
  const pastTrips = trips.filter(trip => trip.status === 'completed');

  const handleDownloadTicket = (trip: Trip) => {
    // In a real app, this would generate and download a PDF ticket
    console.log('Downloading ticket for:', trip.bookingRef);
  };

  const handleCancelTrip = (tripId: string) => {
    // In a real app, this would call an API to cancel the trip
    console.log('Cancelling trip:', tripId);
  };

  return (
    <div className="min-h-screen bg-gray-50 py-8">
      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        {/* Header */}
        <div className="mb-8">
          <h1 className="text-3xl font-bold text-gray-900 mb-2">
            {t('my.trips')}
          </h1>
          <p className="text-gray-600">
          {t('welcome.back')}, {user?.name}
          </p>
        </div>

        {/* Tabs */}
        <div className="mb-8">
          <div className="border-b border-gray-200">
            <nav className="-mb-px flex space-x-8 rtl:space-x-reverse">
              <button
                onClick={() => setActiveTab('upcoming')}
                className={`py-4 px-1 border-b-2 font-medium text-sm ${
                  activeTab === 'upcoming'
                    ? 'border-emerald-500 text-emerald-600'
                    : 'border-transparent text-gray-500 hover:text-gray-700 hover:border-gray-300'
                }`}
              >
                {t('upcoming')} ({upcomingTrips.length})
              </button>
              <button
                onClick={() => setActiveTab('past')}
                className={`py-4 px-1 border-b-2 font-medium text-sm ${
                  activeTab === 'past'
                    ? 'border-emerald-500 text-emerald-600'
                    : 'border-transparent text-gray-500 hover:text-gray-700 hover:border-gray-300'
                }`}
              >
                {t('past')} ({pastTrips.length})
              </button>
            </nav>
          </div>
        </div>

        {/* Trip Cards */}
        <div className="grid grid-cols-1 lg:grid-cols-2 xl:grid-cols-3 gap-6">
          {(activeTab === 'upcoming' ? upcomingTrips : pastTrips).map((trip) => (
            <div key={trip.id} className="bg-white rounded-lg shadow-md overflow-hidden hover:shadow-lg transition-shadow">
              <div className="p-6">
                <div className="flex items-center justify-between mb-4">
                  <div>
                    <h3 className="text-lg font-semibold text-gray-900">{trip.trainName}</h3>
                    <p className="text-sm text-gray-600">{trip.bookingRef}</p>
                  </div>
                  <span className={`px-2 py-1 rounded-full text-xs font-medium ${
                    trip.class === 'VIP' ? 'bg-yellow-100 text-yellow-800' :
                    trip.class === 'Business' ? 'bg-blue-100 text-blue-800' :
                    'bg-gray-100 text-gray-800'
                  }`}>
                    {trip.class}
                  </span>
                </div>

                <div className="space-y-3 mb-4">
                  <div className="flex items-center space-x-3 rtl:space-x-reverse">
                    <MapPin className="h-4 w-4 text-gray-400" />
                    <span className="text-sm text-gray-600">
                      {trip.from} → {trip.to}
                    </span>
                  </div>
                  <div className="flex items-center space-x-3 rtl:space-x-reverse">
                    <Calendar className="h-4 w-4 text-gray-400" />
                    <span className="text-sm text-gray-600">{trip.date}</span>
                  </div>
                  <div className="flex items-center space-x-3 rtl:space-x-reverse">
                    <Clock className="h-4 w-4 text-gray-400" />
                    <span className="text-sm text-gray-600">
                      {trip.departure} - {trip.arrival}
                    </span>
                  </div>
                  <div className="flex items-center space-x-3 rtl:space-x-reverse">
                    <Train className="h-4 w-4 text-gray-400" />
                    <span className="text-sm text-gray-600">
                      Seat {trip.seat}
                    </span>
                  </div>
                </div>

                <div className="flex items-center justify-between pt-4 border-t border-gray-200">
                  <span className="text-lg font-bold text-gray-900">
                    {trip.price} SDG
                  </span>
                  <div className="flex space-x-2 rtl:space-x-reverse">
                    <button
                      onClick={() => handleDownloadTicket(trip)}
                      className="bg-blue-600 text-white px-3 py-1 rounded text-sm hover:bg-blue-700 transition-colors flex items-center"
                    >
                      <Download className="h-4 w-4 mr-1 rtl:mr-0 rtl:ml-1" />
                      {t('download.ticket')}
                    </button>
                    {trip.status === 'upcoming' && (
                      <button
                        onClick={() => handleCancelTrip(trip.id)}
                        className="border border-red-300 text-red-600 px-3 py-1 rounded text-sm hover:bg-red-50 transition-colors"
                      >
                        <X className="h-4 w-4" />
                      </button>
                    )}
                  </div>
                </div>
              </div>
            </div>
          ))}
        </div>

        {/* Empty State */}
        {(activeTab === 'upcoming' ? upcomingTrips : pastTrips).length === 0 && (
          <div className="text-center py-12">
            <Train className="h-12 w-12 text-gray-400 mx-auto mb-4" />
            <p className="text-gray-500">
            {activeTab === 'upcoming' ? t('no.upcoming.trips') : t('no.past.trips')}
            </p>
          </div>
        )}

        {/* Ticket Modal */}
        {selectedTrip && (
          <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50">
            <div className="bg-white rounded-lg max-w-md w-full mx-4 p-6">
              <div className="flex items-center justify-between mb-6">
              <h3 className="text-xl font-bold text-gray-900">{t('e.ticket')}</h3>
                <button
                  onClick={() => setSelectedTrip(null)}
                  className="text-gray-400 hover:text-gray-600"
                >
                  <X className="h-6 w-6" />
                </button>
              </div>

              <div className="space-y-4">
                <div className="text-center">
                  <QrCode className="h-24 w-24 mx-auto mb-4 text-gray-400" />
                <p className="text-sm text-gray-600">{t('scan.qr.code')}</p>
                </div>

                <div className="bg-gray-50 rounded-lg p-4 space-y-2">
                  <div className="flex justify-between">
                  <span className="text-gray-600">{t('booking.ref')}</span>
                    <span className="font-medium">{selectedTrip.bookingRef}</span>
                  </div>
                  <div className="flex justify-between">
                  <span className="text-gray-600">{t('train')}</span>
                    <span className="font-medium">{selectedTrip.trainName}</span>
                  </div>
                  <div className="flex justify-between">
                  <span className="text-gray-600">{t('route')}:</span>
                  <span className="font-medium">{t(selectedTrip.from.toLowerCase())} → {t(selectedTrip.to.toLowerCase())}</span>
                  </div>
                  <div className="flex justify-between">
                  <span className="text-gray-600">{t('date')}:</span>
                    <span className="font-medium">{selectedTrip.date}</span>
                  </div>
                  <div className="flex justify-between">
                  <span className="text-gray-600">{t('departure')}:</span>
                    <span className="font-medium">{selectedTrip.departure}</span>
                  </div>
                  <div className="flex justify-between">
                  <span className="text-gray-600">{t('seat')}:</span>
                    <span className="font-medium">{selectedTrip.seat}</span>
                  </div>
                  <div className="flex justify-between">
                  <span className="text-gray-600">{t('class')}:</span>
                  <span className="font-medium">{t(selectedTrip.class.toLowerCase())}</span>
                  </div>
                </div>

                <button
                  onClick={() => handleDownloadTicket(selectedTrip)}
                  className="w-full bg-blue-600 text-white py-2 rounded-lg hover:bg-blue-700 transition-colors"
                >
                {t('download.pdf')}
                </button>
              </div>
            </div>
          </div>
        )}
      </div>
    </div>
  );
};

export default Dashboard;