using System.Net;
using System.Net.Mail;
using NuGet.Protocol;

namespace Forum.Tools
{

    public class MailService: IMailService
    {
        private string FromEmail;
        private string FromPassword;
        private readonly MailMessage _email;
        private readonly IConfiguration _configuration;
        public MailService(MailMessage email, IConfiguration configuration)
        {
            _configuration = configuration;
            _email = email;
        }


        public async Task SendEmail(string email, string emailBody, string subject, bool isHtml)
        {
            try
            {
                FromEmail = _configuration.GetValue<string>("EmailSettings:email");
                FromPassword = _configuration.GetValue<string>("EmailSettings:password");

                _email.From = new MailAddress(FromEmail);
                _email.Subject = subject;
                _email.To.Add(new MailAddress(email));
                _email.Body = emailBody;
                _email.IsBodyHtml = isHtml;
                var smptClient = new SmtpClient("smtp.gmail.com")
                {
                    Port = 587,
                    Credentials = new NetworkCredential(FromEmail, FromPassword),
                    EnableSsl = true
                };
                await smptClient.SendMailAsync(_email);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"the email to {email} was not sent cause {ex.Message} ");
                throw;
            }

        }
    }
}
