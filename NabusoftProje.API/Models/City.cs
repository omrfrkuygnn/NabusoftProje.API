using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LocationManagement.API.Models
{
    // 📌 Bu sınıf MySQL'deki "iller" tablosuna karşılık gelir
    [Table("iller")]
    public class City
    {
        // 🔑 Veritabanındaki 'id' sütunu (primary key)
        [Key]
        [Column("id")]
        public int CityId { get; set; }

        // 🏙️ Veritabanındaki 'name' sütunu (zorunlu, max 255 karakter)
        [Required]
        [MaxLength(255)]
        [Column("name")]
        public string CityName { get; set; }

        // 🚗 Veritabanındaki 'plaka' sütunu (plaka kodu)
        [Column("plaka")]
        public int PlateCode { get; set; }

        // 🔗 Navigation property: Bir ilin birden fazla ilçesi olabilir
        public virtual ICollection<District>? Districts { get; set; }
    }
}