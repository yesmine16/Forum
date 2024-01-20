using System.ComponentModel.DataAnnotations;

namespace Forum.Models
{
    public class Theme
    {
        [Required]
        public Guid id { get; set; }
        [Required]
        public string titre { get; set; } = string.Empty;
    }
}
