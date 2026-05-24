using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;

namespace EcommerceMVC.Helpers
{
    public class VNMailService : IVNMailService
    {
        private readonly MailSettings _mailSettings;

        public VNMailService(IOptions<MailSettings> mailSettings)
        {
            _mailSettings = mailSettings.Value;
        }

        public async Task<int> SendMail(string toMail, string subject, string message)
        {
            try
            {
                using (SmtpClient smtpClient = new SmtpClient(_mailSettings.Host))
                {
                    smtpClient.EnableSsl = true;
                    smtpClient.UseDefaultCredentials = false;
                    smtpClient.Credentials = new NetworkCredential(_mailSettings.FromEmail, _mailSettings.Password);
                    smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                    smtpClient.Port = _mailSettings.Port;

                    using (MailMessage mail = new MailMessage())
                    {
                        mail.From = new MailAddress(_mailSettings.FromEmail, _mailSettings.DisplayName);
                        mail.To.Add(new MailAddress(toMail));
                        mail.IsBodyHtml = true;
                        mail.Subject = subject;
                        mail.Body = message;

                        await smtpClient.SendMailAsync(mail);
                    }
                }
            }
            catch (Exception ex)
            {
                // In production, you might want to log the exception
                return 0;
            }
            return 1;
        }
    }
}
