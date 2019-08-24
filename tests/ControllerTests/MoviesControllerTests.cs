using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MovieService.Controllers;
using MovieService.CustomActionResults;
using MovieService.DomainLayer.Exceptions;
using MovieService.DomainLayer.Managers.Models;
using MovieService.ResourceModels;
using MovieService.DomainLayer.Managers.Parsers;
using TestingCommon.AssertHelpers;
using MovieService.DomainLayer.Managers.Enums;
using ControllerTests.TestDoubles;
using Microsoft.AspNetCore.Http;
using System.IO;

namespace ControllerTests
{
    [TestClass]
    public class MoviesControllerTests
    {
        [TestMethod]
        [TestCategory("Class Test")]
        public async Task MoviesController_GetAllMovies_WhenAllGoesOk_ShouldReturnAllMovies()
        {
            // Arrange
            var expectedMovies = GetArrangedMovies();
            var moviesController = new MoviesControllerForTest(expectedMovies);

            // Act
            var actionResult = await moviesController.Get().ConfigureAwait(false);
            var okObjectResult = (OkObjectResult)actionResult.Result;
            var actualMovieResources = (IEnumerable<MovieResource>)okObjectResult.Value;

            // Assert
            var actualMovies = MapMovieResourceToMovie(actualMovieResources);
            MovieAssertions.AssertMoviesAreEqual(expectedMovies, actualMovies);
        }

        [TestMethod]
        [TestCategory("Class Test")]
        public async Task MoviesController_GetAllMovies_WhenImdbBaseUrlConfigurationSettingIsMissing_ShouldReturnHttpErrorResponse()
        {
            // Arrange
            var expectedExceptionMessage = "The Configuration Setting: ImdbBaseUrl is missing from the configuration file";
            var configurationSettingMissingException = new ConfigurationSettingMissingException(expectedExceptionMessage);

            var moviesController = new MoviesControllerForTest(configurationSettingMissingException);

            // Act
            var actionResult = await moviesController.Get().ConfigureAwait(false);
            var actualExceptionResult = (ExceptionActionResult)actionResult.Result;

            // Assert
            Assert.IsInstanceOfType(actualExceptionResult.Exception, typeof(ConfigurationSettingMissingException));
            Assert.AreEqual(expectedExceptionMessage, actualExceptionResult.Exception.Message);            
        }

        [TestMethod]
        [TestCategory("Class Test")]
        public async Task MoviesController_GetAllMovies_WhenUnexpectedExceptionOccurs_ShouldReturnStatusCode500Response()
        {
            // Arrange
            var expectedExceptionMessage = "Some arbitary Null reference issue";
            var nullReferenceException = new NullReferenceException(expectedExceptionMessage);

            var moviesController = new MoviesControllerForTest(nullReferenceException);

            // Act
            var actionResult = await moviesController.Get().ConfigureAwait(false);
            var actualExceptionResult = (ExceptionActionResult)actionResult.Result;

            var httpContext = new DefaultHttpContext();
            var actionContext = new ActionContext { HttpContext = httpContext };
            await actualExceptionResult.ExecuteResultAsync(actionContext).ConfigureAwait(false);

            // Assert
            Assert.AreEqual(500, httpContext.Response.StatusCode);
            Assert.AreEqual("NullReferenceException", httpContext.Response.Headers["Exception-Type"][0]);
        }

        [TestMethod]
        [TestCategory("Class Test")]
        public async Task MoviesController_GetMoviesByGenre_WhenCalledWithAValidGenre_ShouldReturnMovies()
        {
            // Arrange
            // Since the Controller is not responsible for filtering by Genre, all we need to assert is that
            // The movies that are arranged/expected are in fact the movies that are returned        
            var expectedMovies = GetArrangedMovies();
            var moviesController = new MoviesControllerForTest(expectedMovies);
            var validGenre = Genre.Drama.ToString();

            // Act
            var actualMovieResources = await moviesController.MoviesByGenre(validGenre).ConfigureAwait(false);

            // Assert
            var actualMovies = MapMovieResourceToMovie(actualMovieResources);
            MovieAssertions.AssertMoviesAreEqual(expectedMovies, actualMovies);
        }

        [TestMethod]
        [TestCategory("Class Test")]
        public async Task MoviesController_GetMoviesByGenre_WhenCalledWithAEmptyGenre_ShouldThrowInvalidGenreException()
        {
            // Arrange
            // Since the Controller is not responsible for filtering by Genre, all we need to assert is that
            // The movies that are arranged/expected are in fact the movies that are returned        
            var expectedMovies = GetArrangedMovies();
            var moviesController = new MoviesControllerForTest(expectedMovies);
            var validGenre = string.Empty;

            try
            {
                // Act
                await moviesController.MoviesByGenre(validGenre).ConfigureAwait(false);
                Assert.Fail("We were expecting an InvalidGenreException to be thrown for an Empty Genre but no Exception was thrown.");
            }
            catch (InvalidGenreException e)
            {
                // Assert
                var exceptionMessagePart = "null or Empty genre is not valid";
                StringAssert.Contains(e.Message, exceptionMessagePart);
            }
        }

        [TestMethod]
        [TestCategory("Class Test")]
        public async Task MoviesController_GetMoviesByGenre_WhenCalledWithANullGenre_ShouldThrowInvalidGenreException()
        {
            // Arrange
            // Since the Controller is not responsible for filtering by Genre, all we need to assert is that
            // The movies that are arranged/expected are in fact the movies that are returned        
            var expectedMovies = GetArrangedMovies();
            var moviesController = new MoviesControllerForTest(expectedMovies);
            string validGenre = null;

            try
            {
                // Act
                await moviesController.MoviesByGenre(validGenre).ConfigureAwait(false);
                Assert.Fail("We were expecting an InvalidGenreException to be thrown for an Empty Genre but no Exception was thrown.");
            }
            catch (InvalidGenreException e)
            {
                // Assert
                var exceptionMessagePart = "null or Empty genre is not valid";
                StringAssert.Contains(e.Message, exceptionMessagePart);
            }
        }

        [TestMethod]
        [TestCategory("Class Test")]
        public async Task MoviesController_GetMoviesByYear_WhenCalledWithAValidYear_ShouldReturnMovies()
        {
            // Arrange
            // Since the Controller is not responsible for filtering by Year, all we need to assert is that
            // The movies that are arranged/expected are in fact the movies that are returned        
            var expectedMovies = GetArrangedMovies();
            var moviesController = new MoviesControllerForTest(expectedMovies);
            var validYear = 1900;

            // Act
            var actualMovieResources = await moviesController.MoviesByYear(validYear).ConfigureAwait(false);

            // Assert
            var actualMovies = MapMovieResourceToMovie(actualMovieResources);
            MovieAssertions.AssertMoviesAreEqual(expectedMovies, actualMovies);
        }

        private static IEnumerable<Movie> MapMovieResourceToMovie(IEnumerable<MovieResource> movieResources)
        {
            foreach (var movieResource in movieResources)
            {
                yield return new Movie(movieResource.Name, GenreParser.Parse(movieResource.Genre), movieResource.Year, movieResource.ImageUrl);
            }
        }

        private static IEnumerable<Movie> GetArrangedMovies()
        {
            yield return new Movie("Star Wars Episode IV: A New Hope", Genre.SciFi, 1977, "StarWarsEpisodeIV.jpg");
            yield return new Movie("Star Wars Episode V: The Empire Strikes Back", Genre.SciFi, 1980, "StarWarsEpisodeV.jpg");
            yield return new Movie("Star Wars Episode VI: Return of the Jedi", Genre.SciFi, 1983, "StarWarsEpisodeVI.jpg");
            yield return new Movie("Star Wars: Episode I: The Phantom Menace", Genre.SciFi, 1999, "StarWarsEpisodeI.jpg");
            yield return new Movie("Star Wars Episode II: Attack of the Clones", Genre.SciFi, 2002, "StarWarsEpisodeII.jpg");
            yield return new Movie("Star Wars: Episode III: Revenge of the Sith", Genre.SciFi, 2005, "StarWarsEpisodeIII.jpg");
            yield return new Movie("Olympus Has Fallen", Genre.Action, 2013, "Olympus_Has_Fallen_poster.jpg");
            yield return new Movie("G.I. Joe: Retaliation", Genre.Action, 2013, "GIJoeRetaliation.jpg");
            yield return new Movie("Jack the Giant Slayer", Genre.Action, 2013, "jackgiantslayer4.jpg");
            yield return new Movie("Drive", Genre.Action, 2011, "FileDrive2011Poster.jpg");
            yield return new Movie("Sherlock Holmes", Genre.Action, 2009, "FileSherlock_Holmes2Poster.jpg");
            yield return new Movie("The Girl with the Dragon Tatoo", Genre.Drama, 2011, "FileThe_Girl_with_the_Dragon_Tattoo_Poster.jpg");
            yield return new Movie("Saving Private Ryan", Genre.Action, 1998, "SavingPrivateRyan.jpg");
            yield return new Movie("Schindlers List", Genre.Drama, 1993, "SchindlersList.jpg");
            yield return new Movie("Good Will Hunting", Genre.Drama, 1997, "FileGood_Will_Hunting_theatrical_poster.jpg");
            yield return new Movie("Citizen Kane", Genre.Drama, 1941, "Citizenkane.jpg");
            yield return new Movie("Shawshank Redemption", Genre.Drama, 1994, "FileShawshankRedemption.jpg");
            yield return new Movie("Forest Gump", Genre.Drama, 1994, "ForrestGump.jpg");
            yield return new Movie("We Bought a Zoo", Genre.Drama, 2011, "FileWe_Bought_a_Zoo_Poster.jpg");
            yield return new Movie("A Beautiful Mind", Genre.Drama, 2001, "FileAbeautifulmindposter.jpg");
            yield return new Movie("Avatar", Genre.SciFi, 2009, "Avatar.jpg");
            yield return new Movie("Iron Man", Genre.SciFi, 2008, "IronMan.jpg");
            yield return new Movie("Terminator 2", Genre.SciFi, 1991, "Terminator2.jpg");
            yield return new Movie("The Dark Knight", Genre.SciFi, 2001, "TheDarkKnight.jpg");
            yield return new Movie("The Matrix", Genre.SciFi, 1999, "TheMatrix.jpg");
            yield return new Movie("Transformers", Genre.SciFi, 2007, "Transformers.jpg");
            yield return new Movie("Revenge Of The Fallen", Genre.SciFi, 2009, "TransformersRevengeOfTheFallen.jpg");
            yield return new Movie("The Dark of the Moon", Genre.SciFi, 2011, "TransformersTheDarkoftheMoon.jpg");
            yield return new Movie("X-Men First Class", Genre.SciFi, 2011, "XMenFirstClass.jpg");
            yield return new Movie("Snitch", Genre.Thriller, 2013, "Snitch.jpg");
            yield return new Movie("Life Of Pi", Genre.Drama, 2012, "LifeOfPi.jpg");
            yield return new Movie("The Call", Genre.Thriller, 2013, "TheCall.jpg");
            yield return new Movie("Wake in Fright", Genre.Thriller, 1971, "WakeInFright.jpg");
            yield return new Movie("Les Miserables", Genre.Musical, 2012, "LesMiserables.jpg");
            yield return new Movie("Footloose", Genre.Musical, 2011, "Footloose.jpg");
            yield return new Movie("The Croods", Genre.Animation, 2013, "TheCroods.jpg");
            yield return new Movie("Oblivion", Genre.SciFi, 2013, "Oblivion.jpg");
        }
    }
}
