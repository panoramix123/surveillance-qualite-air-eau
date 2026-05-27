using Smart_Territories.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace Smart_Territories.Services
{
    public class AbonneService
    {
        private const string FichierJson = "abonnes.json";
        private List<Abonne> _abonnes = new List<Abonne>();

        public AbonneService()
        {
            ChargerAbonnes();
        }

        private void ChargerAbonnes()
        {
            try
            {
                if (File.Exists(FichierJson))
                {
                    string json = File.ReadAllText(FichierJson);
                    _abonnes = JsonSerializer.Deserialize<List<Abonne>>(json) ?? new List<Abonne>();
                }
            }
            catch (Exception ex)
            {
                LogService.EcrireErreur("Erreur lecture JSON : " + ex.Message);
            }
        }

        public List<Abonne> ObtenirAbonnes() => _abonnes;

        public void AjouterAbonne(Abonne abonne)
        {
            abonne.DateModification = DateTime.Now;
            _abonnes.Add(abonne);
            SauvegarderAbonnes();
        }

        public void SupprimerAbonne(Abonne abonneASupprimer)
        {
            //On cherche l'abonné dans la liste 
            var abonne = _abonnes.Find(a => a.Email == abonneASupprimer.Email);

            if (abonne != null)
            {
                _abonnes.Remove(abonne);
                SauvegarderAbonnes();
            }
        }

        /// Modifie un abonné existant et enregistre l'horodatage exact de la modification.
        public void ModifierAbonne(string email, string nouveauNom, string nouvelleCommune)
        {
            // Recherche de l'abonné correspondant dans la liste
            Abonne abonne = _abonnes.Find(a => a.Email.Equals(email, StringComparison.OrdinalIgnoreCase));

            if (abonne != null)
            {
                // Application des nouvelles valeurs
                abonne.Nom = nouveauNom;
                abonne.Commune = nouvelleCommune;

                // Capture instantanée de la date et de l'heure de modification
                abonne.DateModification = DateTime.Now;

                // Persistance des données mises à jour dans le fichier JSON
                SauvegarderAbonnes();
            }
        }

        private void SauvegarderAbonnes()
        {
            try
            {
                string json = JsonSerializer.Serialize(_abonnes, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(FichierJson, json);
            }
            catch (Exception ex)
            {
                LogService.EcrireErreur("Erreur écriture JSON : " + ex.Message);
            }
        }


    }
}