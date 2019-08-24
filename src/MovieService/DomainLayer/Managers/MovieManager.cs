using MovieService.DomainLayer.Configuration;
using MovieService.DomainLayer.Exceptions;
using MovieService.DomainLayer.Managers.Enums;
using MovieService.DomainLayer.Managers.Models;
using MovieService.DomainLayer.Managers.Services.ImdbService;
using MovieService.DomainLayer.ServiceLocators;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace MovieService.DomainLayer.Managers
{
    internal sealed class MovieManager : IDisposable
    {
        private bool _disposed;

        private readonly ServiceLocatorBase _serviceLocator;
        private ImdbServiceGateway _movieServiceGateway;
        private ImdbServiceGateway MovieServiceGateway { get { return _movieServiceGateway ?? (_movieServiceGateway = _serviceLocator.CreateMovieServiceGateway(ConfigurationProvider.GetImdbBaseUrl())); } }

        private ConfigurationProviderBase _configurationProvider;
        private ConfigurationProviderBase ConfigurationProvider { get { return _configurationProvider ?? (_configurationProvider = _serviceLocator.CreateConfigurationProvider()); } }

        public MovieManager(ServiceLocatorBase serviceLocator)
        {
            _serviceLocator = serviceLocator;
        }

        public async Task<IEnumerable<Movie>> GetAllMovies()
        {
            return await MovieServiceGateway.GetAllMovies().ConfigureAwait(false);
        }

        public async Task<IEnumerable<Movie>> GetMoviesByGenre(Genre genre)
        {
            var allMovies = await GetAllMovies().ConfigureAwait(false);
            return allMovies.Where(m => m.Genre == genre);
        }

        public async Task<IEnumerable<Movie>> GetMoviesByYear(int year)
        {
            var allMovies = await GetAllMovies().ConfigureAwait(false);
            var moviesByYear = allMovies.Where(m => m.Year == year);

            if (moviesByYear.Any())
            {
                return moviesByYear;
            }

            var distinctYears = allMovies.Select(m => m.Year).Distinct();
            throw new NoMoviesFoundForGivenYearException($"No movies Found for the year: {year}. Valid values for year are: {string.Join(", ", distinctYears)}");
        }

        [ExcludeFromCodeCoverage]
        private void Dispose(bool disposing)
        {
            if (disposing && !_disposed)
            {
                var tempMovieServiceGateway = _movieServiceGateway;
                tempMovieServiceGateway.Dispose();
                _movieServiceGateway = null;
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
