using Microsoft.EntityFrameworkCore;
using Sudan_Train.Data.Entity;
using Sudan_Train.Infrastructure.Abstracts;
using Sudan_Train.Infrastructure.context;
using Sudan_Train.Infrastructure.InfrastructureBases;

namespace Sudan_Train.Infrastructure.Repositories
{
    public class TripSeatRepository : GenericRepositoryAsync<TripSeat>, ITripSeatRepository
    {
        #region Fields
        private DbSet<TripSeat> tripSeats;
        #endregion

        #region Constructors
        public TripSeatRepository(ApplicationDBContext dbContext) : base(dbContext)
        {
            tripSeats = dbContext.Set<TripSeat>();
        }
        #endregion

        #region Handle Functions
        #endregion
    }
}

