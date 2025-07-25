using System;

namespace NabusoftProje.API.DTOs
{
    public class UpdateProfileDto
    {
        public string? FullName { get; set; }
        public string? PhotoPath { get; set; }
        public DateTime? BirthDate { get; set; }
    }
} 