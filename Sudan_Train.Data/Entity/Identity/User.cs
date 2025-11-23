using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using EntityFrameworkCore.EncryptColumn.Attribute;
using Microsoft.AspNetCore.Identity;

namespace Sudan_Train.Data.Entity.Identity
{
    public class User : IdentityUser<int>
    {
        public User()
        {
            UserRefreshTokens = new HashSet<UserRefreshToken>();
            Bookings = new HashSet<Booking>();
        }

        public string FirstName { get; set; } = default!;
        public string LastName { get; set; } = default!;
        public string? Address { get; set; }
        public string? Nationality { get; set; }

        [EncryptColumn]
        public string? Code { get; set; }

        public bool IsActive { get; set; } = true;

        [InverseProperty(nameof(UserRefreshToken.User))]
        public ICollection<UserRefreshToken> UserRefreshTokens { get; set; }

        public ICollection<Booking> Bookings { get; set; }
    }
}