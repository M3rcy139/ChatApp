using ChatApp.Domain.Models;

namespace ChatApp.Tests.TestData;

public static class MessageTestData
{
    public const string MessageText = "Message";
    public const string EditedMessageText = "EditedMessage";
    public const string SearchMessage = "test message";
    public const string UpdatedMessageText = "Updated message";
    public const string SearchQuery = "test";
    public const string TsSearchQuery = "test:*";

    public static Message CreateMessage(string text = MessageText, Guid? chatId = null) => new Message
    {
        Id = Guid.NewGuid(),
        ChatId = chatId ?? Guid.NewGuid(),
        SenderId = Guid.NewGuid(),
        Text = text
    };

    public static Message CreateEditedMessage(string text = EditedMessageText, Guid? chatId = null) => new Message
    {
        Id = Guid.NewGuid(),
        ChatId = chatId ?? Guid.NewGuid(),
        SenderId = Guid.NewGuid(),
        Text = text,
        EditedAt = DateTime.UtcNow,
    };

    public static List<Message> CreateCahedMessages(int count)
    {
        return Enumerable.Range(1, count)
            .Select(i => new Message
            {
                Text = $"{MessageText} {i}"
            }).ToList();
    }

    public static List<Message> CreateMessages(string text = MessageText) => new List<Message>
    {
        CreateMessage(text)
    };
}