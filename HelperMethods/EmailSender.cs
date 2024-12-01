using System.Net;
using System.Net.Mail;

namespace Expense_App.HelperMethods
{
    public class EmailSender :IEmailSender
    {
        public Task SendEmailAsync(string email,string subject,string message)
        {
            var mail = "specificprojectmail@gmail.com";
            var pw = "bmtf akpv vjxc vevb";
            var client = new SmtpClient("smtp.gmail.com", 587)
            {
                EnableSsl = true,
                Credentials = new NetworkCredential(mail,pw)

            };
            return client.SendMailAsync(new MailMessage(from: mail, to: email, subject, message));
        } 
    }
}
