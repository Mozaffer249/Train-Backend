using Microsoft.EntityFrameworkCore;
using Sudan_Train.Data.Entity;
using Sudan_Train.Infrastructure.Abstracts;
using Sudan_Train.Infrastructure.context;
using Sudan_Train.Infrastructure.InfrastructureBases;

namespace Sudan_Train.Infrastructure.Repositories
{
    public class TripRepository : GenericRepositoryAsync<Trip>, ITripRepository
    {
        #region Fields
        private DbSet<Trip> trips;
        #endregion

        #region Constructors
        public TripRepository(ApplicationDBContext dbContext) : base(dbContext)
        {
            trips = dbContext.Set<Trip>();
        }
        #endregion

        #region Handle Functions
        #endregion
    }
}

