using System.Security.Claims;
using ChatApp.Business.DTOs.Requests;
using ChatApp.Business.DTOs.Responses;
using ChatApp.Business.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChatApp.API.Controllers;

[ApiController]
[Route("api/chats")]
[Authorize]
public class ChatController : ControllerBase
{
    private readonly IChatService _chatService;
    public ChatController(IChatService chatService)
    {
        _chatService = chatService;
    }

    [HttpGet]
    public async Task<IActionResult> GetChats()
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var chats = await _chatService.GetUserChatsAsync(userId);
        
        var chatDtos = chats.Select(chat => new UserChatsReponse
        {
            Id = chat.Id,
            Name = chat.Name
        }).ToList();

        return Ok(chatDtos);
    }

    [HttpPost]
    public async Task<IActionResult> CreateChat([FromBody] CreateChatRequest request)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var chat = await _chatService.CreateChatAsync(userId, request.Name, request.ParticipantUserIds);
        return Ok(chat);
    }
}