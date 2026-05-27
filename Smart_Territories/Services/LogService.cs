using System;
using System.IO;

namespace Smart_Territories.Services
{
    public static class LogService
    {
        private const string LogFile = "application_erreurs.log";

        public static void EcrireErreur(string message)
        {
            try
            {
                string ligne = $"[{DateTime.Now:dd/MM/yyyy HH:mm:ss}] {message}{Environment.NewLine}";
                File.AppendAllText(LogFile, ligne);
            }
            catch { }
        }
    }
}