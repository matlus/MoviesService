using System;
using System.Diagnostics.CodeAnalysis;

namespace MovieService.DomainLayer.Exceptions
{

    [Serializable]
    [ExcludeFromCodeCoverage]
    public class MovieServiceBaseException : Exception
    {
        public MovieServiceBaseException() { }
        public MovieServiceBaseException(string message) : base(message) { }
        public MovieServiceBaseException(string message, Exception inner) : base(message, inner) { }
        protected MovieServiceBaseException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
