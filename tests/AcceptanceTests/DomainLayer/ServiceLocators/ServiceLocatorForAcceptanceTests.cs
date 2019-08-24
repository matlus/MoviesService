using AcceptanceTests.Spies;
using AcceptanceTests.TestMediators;
using MovieService.DomainLayer.Configuration;
using MovieService.DomainLayer.Managers;
using MovieService.DomainLayer.Managers.Services.ImdbService;
using MovieService.DomainLayer.ServiceLocators;
using System.Net.Http;

namespace AcceptanceTests.DomainLayer.ServiceLocators
{
    internal sealed class ServiceLocatorForAcceptanceTests : ServiceLocatorBase
    {
        private readonly TestMediatorForAcceptanceTests _testMediatorForAcceptanceTests;

        public ServiceLocatorForAcceptanceTests(TestMediatorForAcceptanceTests testMediatorForAcceptanceTests)
        {
            _testMediatorForAcceptanceTests = testMediatorForAcceptanceTests;
        }

        protected override ConfigurationProviderBase CreateConfigurationProviderCore()
        {
            return new ConfigurationProvider();
        }

        protected override HttpMessageHandler CreateHttpMessageHandlerCore()
        {
            return new HttpMessageHandlerSpy(_testMediatorForAcceptanceTests);
        }

        protected override MovieManager CreateMovieManagerCore()
        {
            return new MovieManager(this);
        }

        protected override ImdbServiceGateway CreateMovieServiceGatewayCore(string baseUrl)
        {
            return new ImdbServiceGateway(baseUrl, this);
        }
    }
}
