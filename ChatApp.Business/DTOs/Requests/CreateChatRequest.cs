namespace ChatApp.Business.DTOs.Requests;

public class CreateChatRequest
{
    public string Name { get; set; } = default!;
    public List<Guid> ParticipantUserIds { get; set; } = new();
}