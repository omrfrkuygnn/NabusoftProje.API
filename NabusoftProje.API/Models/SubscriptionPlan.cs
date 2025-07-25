using System;
using System.Collections.Generic;

namespace NabusoftProje.API.Models
{
    public class SubscriptionPlan
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int DurationInDays { get; set; }
        public int MaxDailyJoins { get; set; }
        public int MaxEventsPerMonth { get; set; }
        public int MaxParticipantsPerEvent { get; set; }
        public bool CanCreateEvents { get; set; }
        public bool HasPrioritySupport { get; set; }
        public bool HasAdvancedAnalytics { get; set; }
        public bool IsActive { get; set; } = true;
        public int SortOrder { get; set; }

        // Navigation
        public ICollection<UserSubscription>? UserSubscriptions { get; set; }
    }
} 