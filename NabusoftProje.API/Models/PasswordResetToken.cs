using System;

namespace NabusoftProje.API.Models
{
    public class PasswordResetToken
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }  // Navigation property

        public string Token { get; set; }
        public DateTime Expiration { get; set; }
        public bool IsUsed { get; set; }
    }
} 