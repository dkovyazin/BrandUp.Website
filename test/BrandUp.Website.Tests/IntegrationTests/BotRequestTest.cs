using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace BrandUp.Website.IntegrationTests
{
    public class BotRequestTest : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly CustomWebApplicationFactory factory;

        public BotRequestTest(CustomWebApplicationFactory factory)
        {
            this.factory = factory ?? throw new ArgumentNullException(nameof(factory));

            this.factory.Server.BaseAddress = new Uri("https://localhost/");
        }

        [Theory]
        [InlineData("Mozilla/5.0 (compatible; YandexBot/3.0; +http://yandex.com/bots)")]
        public async Task Default(string userAgent)
        {
            factory.ClientOptions.BaseAddress = new Uri("https://localhost/");
            factory.ClientOptions.AllowAutoRedirect = false;

            using var client = factory.CreateClient();
            client.DefaultRequestHeaders.UserAgent.ParseAdd(userAgent);
            var response = await client.GetAsync("/");

            Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);

            var responseHtml = await response.Content.ReadAsStringAsync();
            Assert.Contains("</body>", responseHtml);

            var visitorStore = (ExampleWebSite.VisitorStore)factory.Services.GetService<Visitors.IVisitorStore>();
            Assert.Equal(0, visitorStore.Count);
        }

        [Theory]
        [InlineData("Mozilla/5.0 (compatible; YandexBot/3.0; +http://yandex.com/bots)")]
        public async Task Navigation(string userAgent)
        {
            factory.ClientOptions.BaseAddress = new Uri("https://localhost/");
            factory.ClientOptions.AllowAutoRedirect = false;

            using var client = factory.CreateClient();
            client.DefaultRequestHeaders.UserAgent.ParseAdd(userAgent);
            var response = await client.PostAsync("/?_nav", new System.Net.Http.StringContent(string.Empty));

            Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);

            var responseHtml = await response.Content.ReadAsStringAsync();
            Assert.Empty(responseHtml);

            var visitorStore = (ExampleWebSite.VisitorStore)factory.Services.GetService<Visitors.IVisitorStore>();
            Assert.Equal(0, visitorStore.Count);
        }

        [Theory]
        [InlineData("Mozilla/5.0 (compatible; YandexBot/3.0; +http://yandex.com/bots)")]
        public async Task Content(string userAgent)
        {
            factory.ClientOptions.BaseAddress = new Uri("https://localhost/");
            factory.ClientOptions.AllowAutoRedirect = false;

            using var client = factory.CreateClient();
            client.DefaultRequestHeaders.UserAgent.ParseAdd(userAgent);
            var response = await client.GetAsync("/?_content");

            Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);

            var responseHtml = await response.Content.ReadAsStringAsync();
            Assert.Empty(responseHtml);

            var visitorStore = (ExampleWebSite.VisitorStore)factory.Services.GetService<Visitors.IVisitorStore>();
            Assert.Equal(0, visitorStore.Count);
        }
    }
}