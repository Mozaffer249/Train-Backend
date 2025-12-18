using Microsoft.EntityFrameworkCore;
using Sudan_Train.Data.Entity;
using Sudan_Train.Infrastructure.Abstracts;
using Sudan_Train.Infrastructure.context;
using Sudan_Train.Infrastructure.InfrastructureBases;

namespace Sudan_Train.Infrastructure.Repositories
{
    public class RegionRepository : GenericRepositoryAsync<Region>, IRegionRepository
    {
        #region Fields
        private DbSet<Region> regions;
        #endregion

        #region Constructors
        public RegionRepository(ApplicationDBContext dbContext) : base(dbContext)
        {
            regions = dbContext.Set<Region>();
        }
        #endregion

        #region Handle Functions
        #endregion
    }
}

