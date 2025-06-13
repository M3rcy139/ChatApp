using System.Security.Claims;
using ChatApp.API.Contracts;
using ChatApp.API.Filters;
using ChatApp.API.Hubs;
using ChatApp.Business.DTOs.Requests;
using ChatApp.Business.Interfaces.Services;
using ChatApp.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace ChatApp.API.Controllers;

[ApiController]
[Route("api/chats/{chatId}/messages")]
[Authorize]
[EnsureChatParticipant]
public class MessageController : ControllerBase
{
    private readonly IMessageService _messageService;
    private readonly IHubContext<ChatHub> _hubContext;
    
    private Guid CurrentUserId => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    public MessageController(IMessageService messageService, IHubContext<ChatHub> hubContext)
    {
        _messageService = messageService;
        _hubContext = hubContext;
    }

    [HttpPost]
    public async Task<IActionResult> SendMessage([FromRoute] Guid chatId, [FromBody] SendMessageRequest request)
    {
        var message = await _messageService.SendMessageAsync(chatId, CurrentUserId, request.Text);
        
        await _hubContext.Clients.Group(chatId.ToString()).SendAsync(SignalRMethods.ReceiveMessage, new
        {
            ChatId = chatId,
            UserId = CurrentUserId,
            Text = message.Text,
            SentAt = message.SentAt
        });
        
        return Ok(new {message = string.Format(InfoMessages.SentMessage, message.Id)});
    }

    [HttpGet]
    public async Task<IActionResult> GetMessages([FromRoute] Guid chatId, int page = 1, int pageSize = 50)
    {
        var messages = await _messageService.GetMessagesByChatIdAsync(chatId, page, pageSize);
        return Ok(messages);
    }
    
    [HttpPut("{messageId}")]
    public async Task<IActionResult> EditMessage([FromRoute] Guid chatId, Guid messageId, [FromBody] EditMessageRequest request)
    {
        var updatedMessage = await _messageService.EditMessageAsync(chatId, messageId, CurrentUserId, request.Text);

        await _hubContext.Clients.Group(chatId.ToString()).SendAsync(SignalRMethods.MessageEdited, new
        {
            ChatId = chatId,
            MessageId = messageId,
            NewText = updatedMessage.Text,
            EditedAt = updatedMessage.EditedAt
        });

        return Ok(new {message = string.Format(InfoMessages.EditedMessage, updatedMessage.Id)});
    }

    [HttpDelete("{messageId}")]
    public async Task<IActionResult> DeleteMessage([FromRoute] Guid chatId, Guid messageId)
    {
        await _messageService.DeleteMessageAsync(chatId, messageId, CurrentUserId);

        await _hubContext.Clients.Group(chatId.ToString()).SendAsync(SignalRMethods.MessageDeleted, new
        {
            ChatId = chatId,
            MessageId = messageId
        });

        return Ok(new { message = string.Format(InfoMessages.DeletedMessage) });
    }
    
    [HttpGet("search")]
    public async Task<IActionResult> SearchMessages([FromRoute] Guid chatId, [FromQuery] string query)
    {
        var messages = await _messageService.SearchMessagesAsync(chatId, CurrentUserId, query);
        return Ok(messages);
    }
}