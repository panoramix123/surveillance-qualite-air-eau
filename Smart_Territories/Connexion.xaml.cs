using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Smart_Territories
{
    public partial class Connexion : Page
    {
        // REMPLACE CETTE URL PAR CELLE DE TON PROJET MOCKAPI (en gardant /users à la fin)
        private readonly string apiUrl = "https://69d420fdd396bd74235ccb69.mockapi.io/users/users";

        // HttpClient unique pour toute la page (bonne pratique)
        private static readonly HttpClient client = new HttpClient();

        public Connexion()
        {
            InitializeComponent();
        }

        // --- MÉTHODE DE CONNEXION ---
        private async void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            string identifiant = TxtUsername.Text;
            string motDePasse = TxtPassword.Password;
            ErrorText.Visibility = Visibility.Collapsed;

            if (string.IsNullOrWhiteSpace(identifiant) || string.IsNullOrWhiteSpace(motDePasse))
            {
                ShowMessage("Veuillez remplir tous les champs.", true);
                return;
            }

            // Désactiver les boutons pendant le chargement
            ToggleButtons(false);
            ShowMessage("Connexion en cours...", false);

            try
            {
                // On interroge MockAPI pour chercher cet utilisateur précis
                string requestUrl = $"{apiUrl}?username={identifiant}";
                HttpResponseMessage response = await client.GetAsync(requestUrl);

                if (response.IsSuccessStatusCode)
                {
                    string jsonResponse = await response.Content.ReadAsStringAsync();

                    // MockAPI renvoie un tableau JSON, on le convertit en liste d'objets C#
                    var users = JsonSerializer.Deserialize<UserAccount[]>(jsonResponse);

                    // Si le tableau contient l'utilisateur et que le mot de passe correspond
                    if (users != null && users.Length > 0 && users[0].password == motDePasse && users[0].username == identifiant)
                    {
                        // Connexion réussie ! On met à jour la session
                        UserSession.IsLoggedIn = true;
                        UserSession.Username = users[0].username;
                        UserSession.Password = users[0].password;
                        UserSession.Email = users[0].email;

                        var mainWindow = Window.GetWindow(this) as MainWindow;
                        if (mainWindow != null) mainWindow.UpdateLoginButtonText("Mon compte");

                        this.NavigationService.Navigate(new Uri("Accueil.xaml", UriKind.Relative));
                        return; // Fin du bloc
                    }
                }

                // Si on arrive ici, soit l'utilisateur n'existe pas, soit le mdp est faux
                ShowMessage("Identifiant ou mot de passe incorrect.", true);
            }
            catch (Exception ex)
            {
                ShowMessage("Erreur réseau. Vérifiez votre connexion.", true);
            }
            finally
            {
                ToggleButtons(true);
            }
        }

        // --- MÉTHODE DE CRÉATION DE COMPTE ---
        private async void BtnRegister_Click(object sender, RoutedEventArgs e)
        {
            string identifiant = TxtUsername.Text;
            string motDePasse = TxtPassword.Password;
            ErrorText.Visibility = Visibility.Collapsed;

            if (string.IsNullOrWhiteSpace(identifiant) || string.IsNullOrWhiteSpace(motDePasse))
            {
                ShowMessage("Remplissez les champs pour créer un compte.", true);
                return;
            }

            ToggleButtons(false);
            ShowMessage("Vérification...", false);

            try
            {
                // 1. Vérifier si l'utilisateur existe déjà
                HttpResponseMessage checkResponse = await client.GetAsync($"{apiUrl}?username={identifiant}");
                string checkJson = await checkResponse.Content.ReadAsStringAsync();

                // Si MockAPI renvoie un résultat (pas un tableau vide "[]" ou "Not found")
                if (checkJson != "[]" && checkJson != "Not found")
                {
                    ShowMessage("Cet identifiant est déjà pris.", true);
                    ToggleButtons(true);
                    return;
                }

                ShowMessage("Création du compte...", false);

                // 2. Préparer les données du nouvel utilisateur
                var newUser = new UserAccount
                {
                    username = identifiant,
                    password = motDePasse,
                    email = $"{identifiant}@safebreath.fr" // Email généré par défaut
                };

                // Convertir l'objet en JSON
                string jsonBody = JsonSerializer.Serialize(newUser);
                var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

                // 3. Envoyer la requête POST à MockAPI
                HttpResponseMessage createResponse = await client.PostAsync(apiUrl, content);

                if (createResponse.IsSuccessStatusCode)
                {
                    ShowMessage("Compte créé avec succès ! Tu peux te connecter.", false);
                    ErrorText.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Green);
                    TxtPassword.Password = ""; // On vide le mot de passe par sécurité
                }
                else
                {
                    ShowMessage("Erreur lors de la création sur le serveur.", true);
                }
            }
            catch (Exception ex)
            {
                ShowMessage("Erreur réseau. Impossible de contacter le serveur.", true);
            }
            finally
            {
                ToggleButtons(true);
            }
        }

        // --- MÉTHODES UTILITAIRES ---
        private void ShowMessage(string message, bool isError)
        {
            ErrorText.Text = message;
            ErrorText.Foreground = new System.Windows.Media.SolidColorBrush(isError ? System.Windows.Media.Colors.DarkRed : System.Windows.Media.Colors.Black);
            ErrorText.Visibility = Visibility.Visible;
        }

        private void ToggleButtons(bool isEnabled)
        {
            BtnLogin.IsEnabled = isEnabled;
            BtnRegister.IsEnabled = isEnabled;
        }
    }

    // --- MODÈLE DE DONNÉES ---
    // Cette classe indique à C# comment lire le JSON de MockAPI
    public class UserAccount
    {
        public string id { get; set; }
        public string username { get; set; }
        public string password { get; set; }
        public string email { get; set; }
    }
}