using FluentAssertions;
using GameStore.PL;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace GameStore.Tests.IntegrationTests
{
    public class ControllersRoutesTest : IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly WebApplicationFactory<Startup> _factory;
        private readonly HttpClient _client;

        public ControllersRoutesTest(WebApplicationFactory<Startup> factory)
        {
            _factory = factory;
            _client = _factory.CreateClient();
        }

        [Theory]
        [InlineData("games", "text/html; charset=utf-8")]
        [InlineData("games/new", "text/html; charset=utf-8")]
        [InlineData("games/update/testgame", "text/html; charset=utf-8")]
        [InlineData("games/remove/testgame", "text/html; charset=utf-8")]
        [InlineData("genre/Strategy", "text/html; charset=utf-8")]
        [InlineData("genre/new", "text/html; charset=utf-8")]
        [InlineData("genre/update/Strategy", "text/html; charset=utf-8")]
        [InlineData("genre/remove/Strategy", "text/html; charset=utf-8")]
        [InlineData("order", "text/html; charset=utf-8")]
        [InlineData("platform", "text/html; charset=utf-8")]
        [InlineData("platform/mobile", "text/html; charset=utf-8")]
        [InlineData("platform/new", "text/html; charset=utf-8")]
        [InlineData("platform/update/mobile", "text/html; charset=utf-8")]
        [InlineData("platform/remove/mobile", "text/html; charset=utf-8")]
        [InlineData("publisher", "text/html; charset=utf-8")]
        [InlineData("publisher/Test Publisher", "text/html; charset=utf-8")]
        [InlineData("publisher/new", "text/html; charset=utf-8")]
        [InlineData("publisher/update/Test Publisher", "text/html; charset=utf-8")]
        [InlineData("publisher/remove/Test Publisher", "text/html; charset=utf-8")]
        public async Task GetEndpoints_ReturnSuccessAndHtmlContentType(string url, string expectedContentType)
        {
            var response = await _client.GetAsync(url);
            string actualContentType = response.Content.Headers.ContentType.ToString();

            response.EnsureSuccessStatusCode();
            actualContentType.Should().Be(expectedContentType);
        }
    }
}
