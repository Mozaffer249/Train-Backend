namespace Sudan_Train.Service.Abstracts
{
    public interface IDistanceCalculationService
    {
        /// <summary>
        /// Calculate distance in kilometers between two coordinates using Haversine formula
        /// </summary>
        double CalculateDistance(double lat1, double lon1, double lat2, double lon2);

        /// <summary>
        /// Calculate total distance for a route with multiple stations
        /// </summary>
        Task<decimal> CalculateRouteDistanceAsync(int originStationId, int destinationStationId, List<int> intermediateStationIds);
    }
}
