using AutoMapper;
using GameStore.DAL.MappingProfiles;
using Xunit;

namespace GameStore.Tests.MappingsProfilesTest
{
    public class DataAccessLayerMapperProfileTest
    {
        [Fact]
        public void DataAccessLayerConfiguration_IsValid()
        {
            var configuration = new MapperConfiguration(expression =>
                expression.AddProfile<EntityDomainMapperProfile>());

            configuration.AssertConfigurationIsValid();
        }
    }
}
