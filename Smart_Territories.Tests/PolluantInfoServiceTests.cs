using Xunit;
using Smart_Territories.Services;

namespace Smart_Territories.Tests
{
    public class PolluantInfoServiceTests
    {
        [Fact]
        public void ObtenirInformations_TagO3_RetourneBonTitreEtDescription()
        {
            // Arrange
            string tagCible = "O3";

            // Act
            var resultat = PolluantInfoService.ObtenirInformations(tagCible);

            // Assert
            Assert.Equal("Ozone (O₃) - Informations Détaillées", resultat.Titre);
            Assert.Contains("Puissant oxydant", resultat.Description);
        }

        [Fact]
        public void ObtenirInformations_TagInconnu_RetourneValeursParDefaut()
        {
            // Arrange
            string tagFaux = "POLLUANT_INCONNU";

            // Act
            var resultat = PolluantInfoService.ObtenirInformations(tagFaux);

            // Assert
            Assert.Equal("Inconnu", resultat.Titre);
            Assert.Equal("Aucune information disponible pour ce capteur.", resultat.Description);
        }
    }
}