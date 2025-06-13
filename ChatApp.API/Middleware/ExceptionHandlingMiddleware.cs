using System.Net;
using ChatApp.Domain.Constants;
using FluentValidation;

namespace ChatApp.API.Middleware;

public class ExceptionHandlingMiddleware : ExceptionHandlingBaseMiddleware
{
    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger,
        IWebHostEnvironment environment) : base (next, logger, environment)
    {
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (ValidationException ex)
        {
            _logger.LogError(ErrorMessages.ValidationFailed, ex.Errors);

            await HandleExceptionResponseAsync(context, ex, HttpStatusCode.BadRequest);
        }
        catch (ArgumentException ex)
        {
            _logger.LogError(ErrorMessages.ArgumentativeException, ex.Message);

            await HandleExceptionResponseAsync(context, ex, HttpStatusCode.BadRequest);
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogError(ErrorMessages.UnauthorizedAccessException, ex.Message);
            
            await HandleExceptionResponseAsync(context, ex, HttpStatusCode.Unauthorized);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(ErrorMessages.InvalidOperationException, ex.Message);

            await HandleExceptionResponseAsync (context, ex, HttpStatusCode.BadRequest);
        }
        catch (Exception ex)
        {
            _logger.LogError(ErrorMessages.UnexpectedErrorWithMessage, ex.Message);

            await HandleExceptionResponseAsync(context, ex, HttpStatusCode.InternalServerError);
        }
        finally
        {
            _logger.LogInformation(InfoMessages.RequestProcessingComplete,
                context.Request.Path, context.Request.Method, context.Response.StatusCode);
        }
    }
}