using Microsoft.EntityFrameworkCore;
using Sudan_Train.Data.Entity;
using Sudan_Train.Infrastructure.Abstracts;
using Sudan_Train.Infrastructure.context;
using Sudan_Train.Infrastructure.InfrastructureBases;

namespace Sudan_Train.Infrastructure.Repositories
{
    public class RouteRepository : GenericRepositoryAsync<Route>, IRouteRepository
    {
        #region Fields
        private DbSet<Route> routes;
        #endregion

        #region Constructors
        public RouteRepository(ApplicationDBContext dbContext) : base(dbContext)
        {
            routes = dbContext.Set<Route>();
        }
        #endregion

        #region Handle Functions
        #endregion
    }
}

