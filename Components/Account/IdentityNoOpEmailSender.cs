using LicentaBalteanu.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using System.Net.Mail;
using System.Net;

namespace LicentaBalteanu.Components.Account
{
    // Remove the "else if (EmailSender is IdentityNoOpEmailSender)" block from RegisterConfirmation.razor after updating with a real implementation.
    internal sealed class IdentityNoOpEmailSender : IEmailSender<ApplicationUser>
    {
        private const string FromEmail = "t3zeu12s12@gmail.com";
        private const string FromPassword = "bingyrazipbooknc";

        public Task SendConfirmationLinkAsync(ApplicationUser user, string email, string confirmationLink) =>
            SendEmailAsync(email, "Confirmă-ți adresa de email",
                $"Bun venit, {user.FirstName}!<br/>Te rugăm să confirmi contul dând click pe acest <a href='{confirmationLink}'>link</a>.");

        public Task SendPasswordResetLinkAsync(ApplicationUser user, string email, string resetLink) =>
            SendEmailAsync(email, "Resetează-ți parola",
                $"Poți reseta parola contului tău dând click pe acest <a href='{resetLink}'>link</a>.");

        public Task SendPasswordResetCodeAsync(ApplicationUser user, string email, string resetCode) =>
            SendEmailAsync(email, "Cod de resetare a parolei",
                $"Codul tău de resetare este: <strong>{resetCode}</strong>");

        private Task SendEmailAsync(string toEmail, string subject, string htmlMessage)
        {
            var smtpClient = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential(FromEmail, FromPassword),
                EnableSsl = true
            };

            var mail = new MailMessage
            {
                From = new MailAddress(FromEmail),
                Subject = subject,
                Body = htmlMessage,
                IsBodyHtml = true
            };

            mail.To.Add(toEmail);

            return smtpClient.SendMailAsync(mail);
        }
    }
}
