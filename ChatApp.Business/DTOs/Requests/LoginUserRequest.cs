namespace ChatApp.Business.DTOs.Requests;

public class LoginUserRequest
{
    public string PhoneNumber { get; set; } = default!;
    public string Password { get; set; } = default!;
}