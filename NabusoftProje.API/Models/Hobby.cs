using System.Collections.Generic;

namespace NabusoftProje.API.Models
{
    public class Hobby
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public ICollection<UserHobbies> UserHobbies { get; set; }
        public ICollection<EventHobby> EventHobbies { get; set; } = new List<EventHobby>();
    }
} 