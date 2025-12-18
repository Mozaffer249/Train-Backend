import { useState } from 'react';
import { Search, Filter, Download } from 'lucide-react';

const BookingsPage = () => {
  const [searchTerm, setSearchTerm] = useState('');

  const bookings = [
    { id: 1001, passenger: 'Ahmed Hassan', route: 'Khartoum → Atbara', date: '2024-12-20', status: 'Confirmed', amount: 'SDG 500' },
    { id: 1002, passenger: 'Fatima Ali', route: 'Atbara → Port Sudan', date: '2024-12-21', status: 'Pending', amount: 'SDG 750' },
    { id: 1003, passenger: 'Mohamed Ibrahim', route: 'Khartoum → Wad Madani', date: '2024-12-22', status: 'Confirmed', amount: 'SDG 300' },
    { id: 1004, passenger: 'Sarah Ahmed', route: 'Port Sudan → Atbara', date: '2024-12-23', status: 'Cancelled', amount: 'SDG 750' },
    { id: 1005, passenger: 'Omar Khalil', route: 'Atbara → Khartoum', date: '2024-12-24', status: 'Confirmed', amount: 'SDG 500' },
  ];

  return (
    <div>
      <div className="mb-8 flex items-center justify-between">
        <div>
          <h1 className="text-3xl font-bold text-gray-900">Bookings</h1>
          <p className="text-gray-600 mt-2">Manage train ticket bookings</p>
        </div>
        <button className="admin-button flex items-center gap-2">
          <Download size={20} />
          Export
        </button>
      </div>

      <div className="admin-card mb-6">
        <div className="flex gap-4">
          <div className="relative flex-1">
            <Search className="absolute left-3 top-1/2 transform -translate-y-1/2 text-gray-400" size={20} />
            <input
              type="text"
              placeholder="Search bookings..."
              value={searchTerm}
              onChange={(e) => setSearchTerm(e.target.value)}
              className="w-full pl-10 pr-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-admin-primary-500"
            />
          </div>
          <button className="admin-button-secondary flex items-center gap-2">
            <Filter size={20} />
            Filter
          </button>
        </div>
      </div>

      <div className="admin-card">
        <div className="overflow-x-auto">
          <table className="w-full">
            <thead className="bg-gray-50 border-b border-gray-200">
              <tr>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                  Booking ID
                </th>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                  Passenger
                </th>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                  Route
                </th>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                  Date
                </th>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                  Amount
                </th>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                  Status
                </th>
              </tr>
            </thead>
            <tbody className="bg-white divide-y divide-gray-200">
              {bookings.map((booking) => (
                <tr key={booking.id} className="hover:bg-gray-50">
                  <td className="px-6 py-4 whitespace-nowrap">
                    <div className="font-medium text-gray-900">#{booking.id}</div>
                  </td>
                  <td className="px-6 py-4 whitespace-nowrap text-gray-900">
                    {booking.passenger}
                  </td>
                  <td className="px-6 py-4 whitespace-nowrap text-gray-600">
                    {booking.route}
                  </td>
                  <td className="px-6 py-4 whitespace-nowrap text-gray-600">
                    {booking.date}
                  </td>
                  <td className="px-6 py-4 whitespace-nowrap font-medium text-gray-900">
                    {booking.amount}
                  </td>
                  <td className="px-6 py-4 whitespace-nowrap">
                    <span className={`px-3 py-1 text-xs rounded-full ${
                      booking.status === 'Confirmed' ? 'bg-green-100 text-green-800' :
                      booking.status === 'Pending' ? 'bg-yellow-100 text-yellow-800' :
                      'bg-red-100 text-red-800'
                    }`}>
                      {booking.status}
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

export default BookingsPage;
