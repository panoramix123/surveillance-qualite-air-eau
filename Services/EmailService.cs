using System;
using System.Net;
using System.Net.Mail;

namespace Wpf_localiser
{
    public static class EmailService
    {
        public static bool EnvoyerAlerte(string destinataire, string sujet, string corps)
        {
            try
            {
                SmtpClient objSmtp = new SmtpClient("smtp.gmail.com", 587)
                {
                    Credentials = new NetworkCredential("alerte.sathebreath@gmail.com", "qqul sfxn ebaf ebpn"),
                    EnableSsl = true
                };

                MailMessage objMail = new MailMessage
                {
                    From = new MailAddress("alerte.sathebreath@gmail.com", "SafeBreath - Alerte"),
                    Subject = sujet,
                    Body = corps,
                    IsBodyHtml = true
                };

                objMail.To.Add(destinataire);
                objSmtp.Send(objMail);

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Échec de l'envoi SMTP : {ex.Message}");
                return false;
            }
        }
    }
}