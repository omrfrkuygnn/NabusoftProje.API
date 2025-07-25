using Microsoft.EntityFrameworkCore;
using NabusoftProje.API.Models;

namespace NabusoftProje.API
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<UserHobbies> UserHobbies { get; set; }
        public DbSet<UserSubscription> UserSubscriptions { get; set; }
        public DbSet<PasswordResetToken> PasswordResetTokens { get; set; }
        public DbSet<SubscriptionPlan> SubscriptionPlans { get; set; }
        public DbSet<TokenPackage> TokenPackages { get; set; }
        public DbSet<Hobby> Hobbies { get; set; }
        public DbSet<EmailVerification> EmailVerifications { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<EventParticipant> EventParticipants { get; set; }
        public DbSet<TokenTransaction> TokenTransactions { get; set; }
        public DbSet<FavoriteEvent> FavoriteEvents { get; set; }
        public DbSet<EventHobby> EventHobbies { get; set; }
        public DbSet<Complaint> Complaints { get; set; }
        public DbSet<ParticipationRequest> ParticipationRequests { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // İlişki ve konfigürasyonlar buraya eklenebilir
        }
    }
} 