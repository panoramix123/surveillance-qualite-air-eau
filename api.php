<?php
// --- 1. SÉCURITÉ : Vérification de la clé d'API ---
// C'est cette clé que tes camarades devront envoyer dans le header HTTP 'x-api-key'
$cle_valide = "cle_valide"; 

$headers = apache_request_headers();
$cle_recue = isset($headers['x-api-key']) ? $headers['x-api-key'] : '';

if ($cle_recue !== $cle_valide) {
    http_response_code(403);
    header('Content-Type: application/json');
    echo json_encode(["erreur" => "Acces refuse. Cle API manquante ou invalide."]);
    exit(); // Bloque l'exécution si la clé est mauvaise
}

// --- 2. EN-TÊTES HTTP (CORS & Type de contenu) ---
// Autorise les navigateurs web à lire les données (contourne la sécurité CORS côté client)
header("Access-Control-Allow-Origin: *"); 
header("Access-Control-Allow-Headers: x-api-key, Content-Type");
header('Content-Type: application/json'); // Dit au client que la réponse est du JSON

// --- 3. CONNEXION À LA BASE DE DONNÉES ---
$db_host = 'host';
$db_name = 'smart_territories';
$db_user = 'user';
$db_pass = 'password';

try {
    $pdo = new PDO("mysql:host=$db_host;dbname=$db_name;charset=utf8mb4", $db_user, $db_pass);
    $pdo->setAttribute(PDO::ATTR_ERRMODE, PDO::ERRMODE_EXCEPTION);
} catch (\PDOException $e) {
    http_response_code(500);
    echo json_encode(["erreur" => "Connexion BDD impossible"]);
    exit();
}

// --- 4. RÉCUPÉRATION DU CAPTEUR DEMANDÉ ---
// Cherche "?capteur=2" dans l'URL. Si rien n'est précisé, on interroge le capteur 1.
$id_equipement = isset($_GET['capteur']) ? (int)$_GET['capteur'] : 1; 

// --- 5. REQUÊTE SQL ---
// Récupère les mesures ayant la date la plus récente pour le capteur spécifié
$sql = "SELECT id_type, valeur 
        FROM Mesure 
        WHERE id_equipement = :id_equipement 
        AND date_heure = (
            SELECT MAX(date_heure) 
            FROM Mesure 
            WHERE id_equipement = :id_equipement
        )";

$stmt = $pdo->prepare($sql);
$stmt->execute(['id_equipement' => $id_equipement]);
$lignes = $stmt->fetchAll(PDO::FETCH_ASSOC);

// --- 6. FORMATAGE DES DONNÉES ---
// On fait correspondre l'id_type de ta table MariaDB au nom voulu dans le JSON
$mapping_types = [
    1  => "temperature",
    2  => "humidite",
    3  => "pression",
    4  => "so2",
    5  => "nox",
    6  => "no2",
    7  => "ozone",
    8  => "pm10",
    9  => "pm2.5", // Nom exact demandé dans ta trame JSON
    10 => "co2",
    11 => "O2",
    12 => "UTN",
    13 => "pH"
];

// Structure par défaut (met 'null' si le capteur n'a pas envoyé cette donnée)
$data_json = [
    "temperature" => null,
    "pression"    => null,
    "humidite"    => null,
    "so2"         => null,
    "nox"         => null,
    "no2"         => null,
    "ozone"       => null,
    "pm10"        => null,
    "pm2.5"       => null,
    "co2"         => null,
    "O2"          => null,
    "UTN"         => null,
    "pH"          => null
];

// Remplissage du tableau avec les valeurs de la BDD
foreach ($lignes as $ligne) {
    $id_type = $ligne['id_type'];
    if (array_key_exists($id_type, $mapping_types)) {
        $nom_json = $mapping_types[$id_type];
        // Forcer le type Float pour éviter d'avoir des nombres entre guillemets ("20.5")
        $data_json[$nom_json] = (float)$ligne['valeur']; 
    }
}

// --- 7. GÉNÉRATION DE LA TRAME FINALE ---
$reponse_finale = [
    "capteur" => [
        "id" => $id_equipement
    ],
    "data" => $data_json
];

// Affiche le résultat proprement structuré
echo json_encode($reponse_finale, JSON_PRETTY_PRINT);
?>
