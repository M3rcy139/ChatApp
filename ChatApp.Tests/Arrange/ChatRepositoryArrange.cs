using ChatApp.DataAccess;
using ChatApp.Domain.Models;
using ChatApp.Tests.TestData;

namespace ChatApp.Tests.Arrange;

public class ChatRepositoryArrange
{

    public static async Task<Chat> AddChatWithUsersAsync(ChatAppDbContext context, List<User>? users = null)
    {
        var chat = ChatTestData.CreateChat(users);

        await context.Chats.AddAsync(chat);
        await context.SaveChangesAsync();
        return chat;
    }
}