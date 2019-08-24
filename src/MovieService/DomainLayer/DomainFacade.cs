using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using MovieService.DomainLayer.Managers;
using MovieService.DomainLayer.Managers.Enums;
using MovieService.DomainLayer.Managers.Models;
using MovieService.DomainLayer.ServiceLocators;

namespace MovieService.DomainLayer
{
    public sealed class DomainFacade : IDisposable
    {
        private bool _disposed;

        private readonly ServiceLocatorBase _serviceLocator;

        private MovieManager _movieManager;
        private MovieManager MovieManager {  get { return _movieManager ?? (_movieManager = _serviceLocator.CreateMovieManager());  } }

        [ExcludeFromCodeCoverage]
        public DomainFacade()
            :this(new ServiceLocator())
        {
        }

        internal DomainFacade(ServiceLocatorBase serviceLocator)
        {
            _serviceLocator = serviceLocator;
        }

        public async Task<IEnumerable<Movie>> GetAllMovies()
        {
            return await MovieManager.GetAllMovies().ConfigureAwait(false);
        }

        public async Task<IEnumerable<Movie>> GetMoviesByGenre(Genre genre)
        {
            return await MovieManager.GetMoviesByGenre(genre).ConfigureAwait(false);
        }

        public async Task<IEnumerable<Movie>> GetMoviesByYear(int year)
        {
            return await MovieManager.GetMoviesByYear(year).ConfigureAwait(false);
        }

        [ExcludeFromCodeCoverage]
        private void Dispose(bool disposing)
        {
            if (disposing && !_disposed)
            {
                var tempMovieManager = _movieManager;
                tempMovieManager.Dispose();
                _movieManager = null;
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
