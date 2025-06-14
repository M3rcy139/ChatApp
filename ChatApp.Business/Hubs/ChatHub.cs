using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace ChatApp.Business.Hubs;

[Authorize]
public class ChatHub : Hub
{
    public override async Task OnConnectedAsync()
    {
        var chatId = Context.GetHttpContext()!.Request.Query["chatId"];

        if (!string.IsNullOrEmpty(chatId))
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, chatId);
        }

        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var chatId = Context.GetHttpContext()!.Request.Query["chatId"];

        if (!string.IsNullOrEmpty(chatId))
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, chatId);
        }

        await base.OnDisconnectedAsync(exception);
    }
}