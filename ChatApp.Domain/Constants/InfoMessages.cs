namespace ChatApp.Domain.Constants;

public static class InfoMessages
{
    public const string HttpRequest = "HTTP Request: {Id} {Method} {Path} Body: {Body}";
    public const string HttpResponse = "HTTP Response: {Id} {StatusCode} Duration: {Duration}ms Body: {Body}";
    
    public const string RequestProcessingComplete = "Completion of request processing. Path: {Path}, Method: {Method}, StatusCode: {StatusCode}";
    
    public const string SentMessage = "Message with Id: {0} successfully sent.";
    public const string EditedMessage = "Message with Id: {0} was successfully edited.";
    public const string DeletedMessage = "Message was successfully deleted.";
    
    public const string CreatedChat = "Chat with Id: {0} was created successfully";
    
    public const string SuccessfulLogin = "Successfully logged in.";
    public const string SuccessfulRegistration = "Successfully registered a user.";
}