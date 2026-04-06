using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Smart_Territories
{
    public partial class Graphiques : Page
    {
        // =====================================================================================
        // EXPLICATION DE L'HTTPCLIENT :
        // On déclare HttpClient en 'static' (unique pour toute l'application). 
        // C'est une règle stricte en C# : si tu crées un nouveau HttpClient à chaque fois 
        // que tu ouvres la page, tu vas saturer les ports réseau de ton PC (Socket Exhaustion).
        // =====================================================================================
        private static readonly HttpClient client = new HttpClient();

        // =====================================================================================
        // URL DE TON API :
        // Remplace "TON_ID" par la vraie valeur de ton projet MockAPI.
        // Assure-toi que l'URL se termine bien par /mesures (le nom de la ressource créée).
        // =====================================================================================
        private readonly string apiUrl = "https://69d420fdd396bd74235ccb69.mockapi.io/users/mesures";

        public Graphiques()
        {
            InitializeComponent();

            // =====================================================================================
            // EXPLICATION DU CHARGEMENT ASYNCHRONE :
            // Le constructeur d'une page (Graphiques()) ne peut pas être 'async'. 
            // Or, un appel réseau à une API prend du temps et DOIT être asynchrone pour ne pas 
            // figer (freezer) toute l'interface de l'application pendant que la donnée arrive.
            // On accroche donc notre méthode de chargement à l'événement "Loaded" de la page.
            // =====================================================================================
            this.Loaded += Graphiques_Loaded;
        }

        // Événement déclenché automatiquement quand la page s'affiche à l'écran
        private async void Graphiques_Loaded(object sender, RoutedEventArgs e)
        {
            // On lance la récupération des données
            await FetchDataFromApiAsync();
        }

        private async Task FetchDataFromApiAsync()
        {
            try
            {
                // =====================================================================================
                // 1. REQUÊTE RÉSEAU :
                // On envoie une requête HTTP de type GET à l'API MockAPI.
                // 'await' signifie : le code s'arrête ici et attend que le serveur réponde.
                // =====================================================================================
                HttpResponseMessage response = await client.GetAsync(apiUrl);

                // On vérifie si le serveur a répondu avec un code 200 (OK).
                if (response.IsSuccessStatusCode)
                {
                    // =====================================================================================
                    // 2. LECTURE DU JSON :
                    // On extrait le contenu textuel brut de la réponse (le bloc JSON écrit à l'étape 1).
                    // =====================================================================================
                    string jsonResponse = await response.Content.ReadAsStringAsync();

                    // =====================================================================================
                    // 3. DÉSÉRIALISATION (Transformation) :
                    // JsonSerializer prend le texte JSON incompréhensible pour le système, 
                    // et le convertit intelligemment en une liste d'objets C# (List<PollutantChart>).
                    // Il associe automatiquement "Title" du JSON à la propriété "Title" de la classe.
                    // Optionnel : PropertyNameCaseInsensitive évite les crashs si le JSON a des minuscules.
                    // =====================================================================================
                    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                    var chartsData = JsonSerializer.Deserialize<List<PollutantChart>>(jsonResponse, options);

                    if (chartsData != null)
                    {
                        // =====================================================================================
                        // 4. PRÉPARATION DU RENDU VISUEL :
                        // L'API ne sait pas quelle est la taille de notre écran. Elle envoie juste la valeur.
                        // On doit calculer la hauteur du rectangle à dessiner pour chaque barre du graphique.
                        // =====================================================================================
                        double maxPixelHeight = 110.0; // Hauteur max disponible dans le cadre gris du XAML

                        foreach (var chart in chartsData)
                        {
                            foreach (var point in chart.Points)
                            {
                                // =====================================================================================
                                // CALCUL EN CROIX :
                                // Hauteur_Barre = (Valeur_Polluant / Valeur_Maximum_Échelle) * Hauteur_Max_Ecran
                                // Cela empêche une valeur de 120 (pour l'Ozone) de sortir physiquement de l'écran.
                                // =====================================================================================
                                point.DisplayHeight = (point.Value / chart.MaxValue) * maxPixelHeight;
                            }
                        }

                        // =====================================================================================
                        // 5. AFFICHAGE (DATA BINDING) :
                        // On injecte notre liste finale dans le composant visuel 'ChartsContainer' du XAML.
                        // Le XAML va automatiquement boucler dessus et générer les rectangles.
                        // =====================================================================================
                        ChartsContainer.ItemsSource = chartsData;
                    }
                }
                else
                {
                    MessageBox.Show("Erreur de l'API : Le serveur a refusé la connexion.", "Erreur HTTP");
                }
            }
            catch (Exception ex)
            {
                // =====================================================================================
                // GESTION DES ERREURS FATALES :
                // Ce bloc "catch" s'active si tu n'as pas internet, ou si l'URL MockAPI est fausse.
                // Sans ça, l'application crasherait brutalement (Fermeture Windows).
                // =====================================================================================
                MessageBox.Show($"Impossible de joindre l'API.\nDétail : {ex.Message}", "Erreur Réseau");
            }
        }
    }

    // =====================================================================================
    // CLASSES DE DONNÉES (LES MODÈLES) :
    // Ces classes sont les "moules" qui permettent au C# de comprendre la forme du JSON.
    // Leurs noms de propriétés (Title, MaxValue, Points) DOIVENT être exactement 
    // les mêmes que les clés écrites dans le texte JSON sur MockAPI.
    // =====================================================================================

    public class PollutantChart
    {
        public string Title { get; set; }     // Nom affiché en haut de la carte
        public double MaxValue { get; set; }  // Limite haute pour calculer la proportion du graphique
        public List<ChartPoint> Points { get; set; } // Liste des 7 jours pour ce polluant
    }

    public class ChartPoint
    {
        public string Label { get; set; }     // Nom du jour (ex: "Lun")
        public double Value { get; set; }     // Valeur brute renvoyée par l'API

        // =====================================================================================
        // PROPRIÉTÉ CALCULÉE LOCALEMENT :
        // DisplayHeight n'existe pas dans le JSON de l'API. C'est normal.
        // C'est une donnée purement visuelle (Frontend) calculée par l'étape 4 ci-dessus 
        // juste avant l'affichage. Le { get; set; } permet au XAML de la lire.
        // =====================================================================================
        public double DisplayHeight { get; set; }
    }
}