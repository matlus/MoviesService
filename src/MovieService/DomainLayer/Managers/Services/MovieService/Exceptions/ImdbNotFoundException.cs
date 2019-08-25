using MovieService.DomainLayer.Exceptions;
using System;
using System.Diagnostics.CodeAnalysis;

namespace MovieService.DomainLayer.Managers.Services.MovieService.Exceptions
{


    [Serializable]
    [ExcludeFromCodeCoverage]
    public sealed class ImdbNotFoundException : MovieServiceTechnicalBaseException
    {
        public override string Reason => "Imdb Service - Not Nound";
        public ImdbNotFoundException() { }
        public ImdbNotFoundException(string message) : base(message) { }
        public ImdbNotFoundException(string message, Exception inner) : base(message, inner) { }
        private ImdbNotFoundException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
