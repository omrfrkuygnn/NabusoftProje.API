namespace EtkinlikKatilimApi.Models
{
    public class Complaint
    {
        public int Id { get; set; }

        public int EventId { get; set; }
        public Event Event { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }

        public string Message { get; set; }
        public string Reason { get; set; } // Şikayet sebebi

        public string Status { get; set; } = "Bekliyor"; // veya enum

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
