namespace Wpf_localiser.Models
{
    public class ZoneAir
    {
        public string NomVille { get; set; } = string.Empty;
        public double Latitude { get; set; }
        public double Longitude { get; set; }

        // C'EST CETTE LIGNE QUI MANQUAIT :
        public int IndiceAqi { get; set; }

        public string Description { get; set; } = string.Empty;
    }
}