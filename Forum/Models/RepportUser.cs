namespace Forum.Models
{
    public class RepportUser
    {
        public Guid Id { get; set; }
        public Guid IssuerUserId { get; set; }
        public Guid ReportedId { get; set; }
    }
}
