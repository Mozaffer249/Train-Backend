using Microsoft.EntityFrameworkCore;
using Sudan_Train.Infrastructure.Abstracts;
using Sudan_Train.Service.Abstracts;

namespace Sudan_Train.Service.Implementations
{
    public class DistanceCalculationService : IDistanceCalculationService
    {
        private readonly IStationRepository _stationRepository;

        public DistanceCalculationService(IStationRepository stationRepository)
        {
            _stationRepository = stationRepository;
        }

        /// <summary>
        /// Calculate distance in kilometers between two coordinates using Haversine formula
        /// </summary>
        public double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
        {
            const double R = 6371; // Earth's radius in kilometers

            var dLat = ToRadians(lat2 - lat1);
            var dLon = ToRadians(lon2 - lon1);

            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                    Math.Cos(ToRadians(lat1)) * Math.Cos(ToRadians(lat2)) *
                    Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            var distance = R * c;

            return distance;
        }

        /// <summary>
        /// Calculate total distance for a route with multiple stations
        /// </summary>
        public async Task<decimal> CalculateRouteDistanceAsync(int originStationId, int destinationStationId, List<int> intermediateStationIds)
        {
            // Build list of all station IDs in order
            var allStationIds = new List<int> { originStationId };
            allStationIds.AddRange(intermediateStationIds);
            allStationIds.Add(destinationStationId);

            // Fetch all stations
            var stations = await _stationRepository.GetTableNoTracking()
                .Where(s => allStationIds.Contains(s.Id))
                .ToListAsync();

            // Calculate cumulative distance
            double totalDistance = 0;
            for (int i = 0; i < allStationIds.Count - 1; i++)
            {
                var fromStation = stations.FirstOrDefault(s => s.Id == allStationIds[i]);
                var toStation = stations.FirstOrDefault(s => s.Id == allStationIds[i + 1]);

                if (fromStation != null && toStation != null)
                {
                    totalDistance += CalculateDistance(
                        fromStation.Latitude, fromStation.Longitude,
                        toStation.Latitude, toStation.Longitude);
                }
            }

            return (decimal)Math.Round(totalDistance, 2);
        }

        private double ToRadians(double degrees)
        {
            return degrees * Math.PI / 180;
        }
    }
}
