using AutoMapper;
using GameStore.PL.MappingProfiles;
using Xunit;

namespace GameStore.Tests.MappingsProfilesTest
{
    public class PresentationLayerMapperProfileTest
    {
        [Fact]
        public void PresentationLayerConfiguration_IsValid()
        {
            var configuration = new MapperConfiguration(expression =>
                expression.AddProfile<PresentationLayerMapperProfile>());

            configuration.AssertConfigurationIsValid();
        }
    }
}
