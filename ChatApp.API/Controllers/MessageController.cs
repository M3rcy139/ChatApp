using System.Security.Claims;
using ChatApp.Business.DTOs.Requests;
using ChatApp.Business.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChatApp.API.Controllers;

[ApiController]
[Route("api/messages")]
[Authorize]
public class MessageController : ControllerBase
{
    private readonly IMessageService _messageService;
    private readonly IChatService _chatService;

    public MessageController(IMessageService messageService, IChatService chatService)
    {
        _messageService = messageService;
        _chatService = chatService;
    }

    [HttpPost]
    public async Task<IActionResult> SendMessage(Guid chatId, [FromBody] SendMessageRequest request)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        await _chatService.EnsureUserIsParticipantAsync(chatId, userId);
        
        var message = await _messageService.SendMessageAsync(chatId, userId, request.Text);
        return Ok(message);
    }

    [HttpGet]
    public async Task<IActionResult> GetMessages(Guid chatId, int page = 1, int pageSize = 50)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        await _chatService.EnsureUserIsParticipantAsync(chatId, userId);
        
        var messages = await _messageService.GetMessagesByChatIdAsync(chatId, page, pageSize);
        return Ok(messages);
    }
}