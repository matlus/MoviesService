using AcceptanceTests.DomainLayer.ServiceLocators;
using AcceptanceTests.TestMediators;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MovieService.DomainLayer;
using MovieService.DomainLayer.Exceptions;
using MovieService.DomainLayer.Managers.Enums;
using MovieService.DomainLayer.Managers.Models;
using MovieService.DomainLayer.Managers.Services.MovieService.Exceptions;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using TestingCommon.AssertHelpers;
using TestingCommon.TestingHelpers;

namespace AcceptanceTests.DomainLayer
{
    [TestClass]
    public class DomainFacadeTests
    {
        private static DomainFacade _domainFacade;
        private readonly TestMediatorForAcceptanceTests _testMediator;
        private readonly Dictionary<Type, ExceptionInformation> exceptionToHttpErrorInfoMapping;
        public DomainFacadeTests()
        {
            _testMediator = new TestMediatorForAcceptanceTests();
            var serviceLocator = new ServiceLocatorForAcceptanceTests(_testMediator);
            _domainFacade = new DomainFacade(serviceLocator);

            exceptionToHttpErrorInfoMapping = new Dictionary<Type, ExceptionInformation>
            {
                { typeof(ImdbNotFoundException),  new ExceptionInformation { HttpStatusCode = HttpStatusCode.NotFound, Code = "NotFound", Message ="The specified resource does not exist. RequestId:ef8d5b10-101e-0082-7750-7cde60000000", ReasonPhrase = "Resource Not Found" } },
                { typeof(ImdbBadRequestException),  new ExceptionInformation { HttpStatusCode = HttpStatusCode.BadRequest, Code = "BadRequest", Message ="This is a Bad Request. Please change the data you've sent.", ReasonPhrase = "This is a Bad Request" } }
            };
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            _domainFacade.Dispose();
        }

        [TestInitialize]
        public void TestInitialize()
        {
            _testMediator.Reset();
        }


        [TestMethod]
        [TestCategory("Acceptance Test")]
        public async Task DomainFacade_GetAllMovies_WhenMethodIsCalled_ShouldReturnAllMovies()
        {
            // Arrange
            var expectedMovies = GetArrangedMovies();
            _testMediator.HttpStatusCode = HttpStatusCode.OK;
            _testMediator.Movies = expectedMovies;

            // Act
            var actualMovies = await _domainFacade.GetAllMovies().ConfigureAwait(false);

            // Assert
            MovieAssertions.AssertMoviesAreEqual(expectedMovies, actualMovies);
        }

        [TestMethod]
        [TestCategory("Acceptance Test")]
        public async Task DomainFacade_GetAllMovies_WhenARequestIsMadeToAnInvalidEndpoint_ShouldThrowNotFoundException()
        {
            // Arrange
            var exceptionInformation = exceptionToHttpErrorInfoMapping[typeof(ImdbNotFoundException)];
            var expectedReasonPhrase = exceptionInformation.ReasonPhrase;
            var expectedCode = exceptionInformation.Code;
            var expectedMessage = exceptionInformation.Message;

            _testMediator.ExceptionInformation = exceptionInformation;

            
            try
            {
                // Act
                await _domainFacade.GetAllMovies().ConfigureAwait(false);
                Assert.Fail("We were expecting an ImdbNotFoundException, but no exception was thrown");
            }
            catch (ImdbNotFoundException e)
            {
                // Assert
                StringAssert.Contains(e.Message, expectedReasonPhrase, $"We were expecting the Exception message to contain: {expectedReasonPhrase}, but it did not. The actual Exception Message is: {e.Message}");
                StringAssert.Contains(e.Message, expectedCode, $"We were expecting the Exception message to contain: {expectedCode}, but it did not. The actual Exception Message is: {e.Message}");
                StringAssert.Contains(e.Message, expectedMessage, $"We were expecting the Exception message to contain: {expectedMessage}, but it did not. The actual Exception Message is: {e.Message}");
            }
        }

        [TestMethod]
        [TestCategory("Acceptance Test")]
        public async Task DomainFacade_GetAllMovies_WhenARequestIsMadeToAnInvalidEndpoint_ShouldThrowBadRequestException()
        {
            // Arrange
            var exceptionInformation = exceptionToHttpErrorInfoMapping[typeof(ImdbBadRequestException)];
            var expectedReasonPhrase = exceptionInformation.ReasonPhrase;
            var expectedCode = exceptionInformation.Code;
            var expectedMessage = exceptionInformation.Message;

            _testMediator.ExceptionInformation = exceptionInformation;


            try
            {
                // Act
                await _domainFacade.GetAllMovies().ConfigureAwait(false);
                Assert.Fail("We were expecting an ImdbNotFoundException, but no exception was thrown");
            }
            catch (ImdbBadRequestException e)
            {
                // Assert
                AssertEx.AssertExceptionMessageContains(new[] { expectedReasonPhrase, expectedCode, expectedMessage }, e);
            }
        }

        [TestMethod]
        [TestCategory("Acceptance Test")]
        public async Task DomainFacade_GetMoviesByGenre_WhenCalledWithAValidGenre_ShouldReturnMoviesMatchingThatGenre()
        {
            // Arrange
            var validGenre = Genre.Drama;
            var unFilteredMovies = GetArrangedMovies();
            var expectedMovies = GetCopiesOfMoviesMatchingGenre(unFilteredMovies, m => m.Genre == validGenre);

            _testMediator.HttpStatusCode = HttpStatusCode.OK;
            _testMediator.Movies = unFilteredMovies;

            // Act
            var actualMovies = await _domainFacade.GetMoviesByGenre(validGenre).ConfigureAwait(false);

            // Assert
            MovieAssertions.AssertMoviesAreEqual(expectedMovies, actualMovies);
        }

        [TestMethod]
        [TestCategory("Acceptance Test")]
        public async Task DomainFacade_GetMoviesByYear_WhenCalledWithAYearThatHasMovies_ShouldReturnMoviesMatchingThatYear()
        {
            // Arrange
            var yearInWhichMoviesExist = 2011;
            var unFilteredMovies = GetArrangedMovies();
            var expectedMovies = GetCopiesOfMoviesMatchingGenre(unFilteredMovies, m => m.Year == yearInWhichMoviesExist);

            _testMediator.HttpStatusCode = HttpStatusCode.OK;
            _testMediator.Movies = unFilteredMovies;

            // Act
            var actualMovies = await _domainFacade.GetMoviesByYear(yearInWhichMoviesExist).ConfigureAwait(false);

            // Assert
            MovieAssertions.AssertMoviesAreEqual(expectedMovies, actualMovies);
        }

        [TestMethod]
        [TestCategory("Acceptance Test")]
        public async Task DomainFacade_GetMoviesByYear_WhenCalledWithAYearThatDoesNotHaveMovies_ShouldThrowNoMoviesFoundForGivenYearException()
        {
            // Arrange
            var yearInWhichNoMoviesExist = 1900;
            var allMovies = GetArrangedMovies();            

            _testMediator.HttpStatusCode = HttpStatusCode.OK;
            _testMediator.Movies = allMovies;

            // Act
            try
            {
                await _domainFacade.GetMoviesByYear(yearInWhichNoMoviesExist).ConfigureAwait(false);
                Assert.Fail("We were expecting an Exception of type NoMoviesFoundForGivenYearException to be thrown, but no exception was thrown");
            }
            catch (NoMoviesFoundForGivenYearException e)
            {
                // Assert
                AssertEx.AssertExceptionMessageContains(new[] { "No movies Found", yearInWhichNoMoviesExist.ToString() }, e);
            }
        }

        private static IEnumerable<Movie> GetCopiesOfMoviesMatchingGenre(IEnumerable<Movie> movies, Func<Movie, bool> predicate)
        {
            var filteredMovies = new List<Movie>();
            foreach (var movie in movies)
            {
                if (predicate(movie))
                {
                    filteredMovies.Add(new Movie(movie.Name, movie.Genre, movie.Year, movie.ImageUrl));
                }
            }

            return filteredMovies;
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

    internal sealed class ExceptionInformation
    {
        public string Message { get; set; }
        public string ReasonPhrase { get; set; }
        public HttpStatusCode HttpStatusCode { get; set; }
        public string Code { get; set; }
    }
}
