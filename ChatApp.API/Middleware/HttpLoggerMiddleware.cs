using System.Diagnostics;
using ChatApp.API.Logging;

namespace ChatApp.API.Middleware;

public class HttpLoggerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<HttpLoggerMiddleware> _logger;
    private const string RequestId = "RequestId";

    public HttpLoggerMiddleware(RequestDelegate next, ILogger<HttpLoggerMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var requestId = Guid.NewGuid().ToString();
        context.Items[RequestId] = requestId;

        var stopwatch = Stopwatch.StartNew();

        await HttpLoggingHandler.LogRequestAsync(context, _logger, requestId);

        var originalBodyStream = context.Response.Body;
        await using var responseBody = new MemoryStream();
        context.Response.Body = responseBody;

        await _next(context);

        stopwatch.Stop();

        await HttpLoggingHandler.LogResponseAsync(context, _logger, requestId, stopwatch.ElapsedMilliseconds);

        await responseBody.CopyToAsync(originalBodyStream);
    }
}