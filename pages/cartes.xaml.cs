using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsPresentation;
using System.Net;
using Wpf_localiser.Services;
using Wpf_localiser.Models;

namespace Wpf_localiser.pages
{
    public partial class cartes : Page
    {
        public cartes()
        {
            InitializeComponent();
            ConfigurerCarte();

            // On lance le téléchargement des données au démarrage de la page
            _ = AfficherZonesAirAsync();
        }

        private void ConfigurerCarte()
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            GMapProvider.UserAgent = "ProjetSafeBreath_alerte.sathebreath@gmail.com";
            GMaps.Instance.Mode = AccessMode.ServerOnly;

            MainMap.MapProvider = GMapProviders.OpenStreetMap;
            MainMap.Position = new PointLatLng(47.4784, -0.5632);

            MainMap.MinZoom = 5;
            MainMap.MaxZoom = 20;
            MainMap.Zoom = 12;
            MainMap.MouseWheelZoomType = MouseWheelZoomType.MousePositionAndCenter;
            MainMap.CanDragMap = true;
            MainMap.DragButton = System.Windows.Input.MouseButton.Left;
        }

        // --- FONCTION ASYNCHRONE POUR LES VRAIES DONNÉES ---
        private async System.Threading.Tasks.Task AfficherZonesAirAsync()
        {
            MainMap.Markers.Clear();

            // On appelle notre nouveau service Open-Meteo
            OpenMeteoService serviceMeteo = new OpenMeteoService();
            var zones = await serviceMeteo.RecupererDonneesAirReellesAsync();

            foreach (var zone in zones)
            {
                SolidColorBrush couleurRemplissage;
                SolidColorBrush couleurBordure;

                // Échelle officielle de l'AQI Européen
                if (zone.IndiceAqi >= 100) // Mauvais / Alerte
                {
                    couleurRemplissage = new SolidColorBrush(Colors.Red) { Opacity = 0.4 };
                    couleurBordure = Brushes.DarkRed;
                }
                else if (zone.IndiceAqi >= 50) // Moyen
                {
                    couleurRemplissage = new SolidColorBrush(Colors.Orange) { Opacity = 0.4 };
                    couleurBordure = Brushes.DarkOrange;
                }
                else // Bon (0-49)
                {
                    couleurRemplissage = new SolidColorBrush(Colors.LimeGreen) { Opacity = 0.4 };
                    couleurBordure = Brushes.Green;
                }

                Ellipse cercleZone = new Ellipse
                {
                    Width = 80,
                    Height = 80,
                    Fill = couleurRemplissage,
                    Stroke = couleurBordure,
                    StrokeThickness = 2,
                    ToolTip = $"{zone.NomVille}\n{zone.Description}"
                };

                GMapMarker marqueur = new GMapMarker(new PointLatLng(zone.Latitude, zone.Longitude))
                {
                    Shape = cercleZone,
                    Offset = new Point(-40, -40)
                };

                MainMap.Markers.Add(marqueur);
            }
        }

        private void BtnRechercher_Click(object sender, RoutedEventArgs e)
        {
            string ville = TxtRecherche.Text.Trim();
            if (string.IsNullOrEmpty(ville)) return;

            GeoCoderStatusCode statusCode;
            var coordonnees = GMapProviders.OpenStreetMap.GetPoint(ville, out statusCode);

            if (statusCode == GeoCoderStatusCode.OK && coordonnees != null)
            {
                MainMap.Position = coordonnees.Value;
                MainMap.Zoom = 12;
            }
            else
            {
                MessageBox.Show($"Impossible de trouver '{ville}'.", "Recherche", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}