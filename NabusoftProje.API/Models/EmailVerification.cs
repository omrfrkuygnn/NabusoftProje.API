using System;

namespace NabusoftProje.API.Models
{
    public class EmailVerification
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Token { get; set; }
        public DateTime Expiration { get; set; }
        public bool IsUsed { get; set; } = false;
        public User User { get; set; }
    }
} 