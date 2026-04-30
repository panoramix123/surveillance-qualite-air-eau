using System.Windows;
using System.Windows.Controls;

namespace Smart_Territories
{
    public partial class Pollens : Page
    {
        public Pollens()
        {
            InitializeComponent();
            this.Unloaded += Page_Unloaded;
        }

        private async void NavigateurPollen_NavigationCompleted(object sender, Microsoft.Web.WebView2.Core.CoreWebView2NavigationCompletedEventArgs e)
        {
            // Le code JavaScript brut qui masque l'en-tête et le pied de page du site Atmo
            string script = @"
                var header = document.querySelector('header'); if(header) header.style.display = 'none';
                var footer = document.querySelector('footer'); if(footer) footer.style.display = 'none';
                var alertbar = document.querySelector('.alert-bar'); if(alertbar) alertbar.style.display = 'none';
            ";

            // On ordonne au navigateur d'exécuter ce script
            await NavigateurPollen.CoreWebView2.ExecuteScriptAsync(script);
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            if (NavigateurPollen != null)
            {
                NavigateurPollen.Dispose();
            }
        }
    }
}