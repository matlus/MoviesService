using System;
using System.Diagnostics.CodeAnalysis;

namespace MovieService.DomainLayer.Exceptions
{

    [Serializable]
    [ExcludeFromCodeCoverage]
    public sealed class InvalidGenreException : MovieServiceBusinessBaseException
    {
        public InvalidGenreException() { }
        public InvalidGenreException(string message) : base(message) { }
        public InvalidGenreException(string message, Exception inner) : base(message, inner) { }
        private InvalidGenreException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
