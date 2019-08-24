using MovieService.DomainLayer.Configuration;
using MovieService.DomainLayer.Managers;
using MovieService.DomainLayer.Managers.Services.ImdbService;
using System.Net.Http;

namespace MovieService.DomainLayer.ServiceLocators
{
    internal abstract class ServiceLocatorBase : IHttpMessageHandlerProvider
    {
        public MovieManager CreateMovieManager()
        {
            return CreateMovieManagerCore();
        }

        public ImdbServiceGateway CreateMovieServiceGateway(string baseUrl)
        {
            return CreateMovieServiceGatewayCore(baseUrl);
        }

        public ConfigurationProviderBase CreateConfigurationProvider()
        {
            return CreateConfigurationProviderCore();
        }

        public HttpMessageHandler CreateHttpMessageHandler()
        {
            return CreateHttpMessageHandlerCore();
        }

        protected abstract HttpMessageHandler CreateHttpMessageHandlerCore();
        protected abstract ConfigurationProviderBase CreateConfigurationProviderCore();
        protected abstract MovieManager CreateMovieManagerCore();
        protected abstract ImdbServiceGateway CreateMovieServiceGatewayCore(string baseUrl);
    }

    internal interface IHttpMessageHandlerProvider
    {
        HttpMessageHandler CreateHttpMessageHandler();
    }
}
