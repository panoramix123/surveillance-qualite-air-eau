using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Smart_Territories.Models;

namespace Smart_Territories.Services
{
    // --- MODÈLES CORRESPONDANT AU JSON ---
    public class ReleveHoraire
    {
        public string date_heure { get; set; }
        public double pm10 { get; set; }
        public double pm25 { get; set; }
        public double no2 { get; set; }
        public double so2 { get; set; }
        public double o3 { get; set; }
    }

    public class OpenMeteoResponse
    {
        public List<ReleveHoraire> polluants { get; set; }
    }

    // --- SERVICE PRINCIPAL ---
    public class ApiService
    {
        private static readonly HttpClient client = new HttpClient();

        private string apiUrl = "https://smart-territories.enzofile.fr/Data/donnees_API_DataAirPL.json";

        public static ApiService Instance { get; } = new ApiService();

        public List<PollutantChart> Charts { get; private set; }
        public event Action DataUpdated;

        private ApiService()
        {
            // Les polluants absents de l'API Open-Meteo resteront initialisés avec des listes vides
            Charts = new List<PollutantChart>
            {
            new PollutantChart { Title = "PM₂.₅ (µg/m³)", MaxValue = 100, Points = new List<ChartPoint>() },
            new PollutantChart { Title = "PM₁₀ (µg/m³)", MaxValue = 200, Points = new List<ChartPoint>() },
            new PollutantChart { Title = "Ozone O₃ (µg/m³)", MaxValue = 250, Points = new List<ChartPoint>() },
            new PollutantChart { Title = "Dioxyde d'azote NO₂ (µg/m³)", MaxValue = 250, Points = new List<ChartPoint>() },
            new PollutantChart { Title = "Dioxyde de soufre SO₂ (µg/m³)", MaxValue = 350, Points = new List<ChartPoint>() },
        
            // --- Variables exigées mais non fournies par l'API actuelle ---
            new PollutantChart { Title = "Dioxyde de carbone CO₂ (ppm)", MaxValue = 1000, Points = new List<ChartPoint>() },
            new PollutantChart { Title = "Pression atmosphérique (hPa)", MaxValue = 1200, Points = new List<ChartPoint>() },
            new PollutantChart { Title = "Température (°C)", MaxValue = 50, Points = new List<ChartPoint>() },
            new PollutantChart { Title = "Taux d'humidité (%)", MaxValue = 100, Points = new List<ChartPoint>() }
            };
        }

        public void ChangerCapteur(string idCapteur)
        {
            if (string.IsNullOrWhiteSpace(idCapteur)) return;

            _ = FetchAndAppendDataAsync();
        }

        public async Task FetchAndAppendDataAsync()
        {
            try
            {
                using var request = new HttpRequestMessage(HttpMethod.Get, apiUrl);
                request.Headers.Add("x-api-key", "CIEL_2026_Smart_Territories_Secret");

                using var responseHttp = await client.SendAsync(request);
                responseHttp.EnsureSuccessStatusCode();

                string json = await responseHttp.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var response = JsonSerializer.Deserialize<OpenMeteoResponse>(json, options);

                if (response?.polluants == null) return;

                // On ajoute la courbe
                foreach (var chart in Charts) chart.Points.Clear();

                // Boucle sur le tableau pour construire la courbe historique
                foreach (var releve in response.polluants)
                {
                    if (DateTime.TryParse(releve.date_heure, out DateTime dt))
                    {
                        string heure = dt.ToString("HH:mm");

                        AjouterPoint("PM₂.₅ (µg/m³)", releve.pm25, heure);
                        AjouterPoint("PM₁₀ (µg/m³)", releve.pm10, heure);
                        AjouterPoint("Ozone O₃ (µg/m³)", releve.o3, heure);
                        AjouterPoint("Dioxyde d'azote NO₂ (µg/m³)", releve.no2, heure);
                        AjouterPoint("Dioxyde de soufre SO₂ (µg/m³)", releve.so2, heure);
                    }
                }

                // Notification à l'IHM
                DataUpdated?.Invoke();
            }
            catch
            {
                // En cas d'échec réseau, on empêche le crash de l'application
            }
        }

        private void AjouterPoint(string titre, double valeur, string labelHeure)
        {
            var chart = Charts.FirstOrDefault(c => c.Title == titre);
            if (chart != null)
            {
                chart.Points.Add(new ChartPoint { Label = labelHeure, Value = valeur, AxisLabel = labelHeure });
            }
        }
    }
}