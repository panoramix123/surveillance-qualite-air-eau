using System;
using System.Collections.Generic;
using System.Linq;
using Smart_Territories.Models;

namespace Smart_Territories.Services
{
    public class DataSimulator
    {
        // Cette méthode remplace l'ancienne ApplyFrequencySimulation
        public List<ChartPoint> GenerateSimulatedPoints(PollutantChart chart, int intervalHours)
        {
            int pointsPerDay = 24 / intervalHours;

            // Si on est sur 24h, on renvoie simplement les jours normaux
            if (pointsPerDay <= 1)
            {
                var originalPoints = new List<ChartPoint>();
                foreach (var p in chart.Points)
                {
                    p.AxisLabel = p.Label;
                    originalPoints.Add(p);
                }
                return originalPoints;
            }

            var newPoints = new List<ChartPoint>();
            var random = new Random();

            for (int i = 0; i < chart.Points.Count - 1; i++)
            {
                var currentDay = chart.Points[i];
                var nextDay = chart.Points[i + 1];

                for (int step = 0; step < pointsPerDay; step++)
                {
                    double fraction = (double)step / pointsPerDay;
                    double interpolatedValue = currentDay.Value + (nextDay.Value - currentDay.Value) * fraction;
                    double noise = (random.NextDouble() - 0.5) * 0.04 * chart.MaxValue;
                    double finalValue = Math.Max(0, interpolatedValue + noise);

                    int currentHour = step * intervalHours;
                    string axisText = "";

                    if (intervalHours <= 3)
                    {
                        if (currentHour % 8 == 0) axisText = currentHour == 0 ? currentDay.Label : $"{currentHour}h";
                    }
                    else
                    {
                        axisText = currentHour == 0 ? currentDay.Label : $"{currentHour}h";
                    }

                    newPoints.Add(new ChartPoint
                    {
                        Label = currentDay.Label,
                        Value = finalValue,
                        AxisLabel = axisText
                    });
                }
            }

            newPoints.Add(new ChartPoint
            {
                Label = chart.Points.Last().Label,
                Value = chart.Points.Last().Value,
                AxisLabel = chart.Points.Last().Label
            });

            return newPoints;
        }
    }
}