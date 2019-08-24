using AcceptanceTests.DomainLayer;
using AcceptanceTests.TestMediators;
using MovieService.DomainLayer.Managers.Models;
using MovieService.DomainLayer.Managers.Parsers;
using MovieService.DomainLayer.Managers.Services.ImdbService.ResourceModels;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AcceptanceTests.Spies
{
    class HttpMessageHandlerSpy : HttpMessageHandler
    {
        private readonly TestMediatorForAcceptanceTests _testMediator;
        public HttpMessageHandlerSpy(TestMediatorForAcceptanceTests testMediator)
        {
            _testMediator = testMediator;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            StringContent stringContent = null;
            var httpResponseMessage = new HttpResponseMessage();

            if (_testMediator.ExceptionInformation == null)
            {
                var imdbMovies = MapMoviesToImdbMovies(_testMediator.Movies);
                var imdbJson = JsonConvert.SerializeObject(imdbMovies);
                httpResponseMessage.StatusCode = _testMediator.HttpStatusCode;
                stringContent = new StringContent(imdbJson, Encoding.UTF8, "application/json");                
            }
            else
            {
                httpResponseMessage.StatusCode = _testMediator.ExceptionInformation.HttpStatusCode;
                httpResponseMessage.ReasonPhrase = _testMediator.ExceptionInformation.ReasonPhrase;
                var xmlContent = ProduceXmlContent(_testMediator.ExceptionInformation);
                stringContent = new StringContent(xmlContent, Encoding.UTF8, "application/xml");
            }

            httpResponseMessage.Content = stringContent;
            return Task.FromResult(httpResponseMessage);
        }

        private string ProduceXmlContent(ExceptionInformation exceptionInformation)
        {
            return $"<?xml version=\"1.0\" encoding=\"utf-8\"?><Error><Code>{exceptionInformation.Code}</Code><Message>{exceptionInformation.Message}</Message></Error>";
        }

        private IEnumerable<ImdbMovieResource> MapMoviesToImdbMovies(IEnumerable<Movie> movies)
        {
            foreach (var movie in movies)
            {
                yield return new ImdbMovieResource { Title = movie.Name, Category = GenreParser.ToString(movie.Genre), ImageUrl = movie.ImageUrl, Year = movie.Year };
            }
        }
    }
}
