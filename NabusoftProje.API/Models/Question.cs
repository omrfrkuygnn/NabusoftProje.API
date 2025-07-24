namespace Event.Models
{
    public class Question
    {
        public int QuestionId { get; set; }
        public int EventId { get; set; }
        public string Text { get; set; }
        public string UserId { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? Answer { get; set; }

        public EventItem Event { get; set; }
    }

}