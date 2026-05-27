using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Wpf_localiser.Models;

namespace Wpf_localiser.Services
{
    public class OpenMeteoService
    {
        private readonly HttpClient _httpClient;

        public OpenMeteoService()
        {
            _httpClient = new HttpClient();
        }

        public async Task<List<ZoneAir>> RecupererDonneesAirReellesAsync()
        {
            // On prépare notre liste de villes à surveiller
            var zones = new List<ZoneAir>
            {
                new ZoneAir { NomVille = "Angers Centre", Latitude = 47.4784, Longitude = -0.5632 },
                new ZoneAir { NomVille = "Trélazé", Latitude = 47.4452, Longitude = -0.4651 },
                new ZoneAir { NomVille = "Saint-Barthélemy-d'Anjou", Latitude = 47.4667, Longitude = -0.4833 }
            };

            // Pour chaque ville, on interroge le vrai serveur Open-Meteo
            foreach (var zone in zones)
            {
                try
                {
                    // L'URL de l'API Open-Meteo pour la qualité de l'air (European AQI)
                    string url = $"https://air-quality-api.open-meteo.com/v1/air-quality?latitude={zone.Latitude.ToString(System.Globalization.CultureInfo.InvariantCulture)}&longitude={zone.Longitude.ToString(System.Globalization.CultureInfo.InvariantCulture)}&current=european_aqi";

                    // On télécharge la réponse
                    string jsonReponse = await _httpClient.GetStringAsync(url);

                    // On analyse le JSON (on cherche la valeur 'european_aqi' dans le bloc 'current')
                    using (JsonDocument doc = JsonDocument.Parse(jsonReponse))
                    {
                        JsonElement root = doc.RootElement;
                        int aqi = root.GetProperty("current").GetProperty("european_aqi").GetInt32();

                        zone.IndiceAqi = aqi;
                        zone.Description = $"Indice AQI Européen : {aqi} (Mesuré en temps réel)";
                    }
                }
                catch (Exception ex)
                {
                    zone.IndiceAqi = 0; // En cas d'erreur de connexion
                    zone.Description = "Erreur de connexion à Open-Meteo : " + ex.Message;
                }
            }

            return zones;
        }
    }
}