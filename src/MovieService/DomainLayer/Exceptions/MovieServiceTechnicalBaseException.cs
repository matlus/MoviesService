using System;
using System.Diagnostics.CodeAnalysis;

namespace MovieService.DomainLayer.Exceptions
{

    [Serializable]
    [ExcludeFromCodeCoverage]
    public class MovieServiceTechnicalBaseException : MovieServiceBaseException
    {
        public MovieServiceTechnicalBaseException() { }
        public MovieServiceTechnicalBaseException(string message) : base(message) { }
        public MovieServiceTechnicalBaseException(string message, Exception inner) : base(message, inner) { }
        protected MovieServiceTechnicalBaseException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
