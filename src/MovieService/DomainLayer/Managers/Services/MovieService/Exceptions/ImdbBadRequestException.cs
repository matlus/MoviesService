using MovieService.DomainLayer.Exceptions;
using System;
using System.Diagnostics.CodeAnalysis;

namespace MovieService.DomainLayer.Managers.Services.MovieService.Exceptions
{

    [Serializable]
    [ExcludeFromCodeCoverage]
    public sealed class ImdbBadRequestException : MovieServiceBusinessBaseException
    {
        public override string Reason => "Imdb Service - Bad Request";
        public ImdbBadRequestException() { }
        public ImdbBadRequestException(string message) : base(message) { }
        public ImdbBadRequestException(string message, Exception inner) : base(message, inner) { }
        private ImdbBadRequestException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
