namespace ChatApp.Business.DTOs.Requests;

public class RegisterUserRequest
{
    public string UserName { get; set; }
    public string PhoneNumber { get; set; }
    public string Password { get; set; }
}