using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using MovieService.DomainLayer.Exceptions;
using MovieService.ExceptionToReasonPhraseMappers;
using MovieService.Middleware.HttpTranslators;
using System;
using System.Net;
using System.Threading.Tasks;

namespace MovieService.CustomActionResults
{
    internal sealed class ExceptionActionResult : ActionResult
    {
        private readonly Exception _exception;
        internal Exception Exception {  get { return _exception; } }

        public ExceptionActionResult(Exception exception)
        {
            _exception = exception;
        }

        public override async Task ExecuteResultAsync(ActionContext context)
        {
            var httpResponse = context.HttpContext.Response;
            await ExceptionToHttpResponseTranslator.HandleException(httpResponse, _exception).ConfigureAwait(false);
        }
    }
}
