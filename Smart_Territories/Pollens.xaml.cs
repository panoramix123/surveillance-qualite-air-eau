using Microsoft.Web.WebView2.Core;
using System;
using System.Windows;
using System.Windows.Controls;

namespace Smart_Territories
{
    public partial class Pollens : Page
    {
        public Pollens()
        {
            InitializeComponent();
            InitializeWebView();
        }

        private async void InitializeWebView()
        {
            try
            {
                // 1. Définition du dossier de cache isolé dans AppData
                string localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                string cacheFolder = System.IO.Path.Combine(localAppData, "SafeBreath_Cache_WebView2");

                // 2. Localisation du dossier contenant le moteur embarqué
                string appFolder = AppDomain.CurrentDomain.BaseDirectory;
                string runtimeFolder = System.IO.Path.Combine(appFolder, "WebView2Runtime");

                // GESTION HYBRIDE : Si le dossier embarqué n'existe pas (ex: en cours de dev dans Visual Studio),
                // on passe 'null' pour forcer Windows à utiliser le runtime Edge global de ton PC.
                string? browserFolder = System.IO.Directory.Exists(runtimeFolder) ? runtimeFolder : null;

                // 3. Initialisation de l'environnement
                var env = await CoreWebView2Environment.CreateAsync(
                    browserExecutableFolder: browserFolder,
                    userDataFolder: cacheFolder
                );

                // 4. Lancement du composant
                await PollenWebView.EnsureCoreWebView2Async(env);

                // 5. Chargement de l'adresse de l'API de cartes
                PollenWebView.Source = new Uri("https://www.atmo-france.org/indiceatmo?bbox=-3.054199,46.749271,2.186279,48.427378&ind=pollen");

                PollenWebView.NavigationCompleted += async (s, e) =>
                {
                    string script = @"
                try {
                    document.querySelector('header').style.display = 'none';
                    document.querySelector('footer').style.display = 'none';
                    document.querySelector('.breadcrumb').style.display = 'none';
                    document.querySelector('.social-links').style.display = 'none';
                    document.body.style.backgroundColor = '#f4f4f4';
                } catch(e) {}
            ";
                    await PollenWebView.ExecuteScriptAsync(script);
                };
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Le module cartographique des pollens n'a pas pu s'initialiser.\n\nDétail technique : {ex.Message}",
                    "Erreur Critique d'initialisation",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
            }
        }
    }
}