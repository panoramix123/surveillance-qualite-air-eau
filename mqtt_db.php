<?php
require('vendor/autoload.php');

use \PhpMqtt\Client\MqttClient;
use \PhpMqtt\Client\ConnectionSettings;

// --- 1. Configuration TTN ---
$server   = 'eu1.cloud.thethings.network';
$port     = 1883;
$clientId = 'dell-r510-client';
$username = 'iqe-iqa@ttn';
$password = 'NNSXS.2NQ7D4QRNLCKMOU62KIZX5NVU2475ZIBN3UPFLA.CSICYZKMD2TK5BM3R4BZCYSES3EHXNYFBBFR2BMRRW6LGHTZQ2LA';
$topic    = 'v3/+/devices/+/up';

// --- 2. Configuration BDD ---
$db_host = 'localhost';
$db_name = 'smart_territories';
$db_user = 'user_iot';
$db_pass = 'Ciel_2026!';

try {
    $pdo = new PDO("mysql:host=$db_host;dbname=$db_name;charset=utf8", $db_user, $db_pass);
    $pdo->setAttribute(PDO::ATTR_ERRMODE, PDO::ERRMODE_EXCEPTION);

    $mqtt = new MqttClient($server, $port, $clientId);
    $settings = (new ConnectionSettings)->setUsername($username)->setPassword($password);

    $mqtt->connect($settings, true);
    echo "Connecté au broker TTN. En attente de données multi-capteurs...\n";

    // --- 3. Abonnement et traitement ---
    $mqtt->subscribe($topic, function ($topic, $message) use ($pdo) {
        $payload = json_decode($message, true);
        
        $dev_id = $payload['end_device_ids']['device_id'] ?? 'inconnu';
        $decoded_data = $payload['uplink_message']['decoded_payload'] ?? [];

        // Correspondance entre les clés JSON et tes id_type en BDD
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
            'co2'         => 10
        ];

        // 1. On cherche l'ID de l'équipement une seule fois
        $stmt = $pdo->prepare("SELECT id_equipement FROM EQUIPEMENT WHERE ref_materiel = ?");
        $stmt->execute([$dev_id]);
        $equip = $stmt->fetch(PDO::FETCH_ASSOC);

        if ($equip) {
            $id_e = $equip['id_equipement'];

            // 2. On boucle sur toutes les données reçues
            foreach ($types_mapping as $key => $id_type) {
                if (isset($decoded_data[$key])) {
                    $valeur = $decoded_data[$key];
                    $id_mesure_unique = uniqid($key . "_"); // ID unique pour chaque mesure

                    $insert = $pdo->prepare("INSERT INTO Mesure (id_mesure, valeur, id_equipement, id_type) VALUES (?, ?, ?, ?)");
                    $insert->execute([$id_mesure_unique, $valeur, $id_e, $id_type]);
                    
                    echo "[$dev_id] - $key : $valeur enregistrée (Type ID: $id_type)\n";
                }
            }
        } else {
            echo "Attention : Capteur [$dev_id] inconnu dans la table EQUIPEMENT\n";
        }
    }, 0);

    $mqtt->loop(true);

} catch (Exception $e) {
    echo "Erreur : " . $e->getMessage();
}
