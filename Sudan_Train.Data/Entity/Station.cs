using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sudan_Train.Data.Entity
{
    public class Station
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(20)]
        public string Code { get; set; } = default!;

        [Required, MaxLength(200)]
        public string NameEn { get; set; } = default!;

        [Required, MaxLength(200)]
        public string NameAr { get; set; } = default!;

        public int CityId { get; set; }

        [ForeignKey(nameof(CityId))]
        public City City { get; set; } = default!;

        public double? Latitude { get; set; }
        public double? Longitude { get; set; }

        public string? AddressEn { get; set; }
        public string? AddressAr { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<RouteStation> RouteStations { get; set; } = new List<RouteStation>();
    }
}

