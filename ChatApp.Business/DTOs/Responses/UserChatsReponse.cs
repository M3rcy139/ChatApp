using ChatApp.Domain.Models;

namespace ChatApp.Business.DTOs.Responses;

public class UserChatsReponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
}