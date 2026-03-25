<?php
require('vendor/autoload.php');

use \PhpMqtt\Client\MqttClient;
use \PhpMqtt\Client\ConnectionSettings;

// --- 1. Configuration TTN ---
$server   = 'server';
$port     = "port";
$clientId = 'client_id';
$username = 'username';
$password = 'password';
$topic    = 'topic';

// --- 2. Configuration MariaDB/MySQL ---
$db_host = 'localhost';
$db_name = 'dbnale';
$db_user = 'user';
$db_pass = 'password!';

try {
    $pdo = new PDO("mysql:host=$db_host;dbname=$db_name;charset=utf8", $db_user, $db_pass);
    $pdo->setAttribute(PDO::ATTR_ERRMODE, PDO::ERRMODE_EXCEPTION);

    $mqtt = new MqttClient($server, $port, $clientId);
    $settings = (new ConnectionSettings)
        ->setUsername($username)
        ->setPassword($password);

    $mqtt->connect($settings, true);
    echo "Connecté au broker TTN avec succès !\n";

    // --- 3. Un seul abonnement suffit ---
    $mqtt->subscribe($topic, function ($topic, $message) use ($pdo) {
        $payload = json_decode($message, true);
        
        $dev_id = $payload['end_device_ids']['device_id'] ?? 'inconnu';
        $temp   = $payload['uplink_message']['decoded_payload']['temperature'] ?? null;

        if ($temp !== null) {
            // On cherche l'ID de l'équipement
            $stmt = $pdo->prepare("SELECT id_equipement FROM EQUIPEMENT WHERE ref_materiel = ?");
            $stmt->execute([$dev_id]);
            $equip = $stmt->fetch(PDO::FETCH_ASSOC);

            if ($equip) {
                $id_e = $equip['id_equipement'];
                $id_mesure_unique = uniqid(); 

                // Insertion (id_type 1 = Température)
                $insert = $pdo->prepare("INSERT INTO Mesure (id_mesure, valeur, id_equipement, id_type) VALUES (?, ?, ?, 1)");
                $insert->execute([$id_mesure_unique, $temp, $id_e]);
                
                echo "Mesure enregistrée pour $dev_id : " . $temp . " °C\n";
            } else {
                echo "Attention : Capteur [$dev_id] non référencé dans la table EQUIPEMENT\n";
            }
        }
    }, 0);

    // Garde le script en vie
    $mqtt->loop(true);

} catch (Exception $e) {
    echo "Erreur : " . $e->getMessage();
}