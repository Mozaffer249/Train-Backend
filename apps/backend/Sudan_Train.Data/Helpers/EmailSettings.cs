using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sudan_Train.Data.Helpers
{
    public class EmailSettings
    {
        public int Port { get; set; }
        public string Host { get; set; } = default!;
        public string FromEmail { get; set; } = default!;
        public string Password { get; set; } = default!;
    }
}