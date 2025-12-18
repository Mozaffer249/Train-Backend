using Microsoft.EntityFrameworkCore;
using Sudan_Train.Data.Entity;
using Sudan_Train.Infrastructure.Abstracts;
using Sudan_Train.Infrastructure.context;
using Sudan_Train.Infrastructure.InfrastructureBases;

namespace Sudan_Train.Infrastructure.Repositories
{
    public class CoachRepository : GenericRepositoryAsync<Coach>, ICoachRepository
    {
        #region Fields
        private DbSet<Coach> coaches;
        #endregion

        #region Constructors
        public CoachRepository(ApplicationDBContext dbContext) : base(dbContext)
        {
            coaches = dbContext.Set<Coach>();
        }
        #endregion

        #region Handle Functions
        #endregion
    }
}

