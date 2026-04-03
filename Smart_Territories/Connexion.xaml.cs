using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Smart_Territories
{
    /// <summary>
    /// Logique d'interaction pour Connexion.xaml
    /// </summary>
    public partial class Connexion : Page
    {
        public Connexion()
        {
            InitializeComponent();
        }

        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            string identifiant = TxtUsername.Text;
            string motDePasse = TxtPassword.Password;

            // Réinitialiser le message d'erreur à chaque tentative
            ErrorText.Visibility = Visibility.Collapsed;

            // 1. Vérification des champs vides
            if (string.IsNullOrWhiteSpace(identifiant) || string.IsNullOrWhiteSpace(motDePasse))
            {
                ShowError("Veuillez remplir tous les champs.");
                return;
            }

            // 2. Simulation de connexion
            if (identifiant == "admin" && motDePasse == "1234")
            {
                // On enregistre les infos dans la session
                UserSession.IsLoggedIn = true;
                UserSession.Username = identifiant;
                UserSession.Password = motDePasse;

                // Mise à jour du bouton de connexion dans la barre de navigation
                var mainWindow = Window.GetWindow(this) as MainWindow;
                if (mainWindow != null) mainWindow.UpdateLoginButtonText("Mon compte");

                // Redirection vers la page d'accueil
                this.NavigationService.Navigate(new Uri("Accueil.xaml", UriKind.Relative));
            }
            else
            {
                ShowError("Identifiant ou mot de passe incorrect.");
            }
        }

        // Petite méthode utilitaire pour afficher l'erreur proprement
        private void ShowError(string message)
        {
            ErrorText.Text = message;
            ErrorText.Visibility = Visibility.Visible;
        }
    }
}
