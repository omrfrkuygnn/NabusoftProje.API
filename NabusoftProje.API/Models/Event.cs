namespace EtkinlikKatilimApi.Models
{
    public class Event
    {
        public int EventId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime Date { get; set; }
        public int Capacity { get; set; }

        public string City { get; set; } // Şehir bilgisi (arama/filtre için)
        public string Category { get; set; } // Kategori bilgisi (arama/filtre için)
        public bool IsActive { get; set; } // Etkinlik yayında mı?
        public string Interest { get; set; } // İlgi alanı (arama/filtre için)

        public int OrganizerId { get; set; }
        public User Organizer { get; set; }
        public ICollection<Favorite> Favorites { get; set; }
        public ICollection<EventParticipation> Participations { get; set; }
    }
}