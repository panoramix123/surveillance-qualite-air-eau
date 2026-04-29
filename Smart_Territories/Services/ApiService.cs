using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows.Threading;
using Smart_Territories.Models;

namespace Smart_Territories.Services
{
    public class ApiService
    {
        // 1. LE SINGLETON : L'unique instance immortelle pour toute l'application
        private static readonly HttpClient client = new HttpClient();
        private readonly string apiUrl = "https://smart-territories.enzofile.fr/api.php";

        // 2. ENSUITE SEULEMENT ON CRÉE LE SINGLETON (qui va utiliser les outils ci-dessus)
        public static ApiService Instance { get; } = new ApiService();

        private DispatcherTimer _bgTimer;

        public List<PollutantChart> Charts { get; private set; }

        // 2. L'ÉVÉNEMENT : L'alarme que le service déclenche quand le JSON est reçu
        public event Action DataUpdated;

        // 3. Constructeur PRIVATE : L'interface graphique ne peut plus le recréer
        private ApiService()
        {
            Charts = new List<PollutantChart>
            {
                new PollutantChart { Title = "PM₂.₅ (µg/m³)", MaxValue = 100, Points = new List<ChartPoint>() },
                new PollutantChart { Title = "PM₁₀ (µg/m³)", MaxValue = 2000, Points = new List<ChartPoint>() },
                new PollutantChart { Title = "Ozone O₃ (µg/m³)", MaxValue = 1000, Points = new List<ChartPoint>() },
                new PollutantChart { Title = "Dioxyde d'azote NO₂ (µg/m³)", MaxValue = 1000, Points = new List<ChartPoint>() },
                new PollutantChart { Title = "Dioxyde de soufre SO₂ (µg/m³)", MaxValue = 1000, Points = new List<ChartPoint>() },
                new PollutantChart { Title = "Dioxyde de carbone CO₂ (ppm)", MaxValue = 1000, Points = new List<ChartPoint>() },
                new PollutantChart { Title = "Pression atmosphérique (hPa)", MaxValue = 1200, Points = new List<ChartPoint>() },
                new PollutantChart { Title = "Température (°C)", MaxValue = 50, Points = new List<ChartPoint>() },
                new PollutantChart { Title = "Taux d'humidité (%)", MaxValue = 100, Points = new List<ChartPoint>() }
            };

            // Le service gère son propre chronomètre, indépendant des pages
            _bgTimer = new DispatcherTimer();
            _bgTimer.Interval = TimeSpan.FromSeconds(5);
            _bgTimer.Tick += async (s, e) => await FetchAndAppendDataAsync();
            _bgTimer.Start();

            // Premier appel immédiat sans attendre 5 secondes
            _ = FetchAndAppendDataAsync();
        }

        public void ChangeInterval(int seconds)
        {
            _bgTimer.Interval = TimeSpan.FromSeconds(seconds);
        }

        public async Task FetchAndAppendDataAsync()
        {
            try
            {
                string json = await client.GetStringAsync(apiUrl);
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var response = JsonSerializer.Deserialize<ApiResponse>(json, options);

                if (response == null || response.data == null) return;
                var d = response.data;

                string timeLabel = DateTime.Now.ToString("HH:mm:ss");

                void AddPoint(string title, double? value)
                {
                    if (!value.HasValue) return;
                    var chart = Charts.First(c => c.Title == title);
                    chart.Points.Add(new ChartPoint { Label = timeLabel, Value = value.Value, AxisLabel = timeLabel });
                    if (chart.Points.Count > 15) chart.Points.RemoveAt(0);
                }

                AddPoint("PM₂.₅ (µg/m³)", d.pm25);
                AddPoint("PM₁₀ (µg/m³)", d.pm10);
                AddPoint("Ozone O₃ (µg/m³)", d.ozone);
                AddPoint("Dioxyde d'azote NO₂ (µg/m³)", d.no2);
                AddPoint("Dioxyde de soufre SO₂ (µg/m³)", d.so2);
                AddPoint("Dioxyde de carbone CO₂ (ppm)", d.co2);
                AddPoint("Pression atmosphérique (hPa)", d.pression);
                AddPoint("Température (°C)", d.temperature);
                AddPoint("Taux d'humidité (%)", d.humidite);

                // 4. ON PRÉVIENT L'INTERFACE que la mémoire vive vient d'être modifiée
                DataUpdated?.Invoke();
            }
            catch
            {
            }
        }
    }

    public class ApiResponse { public ApiData data { get; set; } }

    public class ApiData
    {
        public double? temperature { get; set; }
        public double? pression { get; set; }
        public double? humidite { get; set; }
        public double? so2 { get; set; }
        public double? nox { get; set; }
        public double? no2 { get; set; }
        public double? ozone { get; set; }
        public double? pm10 { get; set; }
        public double? co2 { get; set; }
        public double? O2 { get; set; }
        public double? UTN { get; set; }
        public double? pH { get; set; }
        [JsonPropertyName("pm2.5")]
        public double? pm25 { get; set; }
    }
}