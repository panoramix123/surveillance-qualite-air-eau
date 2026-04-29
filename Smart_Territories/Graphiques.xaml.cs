using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Smart_Territories.Models;
using Smart_Territories.Services;

namespace Smart_Territories
{
    public partial class Graphiques : Page
    {
        private List<PollutantChart> allCharts;
        private List<PollutantSelectorItem> selectorItems;

        public Graphiques()
        {
            InitializeComponent();

            // On s'abonne à l'événement global : à chaque nouveau JSON, la fonction UpdateDisplay s'exécute
            ApiService.Instance.DataUpdated += UpdateDisplay;

            this.Loaded += Page_Loaded;
            this.Unloaded += Page_Unloaded; // INDISPENSABLE pour éviter les fuites de mémoire
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            // On récupère les données de l'instance immortelle
            allCharts = ApiService.Instance.Charts;

            if (selectorItems == null)
            {
                selectorItems = allCharts.Select(c => new PollutantSelectorItem { Name = c.Title, IsSelected = true }).ToList();
                PollutantSelector.ItemsSource = selectorItems;
            }

            UpdateDisplay();
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            // DÉCONNEXION OBLIGATOIRE quand on quitte l'onglet
            ApiService.Instance.DataUpdated -= UpdateDisplay;
        }

        private void UpdateDisplay()
        {
            if (allCharts == null || ChartsContainer == null || TxtTitle == null) return;

            var selectedNames = selectorItems.Where(i => i.IsSelected).Select(i => i.Name).ToList();
            var filtered = allCharts.Where(c => selectedNames.Contains(c.Title)).ToList();

            foreach (var chart in filtered)
            {
                chart.DisplayPoints = new List<ChartPoint>(chart.Points);
                CalculateCoordinates(chart);
            }

            ChartsContainer.ItemsSource = null;
            ChartsContainer.ItemsSource = filtered;
            TxtTitle.Text = selectedNames.Count == 0 ? "Aucun polluant sélectionné" : "Analyses détaillées (En Direct)";
        }

        private void SliderFrequency_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            int seconds = (int)e.NewValue;
            if (TxtSliderValue != null) TxtSliderValue.Text = $"{seconds}s";

            // On envoie la nouvelle vitesse au chronomètre du Singleton
            ApiService.Instance.ChangeInterval(seconds);
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
    }  
}