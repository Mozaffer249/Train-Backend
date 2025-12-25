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

        [Required]
        public double Latitude { get; set; }

        [Required]
        public double Longitude { get; set; }

        public string? AddressEn { get; set; }
        public string? AddressAr { get; set; }

        // Google Integration Fields
        [MaxLength(255)]
        public string? GooglePlaceId { get; set; }

        [MaxLength(500)]
        public string? FormattedAddress { get; set; }

        [MaxLength(50)]
        public string? PlusCode { get; set; }

        [MaxLength(100)]
        public string? GoogleType { get; set; }

        public DateTime? GoogleSyncedAt { get; set; }
        public bool IsFromGoogle { get; set; } = false;

        [MaxLength(50)]
        public string? BusinessStatus { get; set; }

        // Spatial Fields
        public double? ServiceRadiusKm { get; set; }

        [MaxLength(50)]
        public string? StationType { get; set; } // "train_station", "bus_station", etc.

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<RouteStation> RouteStations { get; set; } = new List<RouteStation>();
    }
}

