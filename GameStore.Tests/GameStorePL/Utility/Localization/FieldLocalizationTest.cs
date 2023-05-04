using FluentAssertions;
using GameStore.PL.DTOs;
using GameStore.PL.DTOs.LocalizationsDTO;
using GameStore.PL.Util.Localizers;
using System.Collections.Generic;
using Xunit;

namespace GameStore.Tests.GameStorePL.Utility.Localization
{
    public class FieldLocalizationTest
    {
        [Fact]
        public void FieldLocalizer_LocalizedField_ForGoodsDTO()
        {
            string name = "test name";
            string description = "test discription";
            string localizedName = "Тест имя";
            string localizedDesctiption = "Тест описание";

            List<GoodsLocalizationDTO> localization = new List<GoodsLocalizationDTO>()
            {
                new GoodsLocalizationDTO
                {
                    Name = localizedName,
                    Description = localizedDesctiption
                }
            };

            GoodsDTO good = new GoodsDTO()
            {
                Name = name,
                Description = description,
                Localizations = localization
            };

            var actualName = FieldLocalizer.GetLocalizedField(g => g.Name, good);

            actualName.Should().NotBeNull()
                .And.BeOfType<string>()
                .And.BeSameAs(localizedName);

            var actualDescription = FieldLocalizer.GetLocalizedField(g => g.Description, good);

            actualDescription.Should().NotBeNull()
                .And.BeOfType<string>()
                .And.BeSameAs(localizedDesctiption);
        }

        [Fact]
        public void FieldLocalizer_LocalizedField_ForGenreDTO()
        {
            string name = "test name";
            string localizedName = "Тест имя";

            List<GenreLocalizationDTO> localization = new List<GenreLocalizationDTO>()
            {
                new GenreLocalizationDTO
                {
                    Name = localizedName,
                }
            };

            GenreDTO good = new GenreDTO()
            {
                Name = name,
                Localizations = localization
            };

            var actualName = FieldLocalizer.GetLocalizedField(g => g.Name, good);

            actualName.Should().NotBeNull()
                .And.BeOfType<string>()
                .And.BeSameAs(localizedName);
        }

        [Fact]
        public void FieldLocalizer_LocalizedField_ForPlatformTypeDTO()
        {
            string name = "test name";
            string localizedName = "Тест имя";

            List<PlatformTypeLocalizationDTO> localization = new List<PlatformTypeLocalizationDTO>()
            {
                new PlatformTypeLocalizationDTO
                {
                    Type = localizedName,
                }
            };

            PlatformTypeDTO good = new PlatformTypeDTO()
            {
                Type = name,
                Localizations = localization
            };

            var actualName = FieldLocalizer.GetLocalizedField(g => g.Type, good);

            actualName.Should().NotBeNull()
                .And.BeOfType<string>()
                .And.BeSameAs(localizedName);
        }

        [Fact]
        public void FieldLocalizer_FieldWithoutLocalization_ReturnedEmptyString()
        {
            string name = "test name";

            List<PlatformTypeLocalizationDTO> localization = new List<PlatformTypeLocalizationDTO>()
            {
                new PlatformTypeLocalizationDTO()
            };

            PlatformTypeDTO good = new PlatformTypeDTO()
            {
                Type = name,
                Localizations = localization
            };

            var actualName = FieldLocalizer.GetLocalizedField(g => g.Type, good);

            actualName.Should().BeOfType<string>()
                .And.BeSameAs(string.Empty);
        }
    }
}
