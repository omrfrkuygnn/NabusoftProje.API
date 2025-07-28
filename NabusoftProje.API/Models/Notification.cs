using EtkinlikKatilimApi.Models;
public class Notification
{
    public int Id { get; set; }

    public int? EventId { get; set; }
    public Event Event { get; set; }

    public int UserId { get; set; }
    public User User { get; set; }

    public string Message { get; set; }
    public DateTime CreatedAt { get; set; }

    public string? Response { get; set; }
    public DateTime? ResponseDate { get; set; }

    public bool IsRead { get; set; } = false;
}

