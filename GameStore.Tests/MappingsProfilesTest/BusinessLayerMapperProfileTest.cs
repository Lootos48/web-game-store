using AutoMapper;
using GameStore.BLL.MappingProfiles;
using Xunit;

namespace GameStore.Tests.MappingsProfilesTest
{
    public class BusinessLayerMapperProfileTest
    {
        [Fact]
        public void BusinessLayerConfiguration_IsValid()
        {
            var configuration = new MapperConfiguration(expression =>
                expression.AddProfile<BusinessMappingProfile>());

            configuration.AssertConfigurationIsValid();
        }
    }
}
