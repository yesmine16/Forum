using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Forum.Models
{
    public class Post
    {
        [Required]
        public Guid Id { get; set; }

        [Required]
        public string Message { get; set; }

        [Required]
        public string Topic { get; set; }

        [Required]
        public DateTime DateCreationMessage { get; set; }

        [ForeignKey("ThemeId")]
        public Guid ThemeId { get; set; }

        [ForeignKey("UserId")]
        public Guid UserId { get; set; }

    }
}
