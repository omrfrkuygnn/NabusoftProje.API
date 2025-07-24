using System.ComponentModel.DataAnnotations;

namespace Event.Models
{
    public class CommentLike
    {
        [Key]
        public int CommentLikeId { get; set; }

        public int CommentId { get; set; }
        public string UserId { get; set; }
        public DateTime LikedAt { get; set; }
    }

}