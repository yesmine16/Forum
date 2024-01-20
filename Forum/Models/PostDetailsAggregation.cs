using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace Forum.Models
{
    public class PostDetailsAggregation
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }

        public Guid ThemeId { get; set; }

        public string Message { get; set; }

        public DateTime DateCreationMessage { get; set; }

        public string UserName { get; set; }

        public string Theme { get; set; }

        public string Topic { get; set; }

        public List<Messages> Messages { get; set; }
        
        public Guid PostId { get; set; }

        public string CheminAvatar { get; set; }

    }
}
