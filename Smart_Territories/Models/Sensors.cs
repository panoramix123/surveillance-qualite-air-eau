namespace Smart_Territories.Models
{
    public class Sensor
    {
        public string strNom { get; set; } = string.Empty;          // Nom du capteur
        public double dblLat { get; set; }          // Latitude
        public double dblLon { get; set; }          // Longitude
        public string strType { get; set; } = string.Empty;         // "Air" ou "Eau"[cite: 1]
        public string strDerniereMesure { get; set; } = string.Empty; // Donnée variable[cite: 1]
    }
}