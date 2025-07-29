using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace LocationManagement.API.Models
{
    // 📌 Bu sınıf MySQL'deki "ilceler" tablosuna karşılık gelir
    [Table("ilceler")]
    public class District
    {
        // 🔑 Veritabanındaki 'id' sütunu (primary key)
        [Key]
        [Column("id")]
        public int DistrictId { get; set; }

        // 🏘️ Veritabanındaki 'name' sütunu
        [Required]
        [MaxLength(255)]
        [Column("name")]
        public string DistrictName { get; set; }

        // 🆔 Veritabanındaki 'kimlikNo' sütunu (opsiyonel, ihtiyaç varsa kullanılabilir)
        [Column("kimlikNo")]
        public int? IdentityNumber { get; set; }

        // 🏙️ Veritabanındaki 'il_id' sütunu (ili temsil eder, foreign key)
        [Required]
        [Column("il_id")]
        public int CityId { get; set; }

        // 🔁 Navigation property: İlçe → İl
        [ForeignKey("CityId")]
        public virtual City? City { get; set; }

        // 🔗 Navigation property: İlçe → Mahalleler
        public virtual ICollection<Neighborhood>? Neighborhoods { get; set; }
    }
}