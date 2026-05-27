using System.Windows; // Nécessaire pour afficher les MessageBox d'erreur
using System.Windows.Controls;
using GMap.NET;
using GMap.NET.MapProviders;
using System.Net;

namespace Wpf_localiser.pages
{
    public partial class cartes : Page
    {
        public cartes()
        {
            InitializeComponent();
            ConfigurerCarte();
        }

        private void ConfigurerCarte()
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            GMapProvider.UserAgent = "ProjetSafeBreath_alerte.sathebreath@gmail.com";
            GMaps.Instance.Mode = AccessMode.ServerOnly;

            MainMap.MapProvider = GMapProviders.OpenStreetMap;
            MainMap.Position = new PointLatLng(47.4784, -0.5632); // Angers par défaut

            MainMap.MinZoom = 5;
            MainMap.MaxZoom = 20;
            MainMap.Zoom = 13;
            MainMap.MouseWheelZoomType = MouseWheelZoomType.MousePositionAndCenter;
            MainMap.CanDragMap = true;
            MainMap.DragButton = System.Windows.Input.MouseButton.Left;
        }

        // --- NOUVELLE FONCTION : RECHERCHE DE VILLE ---
        private void BtnRechercher_Click(object sender, RoutedEventArgs e)
        {
            string ville = TxtRecherche.Text.Trim(); // Récupère le texte tapé

            if (string.IsNullOrEmpty(ville))
            {
                return; // Si la barre est vide, on ne fait rien
            }

            // On demande à OpenStreetMap de trouver les coordonnées GPS de la ville
            GeoCoderStatusCode statusCode;
            var coordonnees = GMapProviders.OpenStreetMap.GetPoint(ville, out statusCode);

            if (statusCode == GeoCoderStatusCode.OK && coordonnees != null)
            {
                // Si la ville est trouvée, on déplace la carte sur ce nouveau point
                MainMap.Position = coordonnees.Value;
                MainMap.Zoom = 12; // On ajuste le zoom pour bien voir la ville
            }
            else
            {
                // Si la ville n'existe pas ou est mal orthographiée
                MessageBox.Show($"Impossible de trouver la ville '{ville}'.\nVeuillez vérifier l'orthographe.",
                                "Recherche introuvable",
                                MessageBoxButton.OK,
                                MessageBoxImage.Warning);
            }
        }
    }
}