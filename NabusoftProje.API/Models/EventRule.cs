namespace Event.Models
{
    public class EventRule
    {
        public int EventId { get; set; }
        public int RuleId { get; set; }
        public int GivenBy { get; set; }
        public DateTime GivenAt { get; set; }
        public EventItem Event { get; set; }
        public Rule Rule { get; set; }
    }
}
