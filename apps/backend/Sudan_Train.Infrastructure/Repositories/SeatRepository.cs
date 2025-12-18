using Microsoft.EntityFrameworkCore;
using Sudan_Train.Data.Entity;
using Sudan_Train.Infrastructure.Abstracts;
using Sudan_Train.Infrastructure.context;
using Sudan_Train.Infrastructure.InfrastructureBases;

namespace Sudan_Train.Infrastructure.Repositories
{
    public class SeatRepository : GenericRepositoryAsync<Seat>, ISeatRepository
    {
        #region Fields
        private DbSet<Seat> seats;
        #endregion

        #region Constructors
        public SeatRepository(ApplicationDBContext dbContext) : base(dbContext)
        {
            seats = dbContext.Set<Seat>();
        }
        #endregion

        #region Handle Functions
        #endregion
    }
}

