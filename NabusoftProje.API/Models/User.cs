namespace EtkinlikKatilimApi.Models
{
    public class User
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }  // Test için açık metin, sonra hashli olacak
        public string Role { get; set; }      // "User" veya "Organizer"

        public ICollection<Event> CreatedEvents { get; set; }
        public ICollection<Favorite> Favorites { get; set; }

        public ICollection<EventParticipation> Participations { get; set; }
    }
}
