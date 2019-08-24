using System;
using System.Diagnostics.CodeAnalysis;

namespace MovieService.DomainLayer.Exceptions
{

    [Serializable]
    [ExcludeFromCodeCoverage]
    public class MovieServiceBusinessBaseException : MovieServiceBaseException
    {
        public MovieServiceBusinessBaseException() { }
        public MovieServiceBusinessBaseException(string message) : base(message) { }
        public MovieServiceBusinessBaseException(string message, Exception inner) : base(message, inner) { }
        protected MovieServiceBusinessBaseException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
