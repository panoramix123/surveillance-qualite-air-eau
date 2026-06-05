namespace Smart_Territories.Services
{
    // L'objet de transport (DTO) pour stocker les réglages
    public class ConfigurationSeuil
    {
        public double SeuilInformation { get; set; } = -1;
        public double SeuilAlerte { get; set; } = -1;
        public string Unite { get; set; } = string.Empty;
        public double ValeurMinimum { get; set; } = 0;
        public string FormatAxe { get; set; } = "0"; // Format par défaut sans virgule
    }

    public static class SeuilPolluantService
    {
        public static ConfigurationSeuil ObtenirConfiguration(string polluant)
        {
            var config = new ConfigurationSeuil();

            switch (polluant)
            {
                case "Ozone O₃ (µg/m³)":
                    config.SeuilInformation = 180; config.SeuilAlerte = 240; config.Unite = "µg/m³"; break;
                case "PM₁₀ (µg/m³)":
                    config.SeuilInformation = 50; config.SeuilAlerte = 80; config.Unite = "µg/m³"; break;
                case "PM₂.₅ (µg/m³)":
                    config.SeuilInformation = 25; config.SeuilAlerte = 50; config.Unite = "µg/m³"; break;
                case "Dioxyde de carbone CO₂ (ppm)":
                    config.SeuilInformation = 1000; config.SeuilAlerte = 1500; config.Unite = "ppm"; break;
                case "Dioxyde d'azote NO₂ (µg/m³)":
                    config.SeuilInformation = 200; config.SeuilAlerte = 400; config.Unite = "µg/m³"; break;
                case "Dioxyde de soufre SO₂ (µg/m³)":
                    config.SeuilInformation = 300; config.SeuilAlerte = 500; config.Unite = "µg/m³"; break;
                case "Pression atmosphérique (hPa)":
                    config.Unite = "hPa"; config.ValeurMinimum = double.NaN; config.FormatAxe = "0.0"; break; // Avec 1 décimale
                case "Température (°C)":
                    config.Unite = "°C"; config.ValeurMinimum = double.NaN; config.FormatAxe = "0.0"; break;
                case "Taux d'humidité (%)":
                    config.Unite = "%"; break;
            }

            return config;
        }
    }
}