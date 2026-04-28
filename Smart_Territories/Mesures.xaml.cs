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

            string title = "";
            string moreInfo = "";

            // switch de données (conservé mais adapté)
            switch (pollutantId)
            {
                case "PM25":
                    title = "PM2.5 - Informations Détaillées";
                    moreInfo = "Origine : Combustions (trafic routier, chauffage, industrie, incendies).\n\nSanté : Pénètrent les barrières pulmonaires et atteignent le sang. Risques cardiovasculaires et respiratoires.\n\nNorme OMS (Annuelle) : < 5 µg/m³.";
                    break;
                case "PM10":
                    title = "PM10 - Informations Détaillées";
                    moreInfo = "Origine : Poussières de chantier, érosion des sols, chantiers, usure des freins/pneus.\n\nSanté : Retenues principalement dans le nez et les voies supérieures. Inflammation, asthme et bronchites.";
                    break;
                case "O3":
                    title = "Ozone (O₃) - Informations Détaillées";
                    moreInfo = "Origine : Polluant secondaire formé par réaction entre NOx et COV sous l'effet du soleil (fortes chaleurs).\n\nSanté : Puissant oxydant, irrite les yeux et les voies respiratoires.\n\nEnvironnement : Altère la croissance des plantes.";
                    break;
                case "NO2":
                    title = "Dioxyde d'azote (NO₂) - Détaillées";
                    moreInfo = "Origine : Combustion à haute température, moteurs Diesel.\n\nSanté : Gaz toxique, irritant pour les bronches. Augmente la sensibilité aux infections respiratoires.";
                    break;
                case "SO2":
                    title = "Dioxyde de soufre (SO₂) - Détaillées";
                    moreInfo = "Origine : Combustion de charbon et pétrole industriel.\n\nSanté : Irritation sévère des voies respiratoires.\n\nEnvironnement : Responsable des pluies acides.";
                    break;
                case "CO2":
                    title = "Dioxyde de carbone (CO₂) - Détaillées";
                    moreInfo = "Origine : Respiration humaine, énergies fossiles.\n\nConfort : Indicateur de confinement. > 1000 ppm : fatigue, maux de tête, baisse de concentration.";
                    break;
                case "HPA":
                    title = "Pression Atmosphérique - Impact";
                    moreInfo = "Anticyclone (Haute Pression) : Situation météorologique bloquée. Empêche la dispersion des polluants et favorise leur accumulation au sol.";
                    break;
                case "TEMP":
                    title = "Température - Impact Pollution";
                    moreInfo = "Les fortes chaleurs accélèrent les réactions chimiques créant des polluants secondaires (Ozone).";
                    break;
                case "HUMID":
                    title = "Taux d'humidité - Impact Pollution";
                    moreInfo = "Forte humidité : Peut agglomérer les particules fines les rendant plus lourdes. Brouillard : Maintient les polluants près du sol.";
                    break;
                case "O2":
                    title = "Oxygène Dissous (O₂) - Santé de l'eau";
                    moreInfo = "Crucial pour la faune aquatique. Une baisse (anoxie) peut être causée par la décomposition massive de matière organique suite à une pollution.\n\nNiveau sain : 7 à 11 mg/L.";
                    break;
                case "UTN":
                    title = "Turbidité (UTN) - Clarté de l'eau";
                    moreInfo = "Mesure le trouble dû aux matières en suspension. Bloque la lumière solaire, empêchant la photosynthèse des plantes aquatiques.";
                    break;
                case "PH":
                    title = "pH - Acidité de l'eau";
                    moreInfo = "Mesure l'acidité (0-7) ou la basicité (7-14). pH neutre sain : 6.5 à 8.5.\n\nAcidification : Rend toxiques certains métaux lourds.";
                    break;
                default:
                    return;
            }

            // CORRECTION INFO-BULLE : On popule le Popup
            PopupTitleText.Text = title;
            PopupDetailText.Text = moreInfo;

            // On définit le bouton cliqué comme la cible d'ancrage du Popup
            InfoPopup.PlacementTarget = clickedButton;

            // On ouvre le Popup dans l'application
            InfoPopup.IsOpen = true;
        }
    }
}