using Xunit;
using Smart_Territories.Services;

namespace Smart_Territories.Tests
{
    public class SeuilPolluantServiceTests
    {
        [Fact]
        public void ObtenirConfiguration_Ozone_RetourneSeuilsEtUniteCorrects()
        {
            // Act
            var config = SeuilPolluantService.ObtenirConfiguration("Ozone O₃ (µg/m³)");

            // Assert
            Assert.Equal(180, config.SeuilInformation);
            Assert.Equal(240, config.SeuilAlerte);
            Assert.Equal("µg/m³", config.Unite);
            Assert.Equal("0", config.FormatAxe);
        }

        [Fact]
        public void ObtenirConfiguration_PressionAtmospherique_RetourneConfigurationSansSeuilEtFormatDecimal()
        {
            // Act
            var config = SeuilPolluantService.ObtenirConfiguration("Pression atmosphérique (hPa)");

            // Assert
            Assert.Equal(-1, config.SeuilInformation); // Vérifie que l'affichage du seuil sera désactivé
            Assert.Equal(-1, config.SeuilAlerte);
            Assert.Equal("0.0", config.FormatAxe); // Vérifie la présence de la virgule sur l'axe
            Assert.Equal("hPa", config.Unite);
            Assert.Equal(double.NaN, config.ValeurMinimum);
        }
    }
}