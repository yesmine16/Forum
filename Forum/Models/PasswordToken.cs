using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Forum.Models
{
    public class PasswordToken
    {
        [Key]
        public Guid Token { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public DateTime ValidationInterval { get; set; }
    }
}
