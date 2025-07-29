using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace LocationManagement.API.Models
{
    // 📌 Bu sınıf MySQL'deki "mahalleler" tablosuna karşılık gelir
    [Table("mahalleler")]
    public class Neighborhood
    {
        // 🔑 Veritabanındaki 'id' sütunu
        [Key]
        [Column("id")]
        public int NeighborhoodId { get; set; }

        // 📛 Veritabanındaki 'bilesenName' sütunu
        [Required(ErrorMessage = "Mahalle adı gereklidir.")]
        [MaxLength(255, ErrorMessage = "Mahalle adı en fazla 255 karakter olabilir.")]
        [Column("bilesenName")]
        public string NeighborhoodName { get; set; }

        // 🆔 Veritabanındaki 'kimlikNo' sütunu (nullable olabilir)
        [Column("kimlikNo")]
        public int? IdentityNumber { get; set; }

        // 🔗 Veritabanındaki 'ilce_id' sütunu (foreign key)
        [Required]
        [Column("ilce_id")]
        public int DistrictId { get; set; }

        // 🔁 Navigation: Mahalle → İlçe
        [ForeignKey("DistrictId")]
        [JsonIgnore]
        public virtual District? District { get; set; }

        // 🔁 Navigation: Mahalle → Semt/Köy (Quarter)
        [JsonIgnore]
        public virtual ICollection<Quarter>? Quarters { get; set; }
    }
}