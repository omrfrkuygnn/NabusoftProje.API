namespace Event.Models
{
    public class Comment
    {
        public int CommentId { get; set; }
        public int EventId { get; set; }
        public string Text { get; set; }
        public string UserId { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? Reply { get; set; }
        public EventItem Event { get; set; }
    }

}