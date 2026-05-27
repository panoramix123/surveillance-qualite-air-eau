<?php
require('vendor/autoload.php'); // Charge la librairie MQTT

use PhpMqtt\Client\MqttClient;
use PhpMqtt\Client\ConnectionSettings;

// --- 3. Correspondance entre les clés JSON (de TTN) et tes id_type en BDD ---
$types_mapping = [
    'temperature' => 1,
    'humidite'    => 2,
    'pression'    => 3,
    'so2'         => 4,
    'nox'         => 5,
    'no2'         => 6,
    'ozone'       => 7,
    'pm10'        => 8,
    'pm25'        => 9,
    'co2'         => 10,
    'O2'          => 11,
    'UTN'         => 12,
    'pH'          => 13
];

// --- 4. Connexion à MariaDB ---
try {
    $pdo = new PDO("mysql:host=$db_host;dbname=$db_name;charset=utf8", $db_user, $db_pass);
    $pdo->setAttribute(PDO::ATTR_ERRMODE, PDO::ERRMODE_EXCEPTION);
    echo "Lien BDD : OK\n";
} catch (PDOException $e) {
    die("Erreur BDD : " . $e->getMessage() . "\n");
}

// --- 5. Connexion au Broker MQTT TTN ---
$mqtt = new MqttClient($server, $port, $clientId);
$settings = (new ConnectionSettings)
    ->setUsername($username)
    ->setPassword($password)
    ->setKeepAliveInterval(60);

try {
    $mqtt->connect($settings, true);
    echo "Lien MQTT TTN : OK (En attente de données...)\n";

    // --- 6. Abonnement au flux de données ---
    $mqtt->subscribe($topic, function ($topic, $message) use ($pdo, $types_mapping) {
        $payload = json_decode($message, true);
        
        // On récupère les données décodées par ton Payload Formatter
        $uplink_message = $payload['uplink_message']['decoded_payload'] ?? null;
        
        if ($uplink_message) {
            // On récupère l'ID du capteur (le dernier octet de ta trame)
            $id_equipement = $uplink_message['emplacement'] ?? 1;
            $date_now = date('Y-m-d H:i:s');

            echo "\n--- Nouveau message reçu du capteur $id_equipement ---\n";

            // Boucle d'insertion pour chaque mesure présente dans le JSON
            foreach ($uplink_message as $cle => $valeur) {
                // Si la clé existe dans notre mapping (ex: 'temperature' -> 1)
                if (isset($types_mapping[$cle]) && $valeur !== null) {
                    $id_type = $types_mapping[$cle];

                    // --- LA CORRECTION EST ICI ---
                    // Génération de l'identifiant unique format texte (ex: "temperature_64a2b...")
                    $id_mesure_genere = $cle . '_' . uniqid();

                    try {
                        // Insertion avec id_mesure généré dynamiquement
                        $sql = "INSERT INTO Mesure (id_mesure, valeur, date_heure, id_equipement, id_type) 
                                VALUES (:id_m, :val, :dt, :id_e, :id_t)";
                        $stmt = $pdo->prepare($sql);
                        $stmt->execute([
                            'id_m' => $id_mesure_genere,
                            'val'  => $valeur,
                            'dt'   => $date_now,
                            'id_e' => $id_equipement,
                            'id_t' => $id_type
                        ]);
                        echo "Enregistré : $cle -> $valeur\n";
                    } catch (Exception $e) {
                        echo "Erreur d'insertion pour $cle : " . $e->getMessage() . "\n";
                    }
                }
            }
        }
    }, 0);

    // Boucle infinie pour maintenir le script en vie
    $mqtt->loop(true);

} catch (Exception $e) {
    echo "Erreur MQTT : " . $e->getMessage() . "\n";
}
?>
