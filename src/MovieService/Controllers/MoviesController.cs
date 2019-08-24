using Microsoft.AspNetCore.Mvc;
using MovieService.CustomActionResults;
using MovieService.DomainLayer;
using MovieService.DomainLayer.Managers.Enums;
using MovieService.DomainLayer.Managers.Models;
using MovieService.DomainLayer.Managers.Parsers;
using MovieService.ResourceModels;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace MovieService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MoviesController : ControllerBase
    {
        /// <summary>
        /// This action returns an ActionResult&lt;T&gt; instead of an IEnumerable&lt;MovieResource&gt;
        /// That is because we're handling Exceptions in this action as well and in order to be
        /// able to return 2 different types from the same method (and T and an Exception result)
        /// we need to resort to an ActionResult type where
        /// the IEnumerable&lt;MovieResource&gt; is wrapped in an OkObjectResult and the Exception
        /// is wrapped in a custom ExceptionActionResult where both are really ActionResult
        /// This is one way to do it (handle exceptions in evey Action of a controller. The other
        /// option is to handle Exceptions in the Middleware (preferred). The other actions
        /// don't have any exception handling but rather let the middleware handled exceptions
        /// </summary>
        /// <returns>ActionResult&lt;IEnumerable&lt;MovieResource&gt;&gt;</returns>
        // GET api/movies
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MovieResource>>> Get()
        {
            try
            {
                var movies = await GetAllMovies().ConfigureAwait(false);
                return new OkObjectResult(MapToMovieResource(movies));
            }
            catch (Exception e)
            {
                return new ExceptionActionResult(e);
            }
        }

        [HttpGet]
        [Route("genre/{genreAsString}")]
        public async Task<IEnumerable<MovieResource>> MoviesByGenre(string genreAsString)
        {
            var genre = GenreParser.Parse(genreAsString);
            var movies = await GetMoviesByGenre(genre).ConfigureAwait(false);
            return MapToMovieResource(movies);
        }

        [HttpGet]
        [Route("year/{year}")]
        public async Task<IEnumerable<MovieResource>> MoviesByYear(int year)
        {
            var movies = await GetMoviesByYear(year).ConfigureAwait(false);
            return MapToMovieResource(movies);
        }

        private static IEnumerable<MovieResource> MapToMovieResource(IEnumerable<Movie> movies)
        {
            foreach (var movie in movies)
            {
                yield return new MovieResource { Name = movie.Name, Genre = GenreParser.ToString(movie.Genre), ImageUrl = movie.ImageUrl, Year = movie.Year };
            }
        }

        protected virtual async Task<IEnumerable<Movie>> GetAllMovies()
        {
            var domainFacade = new DomainFacade();
            return await domainFacade.GetAllMovies().ConfigureAwait(false);
        }

        protected virtual async Task<IEnumerable<Movie>> GetMoviesByGenre(Genre genre)
        {
            var domainFacade = new DomainFacade();
            return await domainFacade.GetMoviesByGenre(genre).ConfigureAwait(false);
        }

        protected virtual async Task<IEnumerable<Movie>> GetMoviesByYear(int year)
        {
            var domainFacade = new DomainFacade();
            return await domainFacade.GetMoviesByYear(year).ConfigureAwait(false);
        }

    }
}
