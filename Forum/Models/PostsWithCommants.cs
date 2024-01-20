using Microsoft.Build.Framework;

namespace Forum.Models
{
    public class PostsWithCommants
    {
        [Required]
        public PostDetailsAggregation Post { get; set; }
        public List<Messages> Comments { get; set; }
    }
}
