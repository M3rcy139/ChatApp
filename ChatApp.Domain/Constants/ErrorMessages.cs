namespace ChatApp.Domain.Constants;

public static class ErrorMessages
{
    public const string ValidationFailed = "Validation error: {0}";
    public const string ArgumentativeException = "An argumentative exception: {0}";
    public const string UnauthorizedAccessException = "An unauthorized access exception: {0}"; 
    public const string InvalidOperationException = "An invalid operation exception: {0}"; 

    public const string UnexpectedErrorWithMessage = "Unexpected error: {0}";
    
    public const string NotParticipantOfChat = "User is not a participant of this chat";
    public const string YouCanOnlyEditOwnMessages = "You can only edit your own messages";
    
    public const string ChatNotFound = "The chat was not found.";
    public const string MessageNotFound = "Message not found in chat.";
    public const string UserNotFound = "User not found.";
    
    public const string FailedToLogin = "Invalid phone number or password.";
    public const string AlreadyExistsUserName = "A user with the same username already exists.";
    public const string AlreadyExistsPhoneNumber = "A user with the same phone number already exists.";
}