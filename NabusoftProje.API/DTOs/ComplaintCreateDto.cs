namespace EtkinlikKatilimApi.Models
{
    public class ComplaintCreateDto
    {
        public int EventId { get; set; }
        public string Message { get; set; }
        public string Reason { get; set; }
    }
}