using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Wpf_localiser.Models;

namespace Wpf_localiser.Services
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
            _abonnes.Add(abonne);
            SauvegarderAbonnes();
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