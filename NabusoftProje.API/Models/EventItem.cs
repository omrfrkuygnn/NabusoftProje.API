using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Event.Models
{
    public class EventItem
    {
        [Key]
        public int EventId {  get; set; }
        public int UserId { get; set; }

        [Required]
        [MaxLength(100)]
        public string EventTitle { get; set; }
        [Required]
        public string EventType { get; set; }
        [Required]
        public string EventCategory { get; set; }

        [Required]
        [MaxLength(500)]
        public string EventDescription { get; set; }
        public DateTime EventStartDate { get; set; }
        public DateTime EventEndDate { get; set; }
        [Required]
        [MaxLength(250)]
        public string EventLocation { get; set; }
        public string EventImg {  get; set; }
        public int EventQuota { get; set; }
        public float EventPrice { get; set; }
        public bool IsPublic { get; set; }
        public string Status { get; set; } = "Aktif";
        public DateTime? CancelledAt { get; set; }
        public DateTime? CreatedAt { get; set; } = DateTime.Now;


        //public ICollection<EventRule> EventRule { get; set; }
        [NotMapped]
        public List<int> RuleIds { get; set; }
        public bool IsDeleted { get; set; } = false;

        public string? CityName { get; set; }
        public string? DistrictName { get; set; }
        public string? NeighborhoodName { get; set; }
        public string? StreetName { get; set; }


    }
}
