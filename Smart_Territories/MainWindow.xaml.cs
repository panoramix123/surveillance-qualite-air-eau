using System;
using System.Windows;
using System.Windows.Input;

namespace Smart_Territories
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            // Ajouter cette ligne pour charger le contenu de bienvenue au démarrage
            MainFrame.Navigate(new Uri("Accueil.xaml", UriKind.Relative));
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
                
        }

        private void BtnMinimize_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void BtnMaximize_Click(object sender, RoutedEventArgs e)
        {
            // Alterne entre le plein écran et la taille normale
            if (this.WindowState == WindowState.Maximized)
            {
                this.WindowState = WindowState.Normal;
            }
            else
            {
                this.WindowState = WindowState.Maximized;
            }
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void BtnNavCartes_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new Uri("Cartes.xaml", UriKind.Relative));
        }

        private void BtnNavMesures_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new Uri("Mesures.xaml", UriKind.Relative));
        }

        private void BtnNavGraphiques_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new Uri("Graphiques.xaml", UriKind.Relative));
        }

        private void BtnNavAlerter_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new Uri("Alerter.xaml", UriKind.Relative));
        }

        private void BtnNavParametres_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new Uri("Parametres.xaml", UriKind.Relative));
        }

        private void BtnNavSeConnecter_Click(object sender, RoutedEventArgs e)
        {
            if (UserSession.IsLoggedIn)
            {
                MainFrame.Navigate(new Uri("MonCompte.xaml", UriKind.Relative));
            }
            else
            {
                MainFrame.Navigate(new Uri("Connexion.xaml", UriKind.Relative));
            }
        }

        private void BtnNavAccueil_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new Uri("Accueil.xaml", UriKind.Relative));
        }

        public void UpdateLoginButtonText(string newText)
        {
            BtnLoginMenu.Content = newText;
        }
    }
}