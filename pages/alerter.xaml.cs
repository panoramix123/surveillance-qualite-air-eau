using System.Windows;
using System.Windows.Controls;
using Wpf_localiser.Models;
using Wpf_localiser.Services;

namespace Wpf_localiser.pages
{
    public partial class alerter : Page
    {
        private AbonneService _abonneService;

        public alerter()
        {
            InitializeComponent();
            _abonneService = new AbonneService();
            ActualiserListe();
        }

        private void ActualiserListe()
        {
            LstAbonnes.ItemsSource = null;
            LstAbonnes.ItemsSource = _abonneService.ObtenirAbonnes();
        }

        private void BtnAjouter_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(TxtNom.Text) && !string.IsNullOrWhiteSpace(TxtEmail.Text))
            {
                _abonneService.AjouterAbonne(new Abonne
                {
                    Nom = TxtNom.Text,
                    Email = TxtEmail.Text,
                    Commune = string.IsNullOrWhiteSpace(TxtCommune.Text) ? "Angers" : TxtCommune.Text
                });
                TxtNom.Clear(); TxtEmail.Clear(); TxtCommune.Clear();
                ActualiserListe();
            }
        }

        private async void BtnDiffuserAlerte_Click(object sender, RoutedEventArgs e)
        {
            LblStatus.Text = "Envoi en cours...";
            var abonnes = _abonneService.ObtenirAbonnes();
            int reussis = 0;

            foreach (var abonne in abonnes)
            {
                bool ok = await EmailService.EnvoyerAlerteAsync(abonne.Email, "Alerte SafeBreath", $"Seuil dépassé à {abonne.Commune}");
                if (ok) reussis++;
            }
            LblStatus.Text = $"Terminé. {reussis}/{abonnes.Count} envoyés.";
        }
    }
}