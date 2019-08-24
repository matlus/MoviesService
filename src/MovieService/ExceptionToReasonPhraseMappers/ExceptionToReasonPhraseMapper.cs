using MovieService.DomainLayer.Exceptions;
using System;
using System.Collections.Generic;

namespace MovieService.ExceptionToReasonPhraseMappers
{
    internal static class ExceptionToReasonPhraseMapper
    {
        private static readonly Dictionary<Type, string> exceptionToReasonPhraseDictionary = new Dictionary<Type, string>
        {
            {typeof(InvalidGenreException), "Invalid Genre"},
            {typeof(NoMoviesFoundForGivenYearException), "No Movies Found for the given Year"},
            {typeof(ConfigurationSettingMissingException), "Configuration Setting Missing" },
            {typeof(ConfigurationSettingKeyNotFoundException), "Configuration Setting Key Not Found" }
        };

        public static string GetReasonPhrase(Exception exception)
        {
            return exceptionToReasonPhraseDictionary[exception.GetType()];
        }
    }
}
