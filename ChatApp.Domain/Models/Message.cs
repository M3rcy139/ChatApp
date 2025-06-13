namespace ChatApp.Domain.Models;

public class Message
{
    public Guid Id { get; set; }
    public Guid ChatId { get; set; }
    public Guid SenderId { get; set; }
    public string Text { get; set; } = default!;
    public DateTime SentAt { get; set; }
    public DateTime EditedAt { get; set; }
}