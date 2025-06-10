namespace ChatApp.Domain.Constants;

public static class InfoMessages
{
    public const string HttpRequest = "HTTP Request: {Id} {Method} {Path} Body: {Body}";
    public const string HttpResponse = "HTTP Response: {Id} {StatusCode} Duration: {Duration}ms Body: {Body}";
    
    public const string RequestProcessingComplete = "Completion of request processing. Path: {Path}, Method: {Method}, StatusCode: {StatusCode}";
}