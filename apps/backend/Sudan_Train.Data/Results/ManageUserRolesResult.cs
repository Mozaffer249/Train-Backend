using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sudan_Train.Data.Results
{
    public class ManageUserRolesResult
    {
        public int UserId { get; set; }
        public List<UserRoles> userRoles { get; set; } = default!;
    }
    public class UserRoles
    {
        public int Id { get; set; }
        public string Name { get; set; } = default!;
        public bool HasRole { get; set; }
    }
}