using System.Collections.Generic;

namespace Smart_Territories.Services
{
    //Stockage du résultat
    public class PolluantInfo
    {
        public string Titre { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }

    public static class PolluantInfoService
    {
        public static PolluantInfo ObtenirInformations(string pollutantId)
        {
            var info = new PolluantInfo();

            switch (pollutantId)
            {
                case "PM25":
                    info.Titre = "PM2.5 - Informations Détaillées";
                    info.Description = "Origine : Combustions (trafic routier, chauffage, industrie, incendies).\n\nSanté : Pénètrent les barrières pulmonaires et atteignent le sang. Risques cardiovasculaires et respiratoires.\n\nNorme OMS (Annuelle) : < 5 µg/m³.";
                    break;
                case "PM10":
                    info.Titre = "PM10 - Informations Détaillées";
                    info.Description = "Origine : Poussières de chantier, érosion des sols, chantiers, usure des freins/pneus.\n\nSanté : Retenues principalement dans le nez et les voies supérieures. Inflammation, asthme et bronchites.";
                    break;
                case "O3":
                    info.Titre = "Ozone (O₃) - Informations Détaillées";
                    info.Description = "Origine : Polluant secondaire formé par réaction entre NOx et COV sous l'effet du soleil (fortes chaleurs).\n\nSanté : Puissant oxydant, irrite les yeux et les voies respiratoires.\n\nEnvironnement : Altère la croissance des plantes.";
                    break;
                case "NO2":
                    info.Titre = "Dioxyde d'azote (NO₂) - Détaillées";
                    info.Description = "Origine : Combustion à haute température, moteurs Diesel.\n\nSanté : Gaz toxique, irritant pour les bronches. Augmente la sensibilité aux infections respiratoires.";
                    break;
                case "SO2":
                    info.Titre = "Dioxyde de soufre (SO₂) - Détaillées";
                    info.Description = "Origine : Combustion de charbon et pétrole industriel.\n\nSanté : Irritation sévère des voies respiratoires.\n\nEnvironnement : Responsable des pluies acides.";
                    break;
                case "CO2":
                    info.Titre = "Dioxyde de carbone (CO₂) - Détaillées";
                    info.Description = "Origine : Respiration humaine, énergies fossiles.\n\nConfort : Indicateur de confinement. > 1000 ppm : fatigue, maux de tête, baisse de concentration.";
                    break;
                case "HPA":
                    info.Titre = "Pression Atmosphérique - Impact";
                    info.Description = "Anticyclone (Haute Pression) : Situation météorologique bloquée. Empêche la dispersion des polluants et favorise leur accumulation au sol.";
                    break;
                case "TEMP":
                    info.Titre = "Température - Impact Pollution";
                    info.Description = "Les fortes chaleurs accélèrent les réactions chimiques créant des polluants secondaires (Ozone).";
                    break;
                case "HUMID":
                    info.Titre = "Taux d'humidité - Impact Pollution";
                    info.Description = "Forte humidité : Peut agglomérer les particules fines les rendant plus lourdes. Brouillard : Maintient les polluants près du sol.";
                    break;
                case "O2":
                    info.Titre = "Oxygène Dissous (O₂) - Santé de l'eau";
                    info.Description = "Crucial pour la faune aquatique. Une baisse (anoxie) peut être causée par la décomposition massive de matière organique suite à une pollution.\n\nNiveau sain : 7 à 11 mg/L.";
                    break;
                case "UTN":
                    info.Titre = "Turbidité (UTN) - Clarté de l'eau";
                    info.Description = "Mesure le trouble dû aux matières en suspension. Bloque la lumière solaire, empêchant la photosynthèse des plantes aquatiques.";
                    break;
                case "PH":
                    info.Titre = "pH - Acidité de l'eau";
                    info.Description = "Mesure l'acidité (0-7) ou la basicité (7-14). pH neutre sain : 6.5 à 8.5.\n\nAcidification : Rend toxiques certains métaux lourds.";
                    break;
                default:
                    info.Titre = "Inconnu";
                    info.Description = "Aucune information disponible pour ce capteur.";
                    break;
            }

            return info;
        }
    }
}