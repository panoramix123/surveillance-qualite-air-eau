using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Smart_Territories.Models
{
    public class PollutantChart
    {
        public string Title { get; set; }
        public double MaxValue { get; set; }
        public List<ChartPoint> Points { get; set; }
        public List<ChartPoint> DisplayPoints { get; set; } // Points actuellement affichés
        public PointCollection LinePoints { get; set; }
    }
}
