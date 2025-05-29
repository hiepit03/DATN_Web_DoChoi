using System.Net.Mail;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using MimeKit.Text;
using System.Threading.Tasks;

namespace Web_DoChoi.Services
{
    public class EmailSender
    {
        public async Task SendEmailAsync(string to, string subject, string htmlBody)
        {
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse("dieckososungchien@gmail.com"));
            email.To.Add(MailboxAddress.Parse(to));
            email.Subject = subject;
            email.Body = new TextPart(TextFormat.Html) { Text = htmlBody };

            // Chỉ định rõ MailKit.Net.Smtp.SmtpClient
            using var smtp = new MailKit.Net.Smtp.SmtpClient();
            await smtp.ConnectAsync("smtp.gmail.com", 587, MailKit.Security.SecureSocketOptions.StartTls);
            await smtp.AuthenticateAsync("dieckososungchien@gmail.com", "qnva qnwl feqv ltxh");
            await smtp.SendAsync(email);
            await smtp.DisconnectAsync(true);
        }
    }
}
