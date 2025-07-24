using System.ComponentModel.DataAnnotations;

namespace Event.Models
{
    public class EventCategory
    {
        public int EventCategoryId { get; set; }

        [Required]
        public string Name { get; set; } = null!;

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public DateTime? UpdatedDate { get; set; }
    }
}
