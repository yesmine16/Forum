namespace Forum.Tools
{
    public interface IMailService
    {
        public Task SendEmail(string email, string emailBody, string subject, bool isHtml);
    }
}
