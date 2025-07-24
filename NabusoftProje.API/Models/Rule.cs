namespace Event.Models
{
    public class Rule
    {
        public int RuleID { get; set; }
        public string RuleDescription { get; set; }
        
        public ICollection<EventRule> EventRule { get; set; }
    }
}
