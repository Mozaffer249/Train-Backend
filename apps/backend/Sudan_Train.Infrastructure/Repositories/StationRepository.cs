using Microsoft.EntityFrameworkCore;
using Sudan_Train.Data.Entity;
using Sudan_Train.Infrastructure.Abstracts;
using Sudan_Train.Infrastructure.context;
using Sudan_Train.Infrastructure.InfrastructureBases;

namespace Sudan_Train.Infrastructure.Repositories
{
    public class StationRepository : GenericRepositoryAsync<Station>, IStationRepository
    {
        #region Fields
        private DbSet<Station> stations;
        #endregion

        #region Constructors
        public StationRepository(ApplicationDBContext dbContext) : base(dbContext)
        {
            stations = dbContext.Set<Station>();
        }
        #endregion

        #region Handle Functions
        public async Task<List<Station>> GetAllAsync()
        {
            return await stations.ToListAsync();
        }
        #endregion
    }
}

