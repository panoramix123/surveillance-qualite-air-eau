namespace Smart_Territories.Models
{
    public class Abonne
    {
        public string Nom { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Commune { get; set; } = string.Empty;
        public DateTime DateModification { get; set; } = DateTime.Now;
    }
}