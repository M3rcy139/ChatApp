using System.Text;
using ChatApp.Domain.Constants;

namespace ChatApp.API.Logging;

public class HttpLoggingHandler
{
    private const int MaxBodyLength = 2048;
        
    public static async Task LogRequestAsync(HttpContext context, ILogger logger, string requestId)
    {
        var requestBody = await ReadRequestBodyAsync(context.Request);
        logger.LogInformation(InfoMessages.HttpRequest,
            requestId,
            context.Request.Method,
            context.Request.Path,
            Truncate(requestBody));
    }

    public static async Task LogResponseAsync(HttpContext context, ILogger logger, string requestId, long duration)
    {
        context.Response.Body.Seek(0, SeekOrigin.Begin);
        var responseBody = await new StreamReader(context.Response.Body).ReadToEndAsync();
        context.Response.Body.Seek(0, SeekOrigin.Begin);

        logger.LogInformation(InfoMessages.HttpResponse,
            requestId,
            context.Response.StatusCode,
            duration,
            Truncate(responseBody));
    }

    private static async Task<string> ReadRequestBodyAsync(HttpRequest request)
    {
        request.EnableBuffering();

        if (request.ContentLength is null or 0)
            return string.Empty;

        var buffer = new byte[Convert.ToInt32(request.ContentLength)];
        await request.Body.ReadAsync(buffer, 0, buffer.Length);
        request.Body.Position = 0;

        return Encoding.UTF8.GetString(buffer);
    }

    private static string Truncate(string value)
    {
        if (string.IsNullOrEmpty(value))
            return string.Empty;

        return value.Length <= MaxBodyLength
            ? value
            : value.Substring(0, MaxBodyLength) + "...(truncated)";
    }
}