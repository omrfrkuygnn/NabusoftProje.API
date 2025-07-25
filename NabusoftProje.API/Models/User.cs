using System;
using System.Collections.Generic;

namespace NabusoftProje.API.Models
{
    public class User
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; }
        public string PasswordHash { get; set; } = string.Empty;
        public DateTime BirthDate { get; set; }
        public bool IsGoogleAccount { get; set; }
        public string Role { get; set; } = "User"; // "Admin" veya "User"
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? LastLoginAt { get; set; }
        public bool IsSubscribed { get; set; }
        public int Tokens { get; set; } = 0;
        public bool IsEmailVerified { get; set; } = false;
        public string? PhotoPath { get; set; }

        // Navigation
        public List<Event> CreatedEvents { get; set; }
        public List<EventParticipant> ParticipatedEvents { get; set; }
        public ICollection<UserSubscription>? Subscriptions { get; set; }
        public ICollection<TokenTransaction> TokenTransactions { get; set; } = new List<TokenTransaction>();
        public ICollection<UserHobbies> UserHobbies { get; set; } = new List<UserHobbies>();
        public ICollection<FavoriteEvent> FavoriteEvents { get; set; }
    }
} 