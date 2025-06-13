using ChatApp.API.Middleware;

namespace ChatApp.API.ServiceCollection;

public static class CustomMiddlewareConfiguration
{
    public static IApplicationBuilder ConfigureCustomMiddleware(this IApplicationBuilder app)
    {
        app.UseMiddleware<HttpLoggerMiddleware>();
        app.UseMiddleware<ExceptionHandlingMiddleware>();

        return app;
    }
}