using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Sudan_Train.Data.Commons;

namespace Sudan_Train.Data.Entity
{
    public class Promotion : AuditableEntity
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string Code { get; set; } = default!;

        [Required, MaxLength(200)]
        public string NameEn { get; set; } = default!;

        [MaxLength(200)]
        public string? NameAr { get; set; }

        [MaxLength(1000)]
        public string? DescriptionEn { get; set; }

        [MaxLength(1000)]
        public string? DescriptionAr { get; set; }

        public PromotionType Type { get; set; }

        public decimal DiscountValue { get; set; }
        public decimal? MaxDiscount { get; set; }
        public decimal? MinimumPurchase { get; set; }

        public int? MaxUsageCount { get; set; }
        public int UsageCount { get; set; } = 0;

        public DateTime ValidFrom { get; set; }
        public DateTime ValidTo { get; set; }

        public bool IsActive { get; set; } = true;

        public ICollection<PromotionUsage> PromotionUsages { get; set; } = new List<PromotionUsage>();
    }
}
