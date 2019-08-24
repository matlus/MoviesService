using MovieService.DomainLayer.Managers.Models;
using MovieService.DomainLayer.Managers.Parsers;
using MovieService.DomainLayer.Managers.Services.ImdbService.ResourceModels;
using MovieService.DomainLayer.Managers.Services.MovieService.Exceptions;
using MovieService.DomainLayer.ServiceLocators;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml;

namespace MovieService.DomainLayer.Managers.Services.ImdbService
{
    internal sealed class ImdbServiceGateway : IDisposable
    {
        private bool _disposed;

        private HttpClient _httpClient;
        public ImdbServiceGateway(string baseUrl, IHttpMessageHandlerProvider httpMessageHandlerProvider)
        {
            _httpClient = CreateHttpClient(baseUrl, httpMessageHandlerProvider);
        }

        private static HttpClient CreateHttpClient(string baseUrl, IHttpMessageHandlerProvider httpMessageHandlerProvider)
        {
            var httpClient = new HttpClient(httpMessageHandlerProvider.CreateHttpMessageHandler());
            httpClient.BaseAddress = new Uri(baseUrl);
            httpClient.DefaultRequestHeaders. Add("Accept-Encoding", "gzip,compress,deflate");
            return httpClient;
        }

        public async Task<IEnumerable<Movie>> GetAllMovies()
        {
            var httpResponseMessage = await _httpClient.GetAsync("AllMovies.json").ConfigureAwait(false);
            await EnsureSuccess(httpResponseMessage).ConfigureAwait(false);

            var moviesResource = await httpResponseMessage.Content.ReadAsAsync<IEnumerable<ImdbMovieResource>>().ConfigureAwait(false);
            return MapMovieResourceToMovie(moviesResource);
        }

        private async Task EnsureSuccess(HttpResponseMessage httpResponseMessage)
        {
            if (httpResponseMessage.IsSuccessStatusCode)
            {
                return;
            }

            var content = await httpResponseMessage.Content.ReadAsStringAsync().ConfigureAwait(false);
            var (code, message) = ExtractCodeAndMessage(content);

            if (httpResponseMessage.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                throw new ImdbNotFoundException($"Calling the IMDB Movie Service resulted in a HTTP Error, with Reason Phrase: {httpResponseMessage.ReasonPhrase}, Code: {code}, and Message: {message}");
            }

            if (httpResponseMessage.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                throw new ImdbBadRequestException($"Calling the IMDB Movie Service resulted in a HTTP Error, with Reason Phrase: {httpResponseMessage.ReasonPhrase}, Code: {code}, and Message: {message}");
            }
        }

        private static (string code, string message) ExtractCodeAndMessage(string content)
        {
            var xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(content);
            var codeNode = xmlDocument.SelectSingleNode("./Error/Code");
            var messageNode = xmlDocument.SelectSingleNode("./Error/Message");
            return (codeNode.InnerText, messageNode.InnerText);
        }

        private static IEnumerable<Movie> MapMovieResourceToMovie(IEnumerable<ImdbMovieResource> moviesResource)
        {
            foreach (var movieResource in moviesResource)
            {
                yield return new Movie(
                    movieResource.Title,
                    GenreParser.Parse(movieResource.Category),
                    movieResource.Year,
                    movieResource.ImageUrl);
            }
        }

        [ExcludeFromCodeCoverage]
        private void Dispose(bool disposing)
        {
            if (disposing && !_disposed)
            {
                var tempHttpClient = _httpClient;
                tempHttpClient.Dispose();
                _httpClient = null;
                _disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
