using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sudan_Train.Data.Helpers
{
    public class UserClaimModel
    {
        public int Id { get; set; }
        public string UserName { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string PhoneNumber { get; set; } = default!;
        public string Role { get; set; } = default!;
    }

}