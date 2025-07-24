namespace Event.Models
{
    public class EventRequest
    {
        public int Id { get; set; }
        public int EventId { get; set; }
        public int UserId { get; set; }
        public DateTime RequestedDate { get; set; } = DateTime.Now;
        public bool IsApproved { get; set; } = false;
        public string RejectReason { get; set; }
    }
}
