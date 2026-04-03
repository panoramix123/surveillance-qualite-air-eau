using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Smart_Territories
{
    public static class UserSession
    {
        public static bool IsLoggedIn { get; set; } = false;
        public static string Username { get; set; }
        public static string Password { get; set; } // En réel, on ne stocke jamais ça en clair
        public static string Email { get; set; } = "utilisateur@safebreath.fr"; // Email par défaut pour l'exemple
    }
}
