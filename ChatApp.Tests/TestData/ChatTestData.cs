using ChatApp.Domain.Models;

namespace ChatApp.Tests.TestData;

public static class ChatTestData
{
    public const string ChatName = "Test Chat";
    
    public static Chat CreateChat( List<User>? users, string name = ChatName)
    {
        return new Chat
        {
            Id = Guid.NewGuid(),
            Name = name,
            Users = users
        };
    }

    public static List<Chat> CreateChats()
    {
        return new List<Chat>
        {
            new() { Id = Guid.NewGuid(), Name = ChatName }
        };
    }
}