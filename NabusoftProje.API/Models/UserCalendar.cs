using System;

namespace NabusoftProje.API.Models
{
    public class UserCalendar
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int EventId { get; set; }
        public string CalendarType { get; set; } // Google, Outlook, Apple
        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }
} 