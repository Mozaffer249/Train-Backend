using Microsoft.EntityFrameworkCore;
using Sudan_Train.Data.Entity;
using Sudan_Train.Infrastructure.Abstracts;
using Sudan_Train.Infrastructure.context;
using Sudan_Train.Infrastructure.InfrastructureBases;

namespace Sudan_Train.Infrastructure.Repositories
{
    public class TrainRepository : GenericRepositoryAsync<Train>, ITrainRepository
    {
        #region Fields
        private DbSet<Train> trains;
        #endregion

        #region Constructors
        public TrainRepository(ApplicationDBContext dbContext) : base(dbContext)
        {
            trains = dbContext.Set<Train>();
        }
        #endregion

        #region Handle Functions
        #endregion
    }
}

