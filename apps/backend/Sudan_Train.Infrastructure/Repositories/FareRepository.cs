using Microsoft.EntityFrameworkCore;
using Sudan_Train.Data.Entity;
using Sudan_Train.Infrastructure.Abstracts;
using Sudan_Train.Infrastructure.context;
using Sudan_Train.Infrastructure.InfrastructureBases;

namespace Sudan_Train.Infrastructure.Repositories
{
    public class FareRepository : GenericRepositoryAsync<Fare>, IFareRepository
    {
        private DbSet<Fare> fares;

        public FareRepository(ApplicationDBContext dbContext) : base(dbContext)
        {
            fares = dbContext.Set<Fare>();
        }
    }
}
