using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sudan_Train.Data.Entity
{
    public class City
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string NameEn { get; set; } = default!;

        [Required, MaxLength(100)]
        public string NameAr { get; set; } = default!;

        // Google Integration Fields
        [MaxLength(255)]
        public string? GooglePlaceId { get; set; }

        [Required]
        public double Latitude { get; set; }

        [Required]
        public double Longitude { get; set; }

        [MaxLength(500)]
        public string? FormattedAddress { get; set; }

        [MaxLength(50)]
        public string? PlusCode { get; set; }

        public DateTime? GoogleSyncedAt { get; set; }
        public bool IsFromGoogle { get; set; } = false;

        // Boundary and Spatial Fields
        public string? BoundaryPolygon { get; set; } // GeoJSON polygon
        public double? BoundingBoxNorth { get; set; }
        public double? BoundingBoxSouth { get; set; }
        public double? BoundingBoxEast { get; set; }
        public double? BoundingBoxWest { get; set; }

        public ICollection<Station> Stations { get; set; } = new List<Station>();
    }
}

