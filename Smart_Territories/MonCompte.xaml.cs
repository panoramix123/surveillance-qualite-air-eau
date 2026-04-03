using System.Windows;
using System.Windows.Controls;

namespace Smart_Territories
{
    public partial class MonCompte : Page
    {
        public MonCompte()
        {
            InitializeComponent();
            // On affiche les infos stockées dans la session
            LblUsername.Text = UserSession.Username;
            LblEmail.Text = UserSession.Email;
        }

        private void BtnModifier_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Fonction de modification bientôt disponible.");
        }

        private void BtnLogout_Click(object sender, RoutedEventArgs e)
        {
            UserSession.IsLoggedIn = false;
            var mainWindow = Window.GetWindow(this) as MainWindow;
            if (mainWindow != null) mainWindow.UpdateLoginButtonText("Se connecter");
            this.NavigationService.Navigate(new System.Uri("Accueil.xaml", System.UriKind.Relative));
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Supprimer définitivement ?", "Attention", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                BtnLogout_Click(sender, e);
            }
        }
    }
}