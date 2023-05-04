using System;
using System.Threading.Tasks;
using GameStore.DomainModels.Exceptions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace GameStore.PL.Middlewares
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;

        private readonly ILogger _logger;

        public ErrorHandlingMiddleware(RequestDelegate next, ILoggerFactory loggerFactory)
        {
            _next = next;

            _logger = loggerFactory.CreateLogger<ErrorHandlingMiddleware>();
        }

        public async Task Invoke(HttpContext context, IWebHostEnvironment env)
        {
            try
            {
                await _next(context);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error occured");

                await HandleExceptionAsync(context, exception, env.IsDevelopment());
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception, bool isDevelopment)
        {
            if (isDevelopment)
            {
                await RedirectToViewResult(context, exception, "Errors/DevelopmentError");
            }
            else if (exception.GetType() == typeof(NotFoundException))
            {
                await RedirectToViewResult(context, exception.Message, "Errors/NotFoundError");
            }
            else if (exception.GetType() == typeof(ArgumentException))
            {
                await RedirectToViewResult(context, exception.Message, "Errors/BadRequest");
            }
            else
            {
                context.Response.Redirect("Errors/Error");
            }
        }

        private async Task RedirectToViewResult(HttpContext context, object model, string path)
        {
            var viewResult = new ViewResult()
            {
                ViewName = path

            };
            var viewDataDictionary = new ViewDataDictionary(new EmptyModelMetadataProvider(),
                                new ModelStateDictionary());
            viewDataDictionary.Model = model;
            viewResult.ViewData = viewDataDictionary;

            var executor = context.RequestServices
                .GetRequiredService<IActionResultExecutor<ViewResult>>();
            var routeData = context.GetRouteData() ?? new RouteData();
            var actionContext = new ActionContext(context, routeData, new ActionDescriptor());

            await executor.ExecuteAsync(actionContext, viewResult);
        }
    }
}
