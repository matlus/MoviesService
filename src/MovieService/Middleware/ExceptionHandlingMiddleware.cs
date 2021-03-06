﻿using Microsoft.AspNetCore.Http;
using MovieService.Middleware.HttpTranslators;
using System;
using System.Threading.Tasks;

namespace MovieService.Middleware
{
    public sealed class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        public ExceptionHandlingMiddleware(RequestDelegate requestDelegate)
        {
            _next = requestDelegate;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            try
            {
                await _next.Invoke(httpContext);
            }
            catch (Exception e)
            {
                await ExceptionToHttpResponseTranslator.HandleException(httpContext.Response, e).ConfigureAwait(false);
            }            
        }
    }
}
