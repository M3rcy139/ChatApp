using System.Security.Claims;
using ChatApp.Business.DTOs.Requests;
using ChatApp.Business.DTOs.Responses;
using ChatApp.Business.Interfaces.Services;
using ChatApp.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChatApp.API.Controllers;

[ApiController]
[Route("api/chats")]
[Authorize]
public class ChatController : ControllerBase
{
    private readonly IChatService _chatService;
    
    private Guid CurrentUserId => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
    public ChatController(IChatService chatService)
    {
        _chatService = chatService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateChat([FromBody] CreateChatRequest request)
    {
        var chat = await _chatService.CreateChatAsync(CurrentUserId, request.Name, request.ParticipantUserIds);
        return Ok(new {message = string.Format(InfoMessages.CreatedChat, chat.Id)});
    }

    [HttpGet]
    public async Task<IActionResult> GetChats()
    {
        var chats = await _chatService.GetUserChatsAsync(CurrentUserId);
        
        var chatDtos = chats.Select(chat => new UserChatsReponse
        {
            Id = chat.Id,
            Name = chat.Name
        }).ToList();

        return Ok(chatDtos);
    }
}