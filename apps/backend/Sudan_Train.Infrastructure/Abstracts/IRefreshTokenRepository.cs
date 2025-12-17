using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sudan_Train.Data.Entity.Identity;
using Sudan_Train.Infrastructure.InfrastructureBases;

namespace Sudan_Train.Infrastructure.Abstracts
{
    public interface IRefreshTokenRepository : IGenericRepositoryAsync<UserRefreshToken>
    {

    }
}