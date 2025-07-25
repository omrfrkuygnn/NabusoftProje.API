namespace EtkinlikKatilimApi.ViewModels
{
    public class EventViewModel
    {
        public int EventId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime Date { get; set; }
        public string City { get; set; }
        public string Category { get; set; }
        public bool IsActive { get; set; }
        public bool IsParticipated { get; set; }
        public bool IsExpired { get; set; }
        public int Capacity { get; set; }
        public int ApprovedParticipants { get; set; }
        public bool IsFull { get; set; }
    }
}
