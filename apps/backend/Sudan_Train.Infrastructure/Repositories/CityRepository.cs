using Microsoft.EntityFrameworkCore;
using Sudan_Train.Data.Entity;
using Sudan_Train.Infrastructure.Abstracts;
using Sudan_Train.Infrastructure.context;
using Sudan_Train.Infrastructure.InfrastructureBases;

namespace Sudan_Train.Infrastructure.Repositories
{
    public class CityRepository : GenericRepositoryAsync<City>, ICityRepository
    {
        #region Fields
        private DbSet<City> cities;
        #endregion

        #region Constructors
        public CityRepository(ApplicationDBContext dbContext) : base(dbContext)
        {
            cities = dbContext.Set<City>();
        }
        #endregion

        #region Handle Functions
        #endregion
    }
}