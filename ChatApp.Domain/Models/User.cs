namespace ChatApp.Domain.Models;

public class User
{
    public Guid Id { get; set; }
    public string UserName { get; set; } = default!;
    public string PhoneNumber { get; set; } = default!;
    public byte[] PasswordHash { get; set; } = default!;
    public ICollection<Chat> Chats { get; set; } = new List<Chat>();
}