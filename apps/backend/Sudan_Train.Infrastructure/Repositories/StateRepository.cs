using Microsoft.EntityFrameworkCore;
using Sudan_Train.Data.Entity;
using Sudan_Train.Infrastructure.Abstracts;
using Sudan_Train.Infrastructure.context;
using Sudan_Train.Infrastructure.InfrastructureBases;

namespace Sudan_Train.Infrastructure.Repositories
{
    public class StateRepository : GenericRepositoryAsync<State>, IStateRepository
    {
        #region Fields
        private DbSet<State> states;
        #endregion

        #region Constructors
        public StateRepository(ApplicationDBContext dbContext) : base(dbContext)
        {
            states = dbContext.Set<State>();
        }
        #endregion

        #region Handle Functions
        #endregion
    }
}

