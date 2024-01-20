using System.ComponentModel.DataAnnotations.Schema;

namespace Forum.Models
{
    public class Messages
    {
        public Guid Id { get; set; }

        public string Content { get; set; }

        public Guid userId { get; set; }

        [ForeignKey("PostId")]
        public Guid PostId { get; set; }

        public DateTime TimeStamp { get; set; }

        public string Issuer { get; set; }

        
    }
}
