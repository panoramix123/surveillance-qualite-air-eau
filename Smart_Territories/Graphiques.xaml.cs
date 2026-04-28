using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Smart_Territories.Models;
using Smart_Territories.Services;

namespace Smart_Territories
{
    public partial class Graphiques : Page
    {
        private static readonly HttpClient client = new HttpClient();
        private readonly string apiUrl = "https://69d420fdd396bd74235ccb69.mockapi.io/users/mesures";

        // Liste complète stockée en mémoire pour éviter de rappeler l'API à chaque clic
        private List<PollutantChart> allCharts = new List<PollutantChart>();
        private List<PollutantSelectorItem> selectorItems = new List<PollutantSelectorItem>();

        public Graphiques()
        {
            InitializeComponent();
            this.Loaded += async (s, e) => await LoadDataAsync();
        }

        private async Task LoadDataAsync()
        {
            try
            {
                string json = await client.GetStringAsync(apiUrl);
                allCharts = JsonSerializer.Deserialize<List<PollutantChart>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                // Initialiser le menu de sélection à gauche
                selectorItems = allCharts.Select(c => new PollutantSelectorItem { Name = c.Title, IsSelected = true }).ToList();
                PollutantSelector.ItemsSource = selectorItems;

                UpdateDisplay();
            }
            catch { MessageBox.Show("Erreur API"); }
        }

        // Met à jour les graphiques affichés selon les CheckBox et la Fréquence
        // Met à jour les graphiques affichés selon les CheckBox et la Fréquence
        private void UpdateDisplay()
        {
            if (allCharts == null || ChartsContainer == null || TxtTitle == null) return;

            var selectedNames = selectorItems.Where(i => i.IsSelected).Select(i => i.Name).ToList();
            var filtered = allCharts.Where(c => selectedNames.Contains(c.Title)).ToList();

            // LECTURE DU CURSEUR (au lieu de la ComboBox)
            int intervalHours = (int)(SliderFrequency?.Value ?? 24);

            // On instancie notre nouveau service de calcul
            var simulator = new DataSimulator();

            foreach (var chart in filtered)
            {
                // On délègue le travail mathématique au service
                chart.DisplayPoints = simulator.GenerateSimulatedPoints(chart, intervalHours);
                CalculateCoordinates(chart);
            }

            ChartsContainer.ItemsSource = null;
            ChartsContainer.ItemsSource = filtered;
            TxtTitle.Text = selectedNames.Count == 0 ? "Aucun polluant sélectionné" : "Analyses détaillées";
        }

        private void CalculateCoordinates(PollutantChart chart)
        {
            double h = 110.0; double w = 300.0;
            chart.LinePoints = new PointCollection();
            double xStep = w / (chart.DisplayPoints.Count - 1);

            bool showMarkers = chart.DisplayPoints.Count <= 7;

            for (int i = 0; i < chart.DisplayPoints.Count; i++)
            {
                var p = chart.DisplayPoints[i];
                double x = i * xStep;
                double y = h - ((p.Value / chart.MaxValue) * h);
                chart.LinePoints.Add(new Point(x, y));

                p.PointX = x - 3;
                p.PointY = y - 3;
                p.TextX = x - 8;
                p.TextY = y - 18;

                // NOUVEAU : Position X pour le texte de l'axe des temps (décalage pour centrer le texte)
                p.AxisX = x - 8;

                p.LabelText = Math.Round(p.Value, 1).ToString();
                p.MarkerVisibility = showMarkers ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        private void CheckBox_FilterChanged(object sender, RoutedEventArgs e) => UpdateDisplay();
        private void SliderFrequency_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            // Met à jour le petit texte à côté du curseur (ex: "6h")
            if (TxtSliderValue != null) TxtSliderValue.Text = $"{e.NewValue}h";
            UpdateDisplay();
        }
    }  
}