using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Smart_Territories.Models
{
    public class ChartPoint
    {
        public string Label { get; set; }
        public double Value { get; set; }
        public double PointX { get; set; }
        public double PointY { get; set; }
        public double TextX { get; set; }
        public double TextY { get; set; }
        public string LabelText { get; set; }
        public Visibility MarkerVisibility { get; set; }

        //Pour l'axe des temps en bas ---
        public string AxisLabel { get; set; }
        public double AxisX { get; set; }
    }
}
