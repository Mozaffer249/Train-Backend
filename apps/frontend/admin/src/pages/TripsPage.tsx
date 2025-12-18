import { useState } from 'react';
import { Search, Plus, Calendar } from 'lucide-react';

const TripsPage = () => {
  const [searchTerm, setSearchTerm] = useState('');

  const trips = [
    { id: 201, train: 'Express 101', route: 'Khartoum → Atbara', departure: '06:00 AM', arrival: '10:30 AM', date: '2024-12-20', status: 'Scheduled' },
    { id: 202, train: 'Regional 102', route: 'Atbara → Port Sudan', departure: '08:00 AM', arrival: '02:00 PM', date: '2024-12-20', status: 'In Transit' },
    { id: 203, train: 'Express 103', route: 'Khartoum → Wad Madani', departure: '09:00 AM', arrival: '11:30 AM', date: '2024-12-20', status: 'Completed' },
    { id: 204, train: 'Local 104', route: 'Port Sudan → Atbara', departure: '03:00 PM', arrival: '09:00 PM', date: '2024-12-20', status: 'Scheduled' },
    { id: 205, train: 'Express 105', route: 'Atbara → Khartoum', departure: '05:00 PM', arrival: '09:30 PM', date: '2024-12-20', status: 'Delayed' },
  ];

  return (
    <div>
      <div className="mb-8 flex items-center justify-between">
        <div>
          <h1 className="text-3xl font-bold text-gray-900">Trips</h1>
          <p className="text-gray-600 mt-2">Manage train schedules and routes</p>
        </div>
        <div className="flex gap-3">
          <button className="admin-button-secondary flex items-center gap-2">
            <Calendar size={20} />
            Schedule
          </button>
          <button className="admin-button flex items-center gap-2">
            <Plus size={20} />
            Add Trip
          </button>
        </div>
      </div>

      <div className="admin-card mb-6">
        <div className="relative">
          <Search className="absolute left-3 top-1/2 transform -translate-y-1/2 text-gray-400" size={20} />
          <input
            type="text"
            placeholder="Search trips..."
            value={searchTerm}
            onChange={(e) => setSearchTerm(e.target.value)}
            className="w-full pl-10 pr-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-admin-primary-500"
          />
        </div>
      </div>

      <div className="admin-card">
        <div className="overflow-x-auto">
          <table className="w-full">
            <thead className="bg-gray-50 border-b border-gray-200">
              <tr>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                  Trip ID
                </th>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                  Train
                </th>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                  Route
                </th>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                  Departure
                </th>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                  Arrival
                </th>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                  Date
                </th>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                  Status
                </th>
              </tr>
            </thead>
            <tbody className="bg-white divide-y divide-gray-200">
              {trips.map((trip) => (
                <tr key={trip.id} className="hover:bg-gray-50">
                  <td className="px-6 py-4 whitespace-nowrap">
                    <div className="font-medium text-gray-900">#{trip.id}</div>
                  </td>
                  <td className="px-6 py-4 whitespace-nowrap text-gray-900">
                    {trip.train}
                  </td>
                  <td className="px-6 py-4 whitespace-nowrap text-gray-600">
                    {trip.route}
                  </td>
                  <td className="px-6 py-4 whitespace-nowrap text-gray-600">
                    {trip.departure}
                  </td>
                  <td className="px-6 py-4 whitespace-nowrap text-gray-600">
                    {trip.arrival}
                  </td>
                  <td className="px-6 py-4 whitespace-nowrap text-gray-600">
                    {trip.date}
                  </td>
                  <td className="px-6 py-4 whitespace-nowrap">
                    <span className={`px-3 py-1 text-xs rounded-full ${
                      trip.status === 'Scheduled' ? 'bg-blue-100 text-blue-800' :
                      trip.status === 'In Transit' ? 'bg-green-100 text-green-800' :
                      trip.status === 'Completed' ? 'bg-gray-100 text-gray-800' :
                      'bg-red-100 text-red-800'
                    }`}>
                      {trip.status}
                    </span>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      </div>
    </div>
  );
};

export default TripsPage;
