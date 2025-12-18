using Microsoft.EntityFrameworkCore;
using Sudan_Train.Data.Entity;
using Sudan_Train.Infrastructure.Abstracts;
using Sudan_Train.Infrastructure.context;
using Sudan_Train.Infrastructure.InfrastructureBases;

namespace Sudan_Train.Infrastructure.Repositories
{
    public class RouteStationRepository : GenericRepositoryAsync<RouteStation>, IRouteStationRepository
    {
        #region Fields
        private DbSet<RouteStation> routeStations;
        #endregion

        #region Constructors
        public RouteStationRepository(ApplicationDBContext dbContext) : base(dbContext)
        {
            routeStations = dbContext.Set<RouteStation>();
        }
        #endregion

        #region Handle Functions
        #endregion
    }
}

