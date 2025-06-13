using System.Net;
using ChatApp.API.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace ChatApp.API.Middleware;

public class ExceptionHandlingBaseMiddleware
{
    protected readonly RequestDelegate _next;
    protected readonly ILogger<ExceptionHandlingBaseMiddleware> _logger;
    protected readonly IWebHostEnvironment _environment;

    protected ExceptionHandlingBaseMiddleware(RequestDelegate next, ILogger<ExceptionHandlingBaseMiddleware> logger, 
        IWebHostEnvironment environment)
    {
        _next = next;
        _logger = logger;
        _environment = environment;
    }

    protected async Task HandleExceptionResponseAsync(HttpContext context, Exception exception, 
        HttpStatusCode statusCode, IEnumerable<object>? errors = null)
    {
        var problemDetails = new CustomProblemDetails
        {
            Status = (int)statusCode,
            Title = exception.Message,
            Instance = context.Request.Path,
            Type = statusCode.ToString(),
            Detail = !_environment.IsProduction() ? exception.ToString() : null,
            Errors = errors
        };

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        await context.Response.WriteAsJsonAsync(problemDetails);
    }
}