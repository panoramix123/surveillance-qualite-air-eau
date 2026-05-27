using System;
using System.Windows.Controls;
using Microsoft.Web.WebView2.Core;

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
            // Initialisation sécurisée du moteur Edge
            await PollenWebView.EnsureCoreWebView2Async();

            // Chargement de l'URL par le code
            PollenWebView.Source = new Uri("https://www.atmo-france.org/indiceatmo?bbox=-3.054199,46.749271,2.186279,48.427378&ind=pollen");

            // Une fois la navigation terminée, on "découpe" la page pour l'esthétique
            PollenWebView.NavigationCompleted += async (s, e) =>
            {
                // Ce script JS cache le header, le footer et centre la carte
                string script = @"
                    try {
                        document.querySelector('header').style.display = 'none';
                        document.querySelector('footer').style.display = 'none';
                        document.querySelector('.breadcrumb').style.display = 'none';
                        document.querySelector('.social-links').style.display = 'none';
                        document.body.style.backgroundColor = '#f4f4f4'; // Fond neutre
                    } catch(e) {}
                ";
                await PollenWebView.ExecuteScriptAsync(script);
            };
        }
    }
}