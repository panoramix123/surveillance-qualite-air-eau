using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using LiveCharts;
using LiveCharts.Wpf;
using Smart_Territories.Services;
using System.ComponentModel;

namespace Smart_Territories
{
    public partial class Graphiques : Page, INotifyPropertyChanged
    {
        // Correction du warning CS8612 sur la nullabilité
        public event PropertyChangedEventHandler? PropertyChanged;

        private ChartValues<double> _valeurs = new ChartValues<double>();
        public ChartValues<double> Valeurs
        {
            get => _valeurs;
            set { _valeurs = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Valeurs))); }
        }

        private ChartValues<string> _labelsTemps = new ChartValues<string>();
        public ChartValues<string> LabelsTemps
        {
            get => _labelsTemps;
            set { _labelsTemps = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(LabelsTemps))); }
        }

        public Graphiques()
        {
            InitializeComponent();
            DataContext = this;

            ApiService.Instance.DataUpdated += UpdateChart;
            this.Loaded += (s, e) => UpdateChart();
            this.Unloaded += (s, e) => ApiService.Instance.DataUpdated -= UpdateChart;
        }

        private void UpdateChart()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                if (TxtTitreGraph == null || ComboPolluant == null) return;

                if (ComboPolluant.SelectedItem is ComboBoxItem item)
                {
                    string nomPolluantCherche = item.Tag?.ToString() ?? "";
                    TxtTitreGraph.Text = item.Content.ToString() + ", en temps réel";

                    var historique = ApiService.Instance.Charts.FirstOrDefault(c => c.Title == nomPolluantCherche);

                    Valeurs.Clear();
                    LabelsTemps.Clear();

                    if (historique != null && historique.Points.Count > 0)
                    {
                        foreach (var point in historique.Points)
                        {
                            Valeurs.Add(point.Value);
                            LabelsTemps.Add(point.Label);
                        }
                    }

                    AjusterAxeEtSeuils(nomPolluantCherche);
                }
            });
        }

        private void AjusterAxeEtSeuils(string polluant)
        {
            string unite = "";
            double minV = 0;
            Func<double, string> formatAxeY = val => val.ToString("0");

            switch (polluant)
            {
                case "Ozone O₃ (µg/m³)":
                    LigneSeuilInfo.Value = 180; LigneSeuilAlerte.Value = 240; unite = "µg/m³"; break;
                case "PM₁₀ (µg/m³)":
                    LigneSeuilInfo.Value = 50; LigneSeuilAlerte.Value = 80; unite = "µg/m³"; break;
                case "PM₂.₅ (µg/m³)":
                    LigneSeuilInfo.Value = 25; LigneSeuilAlerte.Value = 50; unite = "µg/m³"; break;
                case "Dioxyde de carbone CO₂ (ppm)":
                    LigneSeuilInfo.Value = 1000; LigneSeuilAlerte.Value = 1500; unite = "ppm"; break;
                case "Dioxyde d'azote NO₂ (µg/m³)":
                    LigneSeuilInfo.Value = 200; LigneSeuilAlerte.Value = 400; unite = "µg/m³"; break;
                case "Dioxyde de soufre SO₂ (µg/m³)":
                    LigneSeuilInfo.Value = 300; LigneSeuilAlerte.Value = 500; unite = "µg/m³"; break;
                case "Pression atmosphérique (hPa)":
                    LigneSeuilInfo.Value = -1; LigneSeuilAlerte.Value = -1; unite = "hPa"; minV = double.NaN;
                    formatAxeY = val => val.ToString("0.0"); break;
                case "Température (°C)":
                    LigneSeuilInfo.Value = -1; LigneSeuilAlerte.Value = -1; unite = "°C"; minV = double.NaN;
                    formatAxeY = val => val.ToString("0.0"); break;
                case "Taux d'humidité (%)":
                    LigneSeuilInfo.Value = -1; LigneSeuilAlerte.Value = -1; unite = "%"; break;
                default:
                    LigneSeuilInfo.Value = -1; LigneSeuilAlerte.Value = -1; unite = ""; break;
            }

            AxeY.LabelFormatter = formatAxeY;
            if (TxtUnite != null) TxtUnite.Text = unite;
            AxeY.MinValue = minV;

            CheckSeuils.IsEnabled = LigneSeuilInfo.Value != -1;
            if (!CheckSeuils.IsEnabled) CheckSeuils.IsChecked = false;

            AppliquerAffichageSeuils();
        }

        private void AppliquerAffichageSeuils()
        {
            bool show = CheckSeuils.IsChecked == true && LigneSeuilInfo.Value > 0;
            LigneSeuilInfo.Visibility = show ? Visibility.Visible : Visibility.Hidden;
            LigneSeuilAlerte.Visibility = show ? Visibility.Visible : Visibility.Hidden;

            if (show)
            {
                double maxCourbe = Valeurs.Count > 0 ? Valeurs.Max() : 0;
                AxeY.MaxValue = maxCourbe > LigneSeuilAlerte.Value ? double.NaN : LigneSeuilAlerte.Value * 1.1;
            }
            else AxeY.MaxValue = double.NaN;
        }

        private void ComboPolluant_SelectionChanged(object sender, SelectionChangedEventArgs e) => UpdateChart();

        private void ComboCapteurs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ComboCapteurs.SelectedItem is ComboBoxItem item && ApiService.Instance != null)
                ApiService.Instance.ChangerCapteur(item.Tag?.ToString() ?? "1");
        }

        private void CheckSeuils_Changed(object sender, RoutedEventArgs e) => AppliquerAffichageSeuils();

        // --- GESTION DU SURVOL (MOUSEMOVE) ---
        public void MonGraphique_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (CheckSeuils.IsChecked != true || LigneSeuilInfo.Value == -1) { PopupSeuil.IsOpen = false; return; }

            try
            {
                var pos = e.GetPosition(MonGraphique);
                double pAlerte = MonGraphique.ConvertToPixels(new Point(0, LigneSeuilAlerte.Value)).Y;
                double pInfo = MonGraphique.ConvertToPixels(new Point(0, LigneSeuilInfo.Value)).Y;

                if (Math.Abs(pos.Y - pAlerte) <= 12)
                {
                    TxtPopupSeuil.Text = $"{LigneSeuilAlerte.Value} {TxtUnite.Text} - Seuil d'alerte";
                    BorderPopup.Background = new SolidColorBrush(Color.FromRgb(231, 76, 60));
                    PopupSeuil.IsOpen = true;
                }
                else if (Math.Abs(pos.Y - pInfo) <= 12)
                {
                    TxtPopupSeuil.Text = $"{LigneSeuilInfo.Value} {TxtUnite.Text} - Seuil d'information";
                    BorderPopup.Background = new SolidColorBrush(Color.FromRgb(243, 156, 18));
                    PopupSeuil.IsOpen = true;
                }
                else PopupSeuil.IsOpen = false;
            }
            catch { PopupSeuil.IsOpen = false; }
        }

        public void MonGraphique_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e) => PopupSeuil.IsOpen = false;
    }
}