using System;

namespace NabusoftProje.API.Models
{
    public class TokenPackage
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty; // Örn: "500 Jeton", "1000 Jeton"
        public string Description { get; set; } = string.Empty;
        public int TokenAmount { get; set; }
        public decimal Price { get; set; }
        public bool IsActive { get; set; } = true;
        public int SortOrder { get; set; }
        public bool IsPopular { get; set; } = false; // Popüler paket işareti
        public string? BonusText { get; set; } // Örn: "%10 Bonus", "En Popüler"
    }
} 