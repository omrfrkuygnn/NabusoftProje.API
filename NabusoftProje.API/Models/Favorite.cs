// Models/Favorite.cs
using System;

namespace EtkinlikKatilimApi.Models
{
    public class Favorite
    {
        public int Id { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }

        public int EventId { get; set; }
        public Event Event { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
