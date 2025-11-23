using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sudan_Train.Data.Helpers
{
    public static class ClaimsStore
    {
        public static List<Claim> claims = new()
        {
            new Claim("Create Booking","false"),
            new Claim("Edit Booking","false"),
            new Claim("Delete Booking","false"),
            new Claim("Create Passenger","false"),
            new Claim("Edit Passenger","false"),
            new Claim("Delete Passenger","false"),
            new Claim("Create Payment","false"),
            new Claim("Edit Payment","false"),
            new Claim("Delete Payment","false"),
            new Claim("Create Ticket","false"),
            new Claim("Edit Ticket","false"),
            new Claim("Delete Ticket","false"),

        };
    }
}