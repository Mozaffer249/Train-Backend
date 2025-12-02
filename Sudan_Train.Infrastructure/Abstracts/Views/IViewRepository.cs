using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sudan_Train.Infrastructure.InfrastructureBases;

namespace Sudan_Train.Infrastructure.Abstracts.Views
{
    public interface IViewRepository<T> : IGenericRepositoryAsync<T> where T : class
    {
        
    }
}