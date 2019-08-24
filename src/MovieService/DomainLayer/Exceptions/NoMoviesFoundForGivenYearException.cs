using System;
using System.Diagnostics.CodeAnalysis;

namespace MovieService.DomainLayer.Exceptions
{

    [Serializable]
    [ExcludeFromCodeCoverage]
    public sealed class NoMoviesFoundForGivenYearException : MovieServiceBusinessBaseException
    {
        public NoMoviesFoundForGivenYearException() { }
        public NoMoviesFoundForGivenYearException(string message) : base(message) { }
        public NoMoviesFoundForGivenYearException(string message, Exception inner) : base(message, inner) { }
        private NoMoviesFoundForGivenYearException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
