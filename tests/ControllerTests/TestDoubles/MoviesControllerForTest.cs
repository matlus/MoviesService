using MovieService.Controllers;
using MovieService.DomainLayer.Managers.Enums;
using MovieService.DomainLayer.Managers.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ControllerTests.TestDoubles
{
    internal sealed class MoviesControllerForTest : MoviesController
    {
        private readonly Exception _exception;
        private readonly IEnumerable<Movie> _movies;

        public MoviesControllerForTest(IEnumerable<Movie> movies)
            :base(domainFacade: null)
        {
            _movies = movies;
        }

        public MoviesControllerForTest(Exception exception)
            :base(domainFacade: null)
        {
            _exception = exception;
        }

        private async Task<IEnumerable<Movie>> GetResponse()
        {
            if (_movies != null)
            {
                return await Task.FromResult(_movies);
            }
            else
            {
                throw _exception;
            }
        }

        protected override async Task<IEnumerable<Movie>> GetAllMovies()
        {
            return await GetResponse();
        }

        protected override async Task<IEnumerable<Movie>> GetMoviesByGenre(Genre genre)
        {
            return await GetResponse();
        }

        protected override async Task<IEnumerable<Movie>> GetMoviesByYear(int year)
        {
            return await GetResponse();
        }
    }
}
