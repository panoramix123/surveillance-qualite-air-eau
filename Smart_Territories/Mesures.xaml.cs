using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives; // Pour PlacementMode

namespace Smart_Territories
{
    public partial class Mesures : Page
    {
        public Mesures()
        {
            InitializeComponent();
        }

        private void InfoButton_Click(object sender, RoutedEventArgs e)
        {
            Button clickedButton = sender as Button;
            if (clickedButton == null) return;

            string pollutantId = clickedButton.Tag as string;
            if (string.IsNullOrEmpty(pollutantId)) return;

            // On interroge notre service pur
            var info = Services.PolluantInfoService.ObtenirInformations(pollutantId);

            if (info.Titre == "Inconnu") return;

            // On met à jour l'interface
            PopupTitleText.Text = info.Titre;
            PopupDetailText.Text = info.Description;

            InfoPopup.PlacementTarget = clickedButton;
            InfoPopup.IsOpen = true;
        }
    }
}