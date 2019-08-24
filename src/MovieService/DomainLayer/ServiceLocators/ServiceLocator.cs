using System.Net.Http;
using MovieService.DomainLayer.Configuration;
using MovieService.DomainLayer.Managers;
using MovieService.DomainLayer.Managers.Services.ImdbService;

namespace MovieService.DomainLayer.ServiceLocators
{
    internal class ServiceLocator : ServiceLocatorBase
    {
        protected override MovieManager CreateMovieManagerCore()
        {
            return new MovieManager(this);
        }

        protected override ImdbServiceGateway CreateMovieServiceGatewayCore(string baseUrl)
        {
            return new ImdbServiceGateway(baseUrl, this);
        }

        protected override ConfigurationProviderBase CreateConfigurationProviderCore()
        {
            return new ConfigurationProvider();
        }

        protected override HttpMessageHandler CreateHttpMessageHandlerCore()
        {
            return new HttpClientHandler();
        }
    }
}
