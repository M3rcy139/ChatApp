using ChatApp.Domain.Models;

namespace ChatApp.Tests.Arrange;

public static class MessageServiceTestData
{
    public const string MessageText = "Message";
    public const string SearchMessage = "test message";
    public const string SearchQuery = "test";
    public const string TsSearchQuery = "test:*";

    public static Message CreateMessage(string text = MessageText) => new Message
    {
        Id = Guid.NewGuid(),
        ChatId = Guid.NewGuid(),
        SenderId = Guid.NewGuid(),
        Text = text
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