using ChatApp.DataAccess;
using ChatApp.Domain.Models;
using ChatApp.Tests.TestData;

namespace ChatApp.Tests.Arrange;

public static class MessageRepositoryArrange
{
    public static async Task<Message> AddMessageAsync(ChatAppDbContext context, string text = MessageTestData.MessageText,
        Guid? chatId = null)
    {
        var message = MessageTestData.CreateMessage(text, chatId);

        await context.Messages.AddAsync(message);
        await context.SaveChangesAsync();
        return message;
    }
}