using AcceptanceTests.EqualityComparers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MovieService.DomainLayer.Managers.Models;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace TestingCommon.AssertHelpers
{
    public static class MovieAssertions
    {
        public static void AssertMoviesAreEqual(IEnumerable<Movie> expectedMovies, IEnumerable<Movie> actualMovies)
        {
            var movieEqualityComparer = new MovieEqualityComparer();
            var exceptionMessage = new StringBuilder();

            foreach (var expectedMovie in expectedMovies)
            {

                if (!actualMovies.Contains(expectedMovie, movieEqualityComparer))
                {
                    exceptionMessage.AppendLine($"The Expected Movie {{{expectedMovie.Name}, {expectedMovie.Genre}, {expectedMovie.Year}, {expectedMovie.ImageUrl}, was Not Found in Actual Movies.}}");
                }
            }

            foreach (var actualMovie in actualMovies)
            {

                if (!expectedMovies.Contains(actualMovie, movieEqualityComparer))
                {
                    exceptionMessage.AppendLine($"The Actual Movie {{{actualMovie.Name}, {actualMovie.Genre}, {actualMovie.Year}, {actualMovie.ImageUrl}, was Not Found in Expected Movies.}}");
                }
            }

            if (exceptionMessage.Length != 0)
            {
                throw new AssertFailedException(exceptionMessage.ToString());
            }
        }
    }
}
