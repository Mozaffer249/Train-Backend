using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sudan_Train.Infrastructure.Abstracts
{
    public interface IRefreshTokenRepository : IGenericRepositoryAsync<UserRefreshToken>
    {

    }
}