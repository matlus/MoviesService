using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MovieService.DomainLayer.Managers.Enums;
using MovieService.ResourceModels;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace EndToEndIntegrationTests
{
    [TestClass]
    public class EndToEndIntegrationTests
    {
        private readonly WebApplicationFactory<MovieService.Startup> _webApplicationFactory;
        private readonly HttpClient _httpClient;

        public EndToEndIntegrationTests()
        {
            _webApplicationFactory = new WebApplicationFactory<MovieService.Startup>();
            _httpClient = _webApplicationFactory.CreateClient();            
        }


        [TestMethod]
        [TestCategory("End-to-End Test")]
        public async Task MoviesService_GetAllMovies_WhenCallToServiceSucceeds_ShouldReturnOneOrMoreMovies()
        {
            // Arrange            

            // Act
            var httpResponseMessage = await _httpClient.GetAsync("/api/movies").ConfigureAwait(false);

            // Assert
            var moviesResource = await httpResponseMessage.Content.ReadAsAsync<IEnumerable<MovieResource>>().ConfigureAwait(false);
            Assert.IsTrue(moviesResource.Any());
        }

        [TestMethod]
        [TestCategory("End-to-End Test")]
        public async Task MoviesService_GetMoviesByGenre_WhenProvidedWithAValidAndExistingGenre_ShouldReturnOneOrMoreMovies()
        {
            // Arrange      
            var validAndExistingGenre = Genre.Action;

            // Act
            var httpResponseMessage = await _httpClient.GetAsync($"/api/movies/genre/{validAndExistingGenre}").ConfigureAwait(false);

            // Assert
            var moviesResource = await httpResponseMessage.Content.ReadAsAsync<IEnumerable<MovieResource>>().ConfigureAwait(false);
            Assert.IsTrue(moviesResource.Any());
        }

        [TestMethod]
        [TestCategory("End-to-End Test")]
        public async Task MoviesService_GetMoviesByGenre_WhenProvidedWithAnInvalidGenre_ShouldReturnHttpErrorResponse()
        {
            // Arrange
            var invvalidGenre = Genre.Action.ToString() + "xxxxx";

            // Act
            var httpResponseMessage = await _httpClient.GetAsync($"/api/movies/genre/{invvalidGenre}").ConfigureAwait(false);

            // Assert
            Assert.AreNotEqual(HttpStatusCode.OK, httpResponseMessage.StatusCode, "The HttpStatusCode is not expected to be \"OK\" (since we're expecting an Error Response), but we received an OK HttpStatusCode");
            Assert.IsTrue(httpResponseMessage.Headers.Contains("Exception-Type"), $"When An exception occurs. The Http Response Message Must contain an Exception-Type HTTP Header but this header was not found. The Headers we did find are: {httpResponseMessage.Headers.Select(keyValuePair => keyValuePair.Key)}");
            Assert.AreNotEqual("OK", httpResponseMessage.ReasonPhrase, "The Reason Phrase should be something similar to the Exception type Thrown, however, we received an \"OK\" Reason phrase");
        }

        [TestMethod]
        [TestCategory("End-to-End Test")]
        public async Task MoviesService_GetMoviesByYear_WhenProvidedWithAValidAndExistingYear_ShouldReturnOneOrMoreMovies()
        {
            // Arrange      
            var validAndExistingYear = 2011;

            // Act
            var httpResponseMessage = await _httpClient.GetAsync($"/api/movies/year/{validAndExistingYear}").ConfigureAwait(false);

            // Assert
            var moviesResource = await httpResponseMessage.Content.ReadAsAsync<IEnumerable<MovieResource>>().ConfigureAwait(false);
            Assert.IsTrue(moviesResource.Any());
        }

    }
}
