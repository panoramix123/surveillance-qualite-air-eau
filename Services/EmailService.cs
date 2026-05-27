using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace Wpf_localiser.Services
{
    public static class EmailService
    {
        public static async Task<bool> EnvoyerAlerteAsync(string destinataire, string sujet, string messageCorps)
        {
            try
            {
                using (SmtpClient client = new SmtpClient("smtp.gmail.com", 587))
                {
                    client.EnableSsl = true;
                    client.UseDefaultCredentials = false;

                    // L'adresse mail avec le point
                    client.Credentials = new NetworkCredential("alerte.sathebreath@gmail.com", "xkpl dumy wwqh cafk");

                    using (MailMessage mail = new MailMessage())
                    {
                        mail.From = new MailAddress("alerte.sathebreath@gmail.com", "Supervision SafeBreath");
                        mail.To.Add(new MailAddress(destinataire));
                        mail.Subject = sujet;
                        mail.Body = messageCorps;

                        await client.SendMailAsync(mail);
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                LogService.EcrireErreur($"Erreur SMTP pour {destinataire} : {ex.Message}");
                return false;
            }
        }
    }
}