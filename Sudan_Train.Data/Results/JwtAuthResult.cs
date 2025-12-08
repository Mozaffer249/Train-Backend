using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sudan_Train.Data.Results
{
    public class JwtAuthResult
    {
        public string AccessToken { get; set; } = default!;
        public RefreshToken RefreshToken { get; set; } = default!;
    }
    public class RefreshToken
    {
        public string UserName { get; set; } = default!;
        public string TokenString { get; set; } = default!;
        public DateTime ExpireAt { get; set; }
    }
}