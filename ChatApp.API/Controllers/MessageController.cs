using System.Security.Claims;
using ChatApp.API.Filters;
using ChatApp.Business.DTOs.Requests;
using ChatApp.Business.Interfaces.Services;
using ChatApp.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChatApp.API.Controllers;

[ApiController]
[Route("api/chats/{chatId}/messages")]
[Authorize]
[EnsureChatParticipant]
public class MessageController : ControllerBase
{
    private readonly IMessageService _messageService;
    private readonly IChatNotificationService _chatNotificationService;
    
    private Guid CurrentUserId => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    public MessageController(IMessageService messageService, IChatNotificationService chatNotificationService)
    {
        _messageService = messageService;
        _chatNotificationService = chatNotificationService;
    }

    [HttpPost]
    public async Task<IActionResult> SendMessage([FromRoute] Guid chatId, [FromBody] SendMessageRequest request)
    {
        var message = await _messageService.SendMessageAsync(chatId, CurrentUserId, request.Text);
        
        await _chatNotificationService.NotifyMessageSentAsync(chatId, CurrentUserId, message.Text, message.SentAt);
        
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

        await _chatNotificationService.NotifyMessageEditedAsync(chatId, messageId, updatedMessage.Text, updatedMessage.EditedAt);

        return Ok(new {message = string.Format(InfoMessages.EditedMessage, updatedMessage.Id)});
    }

    [HttpDelete("{messageId}")]
    public async Task<IActionResult> DeleteMessage([FromRoute] Guid chatId, Guid messageId)
    {
        await _messageService.DeleteMessageAsync(chatId, messageId, CurrentUserId);

        await _chatNotificationService.NotifyMessageDeletedAsync(chatId, messageId);

        return Ok(new { message = string.Format(InfoMessages.DeletedMessage) });
    }
    
    [HttpGet("search")]
    public async Task<IActionResult> SearchMessages([FromRoute] Guid chatId, [FromQuery] string query)
    {
        var messages = await _messageService.SearchMessagesAsync(chatId, CurrentUserId, query);
        return Ok(messages);
    }
}