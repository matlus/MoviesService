using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using MovieService.DomainLayer.Exceptions;
using System;
using System.Net;
using System.Threading.Tasks;

namespace MovieService.Middleware.HttpTranslators
{
    public static class ExceptionToHttpResponseTranslator
    {
        public static async Task HandleException(HttpResponse httpResponse, Exception exception)
        {
            httpResponse.Headers.Add("Exception-Type", exception.GetType().Name);

            if (exception is MovieServiceBaseException e)
            {
                httpResponse.HttpContext.Features.Get<IHttpResponseFeature>().ReasonPhrase = e.Reason;
            }

            if (exception is MovieServiceBusinessBaseException)
            {
                httpResponse.StatusCode = (int)HttpStatusCode.BadRequest;
            }
            else
            {
                httpResponse.StatusCode = (int)HttpStatusCode.InternalServerError;
            }

            await httpResponse.WriteAsync(exception.Message).ConfigureAwait(false);
        }
    }
}
