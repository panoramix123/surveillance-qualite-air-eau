using System;
using System.IO;

namespace Wpf_localiser.Services
{
    public static class LogService
    {
        private static string strPath = AppDomain.CurrentDomain.BaseDirectory + "log.txt";

        public static void subWriteLog(string strRubrique, string strMsg)
        {
            try
            {
                string strLine = $"{DateTime.Now:dd/MM/yyyy - HH:mm:ss} - {strRubrique.PadRight(6)} - {strMsg}";
                File.AppendAllLines(strPath, new[] { strLine });
            }
            catch { }
        }
    }
}