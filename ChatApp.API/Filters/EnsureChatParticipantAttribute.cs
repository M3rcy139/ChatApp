using System.Security.Claims;
using ChatApp.Business.Interfaces.Services;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ChatApp.API.Filters;

public class EnsureChatParticipantAttribute: ActionFilterAttribute
{
    public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var chatService = context.HttpContext.RequestServices.GetRequiredService<IChatService>();
        var userId = Guid.Parse(context.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        
        if (context.ActionArguments.TryGetValue("chatId", out var chatIdObj) && chatIdObj is Guid chatId)
        {
            await chatService.EnsureUserIsParticipantAsync(chatId, userId);
        }
        
        await next();
    }
}