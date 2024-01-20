using System.ComponentModel.DataAnnotations;

namespace Forum.Models
{
    public class Likes
    {
        [Required]
        public Guid UserId { get; set; }
        [Required]
        public Guid Id { get; set; }
        [Required]
        public Guid PostId { get; set; }


    }
}
