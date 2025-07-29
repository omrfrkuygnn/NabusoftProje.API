using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace LocationManagement.API.Models
{
    // 📌 Bu sınıf MySQL'deki "csbms" tablosuna karşılık gelir
    [Table("csbms")]
    public class Quarter
    {
        // 🔑 Veritabanındaki 'id' sütunu
        [Key]
        [Column("id")]
        public int QuarterId { get; set; }

        // 🏘️ Veritabanındaki 'bilesenName' sütunu (Semt/Köy adı)
        [Required(ErrorMessage = "Semt/Köy adı gereklidir.")]
        [MaxLength(255, ErrorMessage = "Semt/Köy adı en fazla 255 karakter olabilir.")]
        [Column("bilesenName")]
        public string QuarterName { get; set; } = string.Empty;

        // 🔗 'mahalle_id' sütununa karşılık gelir (foreign key)
        [Required]
        [Column("mahalle_id")]
        public int NeighborhoodId { get; set; }

        // 🔁 Navigation: Semt/Köy → Mahalle ilişkisi
        [JsonIgnore] // Sonsuz döngü olmaması için
        [ForeignKey("NeighborhoodId")]
        public virtual Neighborhood? Neighborhood { get; set; }
    }
}