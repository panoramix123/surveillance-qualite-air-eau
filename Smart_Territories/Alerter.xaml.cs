using Smart_Territories.Models;
using Smart_Territories.Services;
using System.Windows;
using System.Windows.Controls;

namespace Smart_Territories
{
    public partial class Alerter : Page
    {
        private AbonneService _abonneService;

        public Alerter()
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

        private void BtnSupprimer_Click(object sender, RoutedEventArgs e)
        {
            // 1. On identifie quel bouton a été cliqué
            Button btn = sender as Button;

            // 2. On récupère l'abonné qui était attaché à ce bouton
            if (btn != null && btn.CommandParameter is Abonne abonneASupprimer)
            {
                // 3. Fenêtre de confirmation 
                MessageBoxResult resultat = MessageBox.Show(
                    $"Êtes-vous sûr de vouloir supprimer l'abonné {abonneASupprimer.Nom} de la liste des alertes ?",
                    "Confirmation de suppression",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                // 4. Si l'utilisateur a cliqué sur "Oui"
                if (resultat == MessageBoxResult.Yes)
                {
                    _abonneService.SupprimerAbonne(abonneASupprimer); // Supprime du JSON
                    ActualiserListe(); // Met à jour l'affichage
                }
            }
        }
    }
}