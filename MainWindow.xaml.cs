using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsPresentation;

namespace Wpf_localiser
{
    public class Abonne
    {
        public string Nom { get; set; } = string.Empty;
        public string Prenom { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string DateInscription { get; set; } = string.Empty;
    }

    public partial class MainWindow : Window
    {
        private ObservableCollection<Abonne> abonnes = new ObservableCollection<Abonne>();
        private readonly string cheminFichier = "abonnes.json";

        public MainWindow()
        {
            InitializeComponent();
            InitMap();
            ChargerAbonnes();
        }

        
        // 1. GESTION DE L'INTERFACE ET NAVIGATION
        
        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void cmdMinimize_Click(object sender, RoutedEventArgs e) => WindowState = WindowState.Minimized;

        private void cmdMaximize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
        }

        private void cmdClose_Click(object sender, RoutedEventArgs e) => Application.Current.Shutdown();

        // NOUVEAU : Méthode pour le bouton Accueil (Logo)
        private void cmdAccueil_Click(object sender, RoutedEventArgs e)
        {
            // Masquer toutes les vues pour revenir au fond gris de démarrage
            panMesures.Visibility = Visibility.Collapsed;
            panCartes.Visibility = Visibility.Collapsed;
            panAbonnes.Visibility = Visibility.Collapsed;
            panAlertes.Visibility = Visibility.Collapsed;
        }

        private void cmdNavigation_Click(object sender, RoutedEventArgs e)
        {
            panMesures.Visibility = Visibility.Collapsed;
            panCartes.Visibility = Visibility.Collapsed;
            panAbonnes.Visibility = Visibility.Collapsed;
            panAlertes.Visibility = Visibility.Collapsed;

            Button btn = (Button)sender;
            switch (btn.Name)
            {
                case "cmdNavMesures": panMesures.Visibility = Visibility.Visible; break;
                case "cmdNavCartes": panCartes.Visibility = Visibility.Visible; break;
                case "cmdNavAbonnes": panAbonnes.Visibility = Visibility.Visible; break;
                case "cmdNavAlertes": panAlertes.Visibility = Visibility.Visible; break;
            }
        }

        // 2. GESTION DES ABONNÉS
       

        private void ChargerAbonnes()
        {
            try
            {
                if (File.Exists(cheminFichier))
                {
                    string json = File.ReadAllText(cheminFichier);
                    abonnes = JsonSerializer.Deserialize<ObservableCollection<Abonne>>(json) ?? new ObservableCollection<Abonne>();
                }
                else
                {
                    abonnes = new ObservableCollection<Abonne>();
                }
            }
            catch (JsonException)
            {
                MessageBox.Show("Le fichier des abonnés est corrompu. Une liste vierge a été initialisée.", "Erreur JSON", MessageBoxButton.OK, MessageBoxImage.Warning);
                abonnes = new ObservableCollection<Abonne>();
            }

            dgAbonnes.ItemsSource = abonnes;
        }

        private void EnregistrerAbonnes()
        {
            string json = JsonSerializer.Serialize(abonnes, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(cheminFichier, json);
        }

        private void cmdAjouter_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNom.Text) || string.IsNullOrWhiteSpace(txtEmail.Text))
            {
                MessageBox.Show("Le nom et l'email sont requis.", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            Abonne nouvelAbonne = new Abonne
            {
                Nom = txtNom.Text.ToUpper(),
                Prenom = txtPrenom.Text,
                Email = txtEmail.Text,
                DateInscription = DateTime.Now.ToString("dd/MM/yyyy à HH:mm")
            };

            abonnes.Add(nouvelAbonne);
            EnregistrerAbonnes();

            txtNom.Clear();
            txtPrenom.Clear();
            txtEmail.Clear();
        }

        private void cmdSupprimer_Click(object sender, RoutedEventArgs e)
        {
            if (dgAbonnes.SelectedItem is Abonne abonneSelectionne)
            {
                abonnes.Remove(abonneSelectionne);
                EnregistrerAbonnes();
            }
        }

       
        // 3. CARTOGRAPHIE
        

        private void InitMap()
        {
            // Forcer le protocole de sécurité moderne pour OSM
            System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12 | System.Net.SecurityProtocolType.Tls13;
            GMap.NET.MapProviders.GMapProvider.UserAgent = "SafeBreath_ClientLourd/1.0 (alerte.sathebreath@gmail.com)";
            GMaps.Instance.Mode = AccessMode.ServerOnly;

            mGMap.MapProvider = OpenStreetMapProvider.Instance;
            mGMap.Position = new PointLatLng(46.2276, 2.2137);
            mGMap.MinZoom = 5;
            mGMap.MaxZoom = 18;
            mGMap.Zoom = 6;
            mGMap.MouseWheelZoomType = MouseWheelZoomType.MousePositionAndCenter;
            mGMap.DragButton = MouseButton.Left;
        }

        private void txtRechercheVille_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                cmdRechercherVille_Click(sender, e);
            }
        }

        private void cmdRechercherVille_Click(object sender, RoutedEventArgs e)
        {
            string ville = txtRechercheVille.Text;

            if (!string.IsNullOrWhiteSpace(ville))
            {
                GeoCoderStatusCode status;
                var point = OpenStreetMapProvider.Instance.GetPoint(ville, out status);

                if (point.HasValue && status == GeoCoderStatusCode.OK)
                {
                    mGMap.Position = point.Value;
                    mGMap.Zoom = 12;
                    AjouterZonesQualiteDynamique(point.Value);
                }
                else
                {
                    MessageBox.Show("Impossible de localiser cette ville.", "Recherche", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }

        private void AjouterZonesQualiteDynamique(PointLatLng centre)
        {
            mGMap.Markers.Clear();

            GMapPolygon polygon = new GMapPolygon(new[]
            {
                new PointLatLng(centre.Lat + 0.05, centre.Lng - 0.05),
                new PointLatLng(centre.Lat + 0.05, centre.Lng + 0.05),
                new PointLatLng(centre.Lat - 0.05, centre.Lng + 0.05),
                new PointLatLng(centre.Lat - 0.05, centre.Lng - 0.05)
            });

            polygon.Shape = new System.Windows.Shapes.Path
            {
                Stroke = Brushes.OrangeRed,
                StrokeThickness = 2,
                Fill = new SolidColorBrush(Color.FromArgb(60, 255, 0, 0)),
                ToolTip = "Zone de surveillance active"
            };

            mGMap.Markers.Add(polygon);
        }

        private void cmdLieuxCelebres_Click(object sender, RoutedEventArgs e)
        {
            mGMap.Markers.Clear();
            mGMap.Position = new PointLatLng(46.2276, 2.2137);
            mGMap.Zoom = 6;

            GMapMarker paris = new GMapMarker(new PointLatLng(48.8566, 2.3522))
            {
                Shape = new System.Windows.Shapes.Ellipse { Width = 15, Height = 15, Fill = Brushes.Blue, ToolTip = "Paris" }
            };
            mGMap.Markers.Add(paris);
        }

      
        // 4. ROUTAGE DES ALERTES SMTP

        private void cmdLancerAlerte_Click(object sender, RoutedEventArgs e)
        {
            if (abonnes == null || abonnes.Count == 0)
            {
                MessageBox.Show("La liste des abonnés est actuellement vide.", "Notification", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            lblAlerteInfo.Text = "Statut : Routage des paquets...";
            lblAlerteInfo.Foreground = Brushes.Orange;

            Task.Run(() =>
            {
                int envoisReussis = 0;

                foreach (var abonne in abonnes)
                {
                    bool succes = EmailService.EnvoyerAlerte(abonne.Email, "Alerte Environnementale", $"Notification de sécurité. Seuils dépassés pour : {abonne.Prenom} {abonne.Nom}.");
                    if (succes) envoisReussis++;
                }

                Application.Current.Dispatcher.Invoke(() =>
                {
                    lblAlerteInfo.Text = $"Statut : Terminé ({envoisReussis}/{abonnes.Count} notifiés)";
                    lblAlerteInfo.Foreground = envoisReussis > 0 ? Brushes.LightGreen : Brushes.Red;

                    lstHistoriqueAlertes.Items.Add($"[{DateTime.Now.ToString("HH:mm:ss")}] Alerte SMTP lancée - {envoisReussis} paquets délivrés.");
                });
            });
        }
    }
}