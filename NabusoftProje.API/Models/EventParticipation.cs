namespace EtkinlikKatilimApi.Models
{
    public class EventParticipation
    {
        public int Id { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }

        public int EventId { get; set; }
        public Event Event { get; set; }

        public DateTime RequestDate { get; set; }
        public bool? IsApproved { get; set; } // null: beklemede, true: onaylı, false: reddedildi
        public string? RejectReason { get; set; }
    }
}
